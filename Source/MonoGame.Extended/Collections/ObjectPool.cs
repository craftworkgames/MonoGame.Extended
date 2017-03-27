using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended.Collections
{
    public enum ObjectPoolIsFullPolicy
    {
        ReturnNull,
        Resize,
        KillExisting,
    }

    public class ObjectPool<T> : IEnumerable<T>
        where T : class, IPoolable
    {
        private readonly Deque<T> _freeItems;
        private readonly Deque<T> _usedItems;

        private readonly Func<T> _instantiationFunction;

        public ObjectPoolIsFullPolicy IsFullPolicy { get; }
        public int Capacity { get; private set; }
        public int Count { get; private set; }

        public ObjectPool(Func<T> instantiationFunc, int initialSize = 16, ObjectPoolIsFullPolicy isFullPolicy = ObjectPoolIsFullPolicy.ReturnNull)
        {
            if (instantiationFunc == null)
                throw new ArgumentNullException(nameof(instantiationFunc));

            _instantiationFunction = instantiationFunc;
            _freeItems = new Deque<T>(initialSize);
            _usedItems = new Deque<T>(initialSize);
            IsFullPolicy = isFullPolicy;
            
            for (var i = 0; i < initialSize; i++)
            {
                var poolable = CreateObject();
                _freeItems.AddToBack(poolable);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _usedItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _usedItems.GetEnumerator();
        }

        public void ReturnAll()
        {
            while (_usedItems.Count > 0)
            {
                T item;
                _usedItems.GetFront(out item);
                item.Return();
            }
        }

        public T New()
        {
            T poolable;

            if (!_freeItems.RemoveFromFront(out poolable))
            {
                if (Count <= Capacity)
                {
                    poolable = CreateObject();
                }
                else
                {
                    switch (IsFullPolicy)
                    {
                        case ObjectPoolIsFullPolicy.ReturnNull:
                            return null;
                        case ObjectPoolIsFullPolicy.Resize:
                            Capacity++;
                            poolable = CreateObject();
                            break;
                        case ObjectPoolIsFullPolicy.KillExisting:
                            if (!_usedItems.GetFront(out poolable))
                                return null;
                            poolable.Return();
                            _freeItems.RemoveFromBack(out poolable);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            poolable.Initialize(Return);
            _usedItems.AddToBack(poolable);
            return poolable;
        }

        private T CreateObject()
        {
            Count++;
            var poolable = _instantiationFunction();
            if (poolable == null)
                throw new NullReferenceException($"The created pooled object of type '{typeof(T).Name}' is null.");
            return poolable;
        }

        private void Return(IPoolable poolable)
        {
            var poolable1 = (T) poolable;
            _freeItems.AddToBack(poolable1);
            _usedItems.Remove(poolable1);
        }
    }
}