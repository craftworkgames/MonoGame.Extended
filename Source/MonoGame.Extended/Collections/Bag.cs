// Original code dervied from:
// https://github.com/thelinuxlich/artemis_CSharp/blob/master/Artemis_XNA_INDEPENDENT/Utils/Bag.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bag.cs" company="GAMADU.COM">
//     Copyright © 2013 GAMADU.COM. All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without modification, are
//     permitted provided that the following conditions are met:
//
//        1. Redistributions of source code must retain the above copyright notice, this list of
//           conditions and the following disclaimer.
//
//        2. Redistributions in binary form must reproduce the above copyright notice, this list
//           of conditions and the following disclaimer in the documentation and/or other materials
//           provided with the distribution.
//
//     THIS SOFTWARE IS PROVIDED BY GAMADU.COM 'AS IS' AND ANY EXPRESS OR IMPLIED
//     WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//     FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL GAMADU.COM OR
//     CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//     CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//     SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
//     ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//     NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
//     ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     The views and conclusions contained in the software and documentation are those of the
//     authors and should not be interpreted as representing official policies, either expressed
//     or implied, of GAMADU.COM.
// </copyright>
// <summary>
//   Class Bag.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended.Collections
{
    public class Bag<T> : IEnumerable<T>
    {
        private T[] _items;

        public int Capacity => _items.Length;
        public bool IsEmpty => Count == 0;
        public int Count { get; private set; }

        public Bag(int capacity = 16)
        {
            _items = new T[capacity];
        }

        public T this[int index]
        {
            get { return _items[index]; }
            set
            {
                EnsureCapacity(index + 1);
                if (index >= Count)
                    Count = index + 1;
                _items[index] = value;
            }
        }

        public void Add(T element)
        {
            EnsureCapacity(Count + 1);
            _items[Count] = element;
            ++Count;
        }

        public void AddRange(Bag<T> range)
        {
            for (int index = 0, j = range.Count; j > index; ++index)
                Add(range[index]);
        }

        public void Clear()
        {
            Array.Clear(_items, 0, Count);
            Count = 0;
        }

        public bool Contains(T element)
        {
            for (var index = Count - 1; index >= 0; --index)
                if (element.Equals(_items[index]))
                    return true;

            return false;
        }

        public T Remove(int index)
        {
            var result = _items[index];
            --Count;
            _items[index] = _items[Count];
            _items[Count] = default(T);
            return result;
        }

        public bool Remove(T element)
        {
            for (var index = Count - 1; index >= 0; --index)
                if (element.Equals(_items[index]))
                {
                    --Count;
                    _items[index] = _items[Count];
                    _items[Count] = default(T);

                    return true;
                }

            return false;
        }

        public bool RemoveAll(Bag<T> bag)
        {
            var isResult = false;
            for (var index = bag.Count - 1; index >= 0; --index)
                if (Remove(bag[index]))
                    isResult = true;
            return isResult;
        }

        private void EnsureCapacity(int capacity)
        {
            if (capacity < _items.Length)
                return;

            var newCapacity = Math.Max((int)(_items.Length * 1.5), capacity);
            var oldElements = _items;
            _items = new T[newCapacity];
            Array.Copy(oldElements, 0, _items, 0, oldElements.Length);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new BagEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new BagEnumerator(this);
        }

        internal struct BagEnumerator : IEnumerator<T>
        {
            private volatile Bag<T> _bag;
            private volatile int _index;

            public BagEnumerator(Bag<T> bag)
            {
                _bag = bag;
                _index = -1;
            }

            T IEnumerator<T>.Current => _bag[_index];
            object IEnumerator.Current => _bag[_index];

            public bool MoveNext()
            {
                return ++_index < _bag.Count;
            }

            public void Dispose()
            {
            }

            public void Reset()
            {
            }
        }
    }
}