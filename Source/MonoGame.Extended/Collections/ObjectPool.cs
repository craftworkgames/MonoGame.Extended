﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

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
        private readonly Deque<T> _freeItems; // circular buffer for O(1) operations
        private T _headNode; // linked list for iteration
        private T _tailNode;

        private readonly Func<T> _instantiationFunction;

        public ObjectPoolIsFullPolicy IsFullPolicy { get; }
        public int Capacity { get; private set; }
        public int Count { get; private set; }

        public event Action<T> ItemUsed;
        public event Action<T> ItemReturned;

        public ObjectPool(Func<T> instantiationFunc, int capacity = 16, ObjectPoolIsFullPolicy isFullPolicy = ObjectPoolIsFullPolicy.ReturnNull)
        {
            if (instantiationFunc == null)
                throw new ArgumentNullException(nameof(instantiationFunc));

            _instantiationFunction = instantiationFunc;
            _freeItems = new Deque<T>(capacity);
            IsFullPolicy = isFullPolicy;
            
            for (var i = 0; i < capacity; i++)
            {
                var poolable = CreateObject();
                _freeItems.AddToBack(poolable);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            var node = _headNode;
            while (node != null)
            {
                node = (T)_headNode.NextNode;
                yield return node;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
            Count++;
            var item = _instantiationFunction();
            item.PreviousNode = _tailNode;
            item.NextNode = null;
            if (_tailNode != null)
                _tailNode.NextNode = item;
            _tailNode = item;
            if (item == null)
                throw new NullReferenceException($"The created pooled object of type '{typeof(T).Name}' is null.");
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

            _freeItems.AddToBack(poolable1);

            ItemReturned?.Invoke((T)item);
        }

        private void Use(T item)
        {
            item.Initialize(Return);
            item.NextNode = null;
            item.PreviousNode = _tailNode;
            _tailNode.NextNode = item;
            _tailNode = item;

            ItemUsed?.Invoke(item);
        }
    }
}