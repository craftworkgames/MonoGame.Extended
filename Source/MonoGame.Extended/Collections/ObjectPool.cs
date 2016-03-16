using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MonoGame.Extended.Collections
{
    // derived from: http://www.codeproject.com/Articles/535735/Implementing-a-Generic-Object-Pool-in-NET
    public partial class ObjectPool<T> : IDisposable where T : class, IPoolable
    {
        private const int DefaultPoolMinimumSize = 0;
        private const int DefaultPoolMaximumSize = 50;

        private long _isDisposed;
        private int _minimumSize;
        private int _maximumSize;

        private readonly ConcurrentQueue<T> _objects = new ConcurrentQueue<T>();
        private readonly ConcurrentDictionary<T, bool> _createdObjects = new ConcurrentDictionary<T, bool>(); 
        private int _adjustSizeInProgress;

        public bool IsDisposed
        {
            get { return Interlocked.Read(ref _isDisposed) == 1; }
        }

        public ObjectPool.Diagnostics Diagnostics { get; } = new ObjectPool.Diagnostics();

        public int AvailableCount
        {
            get { return _objects.Count; }
        }

        public int MinimumSize
        {
            get { return _minimumSize; }
            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }

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
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }

                CheckLimits(_minimumSize, value);
                _maximumSize = value;
                AdjustSize();
            }
        }

        public Func<T> FactoryMethod { get; }

        public ObjectPool(int minimumSize = DefaultPoolMinimumSize, int maximumSize = DefaultPoolMaximumSize, Func<T> factoryMethod = null)
        {
            CheckLimits(minimumSize, maximumSize);
            FactoryMethod = factoryMethod;
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
                Diagnostics.DecrementInstancesInUse();
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
                {
                    continue;
                }

                Diagnostics.DecrementInstancesInUse();
                Diagnostics.IncrementInstancesOverflow();
                DestroyObject(poolable);
            }

            _adjustSizeInProgress = 0;
        }

        private T CreateObject()
        {
            T newObject;

            if (FactoryMethod != null)
            {
                newObject = FactoryMethod();
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

            Diagnostics.IncrementInstancesCreated();
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

            var disposable = poolable as IDisposable;
            disposable?.Dispose();
            Diagnostics.IncrementInstancesDestroyed();
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
                Diagnostics.IncrementInstancesHit();
                Task.Factory.StartNew(AdjustSize);
            }
            else
            {
                //TODO: Add enumeration specifying how to handle a miss.
                Diagnostics.IncrementInstancesMissed();
                obj = CreateObject();
            }

            Diagnostics.IncrementInstancesInUse();
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
                Diagnostics.IncrementInstancesOverflow();
                DestroyObject(poolable);
            }
            else if (!poolable.ResetState())
            {
                Diagnostics.IncrementInstancesResetStateFailed();
                DestroyObject(poolable);
            }

            if (resurrectObject)
            {
                Diagnostics.IncrementInstancesRessurected();
                GC.ReRegisterForFinalize(poolable);
            }

            Diagnostics.DecrementInstancesInUse();
            Diagnostics.IncrementInstancesReturned();
            _objects.Enqueue(poolable);
        }
    }
}


