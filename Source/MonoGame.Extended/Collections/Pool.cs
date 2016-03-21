using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended.Collections
{
    public class Pool<T> : ICollection<T>
        where T : class, IPoolable
    {
        private readonly Deque<T> _freeItems;
        private readonly Deque<T> _usedItems;
        private readonly Func<int, T> _newObjectFunction;

        public int Count
        {
            get { return _usedItems.Count; }
        }

        public int Capacity
        {
            get { return _usedItems.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        public Pool(int capacity, Func<int, T> newObjectFunction)
        {
            if (newObjectFunction == null)
            {
                throw new ArgumentNullException(nameof(newObjectFunction));
            }

            _newObjectFunction = newObjectFunction;
            _freeItems = new Deque<T>(capacity);
            _usedItems = new Deque<T>(capacity);
            for (var i = 0; i < capacity; i++)
            {
                var poolable = CreateObject(i);
                _freeItems.AddToBack(poolable);
            }
        }

        private T CreateObject(int index)
        {
            var newObject = _newObjectFunction(index);
            if (newObject == null)
            {
                throw new NullReferenceException($"The created pooled object of type '{typeof (T).Name}' is null.");
            }
            return newObject;
        }

        public T Create(bool killIfEmpty = false)
        {
            while (true)
            {
                T poolable;

                if (_freeItems.RemoveFromFront(out poolable))
                {
                    poolable.Initialize(Return);
                    _usedItems.AddToBack(poolable);
                    return poolable;
                }

                if (!killIfEmpty)
                {
                    return null;
                }
                if (!_usedItems.GetFront(out poolable))
                {
                    return null;
                }

                poolable.Return();
                killIfEmpty = false;
            }
        }

        private void Return(IPoolable poolable)
        {
            poolable.ResetState();
            var poolable1 = (T)poolable;
            _freeItems.AddToBack(poolable1);
            _usedItems.Remove(poolable1);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _usedItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _usedItems.GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Contains(T item)
        {
            return _usedItems.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            _usedItems.CopyTo(array, arrayIndex);
        }
    }
}


