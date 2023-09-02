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
        private readonly bool _isPrimitive;

        public int Capacity => _items.Length;
        public bool IsEmpty => Count == 0;
        public int Count { get; private set; }

        public Bag(int capacity = 16)
        {
            _isPrimitive = typeof(T).IsPrimitive;
            _items = new T[capacity];
        }

        public T this[int index]
        {
            get => index >= _items.Length ? default(T) : _items[index];
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
            if(Count == 0)
                return;

            // non-primitive types are cleared so the garbage collector can release them
            if (!_isPrimitive)
                Array.Clear(_items, 0, Count);

            Count = 0;
        }

        public bool Contains(T element)
        {
            for (var index = Count - 1; index >= 0; --index)
            {
                if (element.Equals(_items[index]))
                    return true;
            }

            return false;
        }

        public T RemoveAt(int index)
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
            {
                if (element.Equals(_items[index]))
                {
                    --Count;
                    _items[index] = _items[Count];
                    _items[Count] = default(T);

                    return true;
                }
            }

            return false;
        }

        public bool RemoveAll(Bag<T> bag)
        {
            var isResult = false;

            for (var index = bag.Count - 1; index >= 0; --index)
            {
                if (Remove(bag[index]))
                    isResult = true;
            }

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

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Get the <see cref="BagEnumerator"/> for this <see cref="Bag{T}"/>. 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Use this method preferentially over <see cref="IEnumerable.GetEnumerator"/> while enumerating via foreach
        /// to avoid boxing the enumerator on every iteration, which can be expensive in high-performance environments.
        /// </remarks>
        public BagEnumerator GetEnumerator()
        {
            return new BagEnumerator(this);
        }

        /// <summary>
        /// Enumerates a Bag.
        /// </summary>
        public struct BagEnumerator : IEnumerator<T>
        {
            private readonly Bag<T> _bag;
            private volatile int _index;

            /// <summary>
            /// Creates a new <see cref="BagEnumerator"/> for this <see cref="Bag{T}"/>.
            /// </summary>
            /// <param name="bag"></param>
            public BagEnumerator(Bag<T> bag)
            {
                _bag = bag;
                _index = -1;
            }

            readonly T IEnumerator<T>.Current => _bag[_index];

            readonly object IEnumerator.Current => _bag[_index];

            /// <summary>
            /// Gets the element in the <see cref="Bag{T}"/> at the current position of the enumerator.
            /// </summary>
            public readonly T Current => _bag[_index];

            /// <inheritdoc/>
            public bool MoveNext()
            {
                return ++_index < _bag.Count;
            }

            /// <inheritdoc/>
            public readonly void Dispose()
            {
            }

            /// <inheritdoc/>
            public readonly void Reset()
            {
            }
        }
    }
}
