using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MonoGame.Extended.Collections
{
    // derived from: http://www.codeproject.com/Articles/535735/Implementing-a-Generic-Object-Pool-in-NET
    public class ObjectPool<T> : IDisposable where T : IPoolable
    {
        private const int _defaultPoolMinimumSize = 0;
        private const int _defaultPoolMaximumSize = 50;

        private int _inUseCount;
        private int _resetFailedCount;
        private int _ressurectedCount;
        private int _hitCount;
        private int _missedCount;
        private int _createdCount;
        private int _destroyedCount;
        private int _overflowCount;
        private int _returnedCount;
        private long _isDisposed;
        private int _minimumSize;
        private int _maximumSize;

        private readonly ConcurrentQueue<T> _objects = new ConcurrentQueue<T>();
        private readonly ConcurrentDictionary<T, bool> _createdObjects = new ConcurrentDictionary<T, bool>(); 
        private int _adjustSizeInProgress;

        public bool IsDisposed => Interlocked.Read(ref _isDisposed) == 1;
        public int InUseCount => _inUseCount;
        public int ResetFailedCount => _resetFailedCount;
        public int RessurectedCount => _ressurectedCount;
        public int HitCount => _hitCount;
        public int MissedCount => _missedCount;
        public int CreatedCount => _createdCount;
        public int DestroyedCount => _destroyedCount;
        public int OverflowCount => _overflowCount;
        public int ReturnedCount => _returnedCount;
        public int AvailableCount => _objects.Count;

        public int MinimumSize
        {
            get { return _minimumSize; }
            set
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                CheckLimits(value, _maximumSize);
                _minimumSize = value;
                AdjustSize();
            }
        }

        public int MaximumSize
        {
            get { return _maximumSize; }
            set
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                CheckLimits(_minimumSize, value);
                _maximumSize = value;
                AdjustSize();
            }
        }

        public Func<T> CreateNewFunction { get; }

        public ObjectPool(int minimumSize = _defaultPoolMinimumSize, int maximumSize = _defaultPoolMaximumSize, Func<T> createNewFunction = null)
        {
            CheckLimits(minimumSize, maximumSize);
            CreateNewFunction = createNewFunction;
            _maximumSize = maximumSize;
            _minimumSize = minimumSize;
            AdjustSize();
        }

        ~ObjectPool()
        {
            Dispose();
        }

        public void Dispose()
        {
            var wasDisposed = Interlocked.CompareExchange(ref _isDisposed, 1, 0);
            if (wasDisposed != 0 || _isDisposed != 1)
            {
                return;
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing)
            {
                return;
            }

            while (_objects.Count > 0)
            {
                T poolable;
                if (_objects.TryDequeue(out poolable))
                {
                    DestroyObject(poolable);
                }
            }

            foreach (var keyValuePair in _createdObjects)
            {
                var poolable = keyValuePair.Key;
                Interlocked.Decrement(ref _inUseCount);
                DestroyObject(poolable);
            }

            _createdObjects.Clear();
        }

        private void CheckLimits(int minimumSize, int maximumSize)
        {
            if (minimumSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumSize), minimumSize, "Minimum pool size must be greater or equal to zero.");
            }

            if (maximumSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumSize), maximumSize, "Maximum pool size must be greater than zero.");
            }

            if (minimumSize > maximumSize)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumSize), minimumSize, "Minimum pool size must be less than or equal to the maximum pool size.");
            }
        }

        private void AdjustSize()
        {
            if (Interlocked.CompareExchange(ref _adjustSizeInProgress, 1, 0) != 0)
            {
                return;
            }

            while (_createdObjects.Count < MinimumSize)
            {
                _objects.Enqueue(CreateObject());
            }

            while (_createdObjects.Count > MaximumSize)
            {
                T poolable;

                if (!_objects.TryDequeue(out poolable))
                    continue;

                //TODO: Policy right now for this is if the object is in-use don't touch it. Will possibly change.

                Interlocked.Increment(ref _overflowCount);
                DestroyObject(poolable);
            }

            _adjustSizeInProgress = 0;
        }

        private T CreateObject()
        {
            T newObject;

            if (CreateNewFunction != null)
            {
                newObject = CreateNewFunction();
            }
            else
            {
                newObject = (T)Activator.CreateInstance(typeof(T));
            }

            if (newObject == null)
            {
                throw new NullReferenceException($"The created pooled object of type '{typeof (T).Name}' is null.");
            }

            if (!_createdObjects.TryAdd(newObject, false))
            {
                throw new Exception($"A pooled object of type '{typeof (T).Name}' has already been created with the hash code '{newObject.GetHashCode()}'. Hash codes for pooled objects must be unique.");
            }

            Interlocked.Increment(ref _createdCount);
            return newObject;
        }

        private void DestroyObject(T poolable)
        {
            bool isDisposed;

            if (!_createdObjects.TryRemove(poolable, out isDisposed))
            {
                return;
            }

            if (isDisposed)
            {
                throw new ObjectDisposedException(typeof (T).Name);
            }

            poolable.Dispose();
            Interlocked.Increment(ref _destroyedCount);
        }

        public T GetObject()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            T obj;
            if (_objects.TryDequeue(out obj))
            {
                Interlocked.Increment(ref _hitCount);
                Task.Factory.StartNew(AdjustSize);
            }
            else
            {
                //TODO: Add enumeration specifying how to handle a miss.
                Interlocked.Increment(ref _missedCount);
                obj = CreateObject();
            }

            Interlocked.Increment(ref _inUseCount);
            return obj;
        }

        public void ReturnObject(T poolable, bool resurrectObject = false)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (AvailableCount >= MaximumSize)
            {
                Interlocked.Increment(ref _overflowCount);
                DestroyObject(poolable);
            }
            else if (!poolable.ResetState())
            {
                Interlocked.Increment(ref _resetFailedCount);
                DestroyObject(poolable);
            }

            if (resurrectObject)
            {
                Interlocked.Increment(ref _ressurectedCount);
                GC.ReRegisterForFinalize(poolable);
            }

            Interlocked.Decrement(ref _inUseCount);
            Interlocked.Increment(ref _returnedCount);
            _objects.Enqueue(poolable);
        }
    }
}


