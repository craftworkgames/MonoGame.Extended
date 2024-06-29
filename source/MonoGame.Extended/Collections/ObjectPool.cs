using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGame.Extended.Collections
{
    public enum ObjectPoolIsFullPolicy
    {
        ReturnNull,
        IncreaseSize,
        KillExisting,
    }

    public class ObjectPool<T> : IEnumerable<T>
        where T : class, IPoolable
    {
        private readonly Action<IPoolable> _returnToPoolDelegate;

        private readonly Deque<T> _freeItems; // circular buffer for O(1) operations
        private T _headNode; // linked list for iteration
        private T _tailNode;

        private readonly Func<T> _instantiationFunction;

        public ObjectPoolIsFullPolicy IsFullPolicy { get; }
        public int Capacity { get; private set; }
        public int TotalCount { get; private set; }
        public int AvailableCount => _freeItems.Count;
        public int InUseCount => TotalCount - AvailableCount;

        public event Action<T> ItemUsed;
        public event Action<T> ItemReturned;

        public ObjectPool(Func<T> instantiationFunc, int capacity = 16, ObjectPoolIsFullPolicy isFullPolicy = ObjectPoolIsFullPolicy.ReturnNull)
        {
            if (instantiationFunc == null)
                throw new ArgumentNullException(nameof(instantiationFunc));

            _returnToPoolDelegate = Return;

            _instantiationFunction = instantiationFunc;
            _freeItems = new Deque<T>(capacity);
            IsFullPolicy = isFullPolicy;

            Capacity = capacity;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var node = _headNode;
            while (node != null)
            {
                yield return node;
                node = (T)node.NextNode;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T New()
        {
            if (!_freeItems.RemoveFromFront(out var poolable))
            {
                if (TotalCount <= Capacity)
                {
                    poolable = CreateObject();
                }
                else
                {
                    switch (IsFullPolicy)
                    {
                        case ObjectPoolIsFullPolicy.ReturnNull:
                            return null;
                        case ObjectPoolIsFullPolicy.IncreaseSize:
                            Capacity++;
                            poolable = CreateObject();
                            break;
                        case ObjectPoolIsFullPolicy.KillExisting:
                            if (_headNode == null)
                                return null;
                            var newHeadNode = (T)_headNode.NextNode;
                            _headNode.Return();
                            _freeItems.RemoveFromBack(out poolable);
                            _headNode = newHeadNode;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            Use(poolable);
            return poolable;
        }

        private T CreateObject()
        {
            TotalCount++;
            var item = _instantiationFunction();
            if (item == null)
                throw new NullReferenceException($"The created pooled object of type '{typeof(T).Name}' is null.");
            item.PreviousNode = _tailNode;
            item.NextNode = null;
            if (_headNode == null)
                _headNode = item;
            if (_tailNode != null)
                _tailNode.NextNode = item;
            _tailNode = item;
            return item;
        }

        private void Return(IPoolable item)
        {
            Debug.Assert(item != null);

            var poolable1 = (T) item;

            var previousNode = (T)item.PreviousNode;
            var nextNode = (T)item.NextNode;

            if (previousNode != null)
                previousNode.NextNode = nextNode;
            if (nextNode != null)
                nextNode.PreviousNode = previousNode;

            if (item == _headNode)
                _headNode = nextNode;
            if (item == _tailNode)
                _tailNode = previousNode;

            if (_tailNode != null)
                _tailNode.NextNode = null;

            _freeItems.AddToBack(poolable1);

            ItemReturned?.Invoke((T)item);
        }

        private void Use(T item)
        {
            item.Initialize(_returnToPoolDelegate);
            item.NextNode = null;
            if(_tailNode is null)
            {
                _headNode = item;
                _tailNode = item;
                item.PreviousNode = null;
            }
            else
            {
                item.PreviousNode = _tailNode;
                _tailNode.NextNode = item;
                _tailNode = item;
            }

            ItemUsed?.Invoke(item);
        }
    }
}
