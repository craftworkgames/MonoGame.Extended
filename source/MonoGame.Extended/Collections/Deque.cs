using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MonoGame.Extended.Collections
{
    internal static class Deque
    {
        internal static readonly Func<int, int> DefaultResizeFunction = x => x * 2;
    }

    /// <summary>
    ///     Represents a collection of objects which elements can added to or removed either from the front or back; a
    ///     <a href="https://en.wikipedia.org/wiki/Double-ended_queue">double ended queue</a> (deque).
    /// </summary>
    /// <remarks>
    ///     <a href="https://en.wikipedia.org/wiki/Circular_buffer">circular array</a> is used as the internal data
    ///     structure for the <see cref="Deque{T}" />.
    /// </remarks>
    /// <typeparam name="T">The type of the elements in the deque.</typeparam>
    public class Deque<T> : IList<T>
    {
        private const int _defaultCapacity = 4;
        private static readonly T[] _emptyArray = new T[0];
        private int _frontArrayIndex;
        private T[] _items;
        private Func<int, int> _resizeFunction = Deque.DefaultResizeFunction;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Deque{T}" /> class that is empty and has the default initial capacity.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The capacity of a <see cref="Deque{T}" /> is the number of elements that the <see cref="Deque{T}" /> can
        ///         hold. As elements are added to a <see cref="Deque{T}" />, <see cref="Capacity" /> is automatically increased by
        ///         <see cref="ResizeFunction" /> as required by reallocating the internal array.
        ///     </para>
        ///     <para>
        ///         If the size of the collection can be estimated, using the <see cref="Deque{T}(int)" /> constructor and
        ///         specifying the initial capacity eliminates the need to perform a number of resizing operations while adding
        ///         elements to the <see cref="Deque{T}" />.
        ///     </para>
        ///     <para>
        ///         The capacity can be decreased by calling the <see cref="TrimExcess" /> method or by setting the
        ///         <see cref="Capacity" /> property explicitly. Decreasing, or increasing, the capacity reallocates memory and
        ///         copies all the
        ///         elements in the <see cref="Deque{T}" />.
        ///     </para>
        ///     <para>This constructor is an O(1) operation.</para>
        /// </remarks>
        public Deque()
        {
            _items = _emptyArray;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Deque{T}" /> class that contains elements copied from the specified
        ///     collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new deque.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection" /> is null.</exception>
        /// <remarks>
        ///     <para>
        ///         The elements are copied onto the <see cref="Deque{T}" /> in the same order they are read by the enumerator of
        ///         <paramref name="collection" />.
        ///     </para>
        ///     <para>This constructor is an O(n) operation, where n is the number of elements in <paramref name="collection" />.</para>
        /// </remarks>
        public Deque(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            var array = collection as T[] ?? collection.ToArray();
            var count = array.Length;

            if (count == 0)
                _items = _emptyArray;
            else
            {
                _items = new T[count];
                array.CopyTo(_items, 0);
                Count = count;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Deque{T}" /> class that is empty and has the specified initial
        ///     capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new <see cref="Deque{T}" /> can initially store.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity" /> is less than 0.</exception>
        /// <remarks>
        ///     <para>
        ///         The capacity of a <see cref="Deque{T}" /> is the number of elements that the <see cref="Deque{T}" /> can
        ///         hold. As elements are added to a <see cref="Deque{T}" />, <see cref="Capacity" /> is automatically increased by
        ///         <see cref="ResizeFunction" /> as required by reallocating the internal array.
        ///     </para>
        ///     <para>
        ///         If the size of the collection can be estimated, specifying the initial capacity eliminates the need to
        ///         perform a number of resizing operations while adding elements to the <see cref="Deque{T}" />.
        ///     </para>
        ///     <para>
        ///         The capacity can be decreased by calling the <see cref="TrimExcess" /> method or by setting the
        ///         <see cref="Capacity" /> property explicitly. Decreasing, or increasing, the capacity reallocates memory and
        ///         copies all the elements in the <see cref="Deque{T}" />.
        ///     </para>
        ///     <para>This constructor is an O(n) operation, where n is <paramref name="capacity" />.</para>
        /// </remarks>
        public Deque(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity was less than zero.");

            _items = capacity == 0 ? _emptyArray : new T[capacity];
        }

        /// <summary>
        ///     Gets or sets the resize function used to calculate and set <see cref="Capacity" /> when a greater capacity is
        ///     required.
        /// </summary>
        /// <returns>
        ///     The <see cref="Func{T, TResult}" /> used to calculate and set <see cref="Capacity" /> when a greater capacity
        ///     is required.
        /// </returns>
        /// <remarks>
        ///     The default resize function is twice the <see cref="Capacity" />. Setting
        ///     <see cref="ResizeFunction" /> to <c>null</c> will set it back to the default.
        /// </remarks>
        public Func<int, int> ResizeFunction
        {
            get => _resizeFunction;
            set => _resizeFunction = value ?? Deque.DefaultResizeFunction;
        }

        /// <summary>
        ///     Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        /// <returns>The number of elements that the <see cref="Deque{T}" /> can contain before resizing is required.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <see cref="Capacity" /> cannot be set to a value less than <see cref="Count" />.
        /// </exception>
        /// <remarks>
        ///     Changing <see cref="Capacity" /> reallocates memory and copies all the
        ///     elements in the <see cref="Deque{T}" />.
        /// </remarks>
        public int Capacity
        {
            get => _items.Length;
            set
            {
                if (value < Count)
                    throw new ArgumentOutOfRangeException(nameof(value), "capacity was less than the current size.");

                if (value == Capacity)
                    return;

                if (value == 0)
                {
                    _items = _emptyArray;
                    return;
                }

                var newItems = new T[value];
                CopyTo(newItems);

                _frontArrayIndex = 0;
                _items = null;
                _items = newItems;
            }
        }

        /// <summary>
        ///     Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Index was out of range. Must be non-negative and less than <see cref="Count" />.
        /// </exception>
        /// <remarks>
        ///     <para></para>
        ///     <para>
        ///         Use <c>0</c> for the <paramref name="index" /> to get or set the element at the beginning of the
        ///         <see cref="Deque{T}" />, and use <c><see cref="Count" /> - 1</c> for the <paramref name="index" /> to get the
        ///         element at the end of the <see cref="Deque{T}" />.
        ///     </para>
        /// </remarks>
        public T this[int index]
        {
            get
            {
                var arrayIndex = GetArrayIndex(index);
                if (arrayIndex == -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(index),
                        "Index was out of range. Must be non-negative and less than the size of the collection.");
                }
                return _items[arrayIndex];
            }
            set
            {
                var arrayIndex = GetArrayIndex(index);
                if (arrayIndex == -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(index),
                        "Index was out of range. Must be non-negative and less than the size of the collection.");
                }
                _items[arrayIndex] = value;
            }
        }

        /// <summary>
        ///     Gets the number of elements contained in the <see cref="Deque{T}" />.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="Deque{T}" />.</returns>
        public int Count { get; private set; }

        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        ///     Returns an enumerator that iterates through the <see cref="Deque{T}" />.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}" /> that can be used to iterate through the <see cref="Deque{T}" />.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (Count == 0)
                yield break;

            if (Count <= _items.Length - _frontArrayIndex)
            {
                for (var i = _frontArrayIndex; i < _frontArrayIndex + Count; i++)
                    yield return _items[i];
            }
            else
            {
                for (var i = _frontArrayIndex; i < Capacity; i++)
                    yield return _items[i];
                for (var i = 0; i < (_frontArrayIndex + Count) % Capacity; i++)
                    yield return _items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<T>.Add(T item)
        {
            AddToBack(item);
        }

        /// <summary>
        ///     Searches for the specified element and returns the zero-based index of the first occurrence within the entire
        ///     <see cref="Deque{T}" />.
        /// </summary>
        /// <param name="item">
        ///     The element to locate in the <see cref="Deque{T}" />. The value can be <c>null</c> for reference
        ///     types.
        /// </param>
        /// <returns>
        ///     The zero-based index of the first occurrence of <paramref name="item" /> within the entire
        ///     <see cref="Deque{T}" />, if found; otherwise, <c>-1</c>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         This method is an O(1) operation if <paramref name="item" /> is at the front or back of the
        ///         <see cref="Deque{T}" />; otherwise, this method is an O(n) operation where n is <see cref="Count" />.
        ///     </para>
        /// </remarks>
        public int IndexOf(T item)
        {
            var comparer = EqualityComparer<T>.Default;

            if (Get(0, out var checkFrontBackItem) && comparer.Equals(checkFrontBackItem, item))
                return 0;

            var backIndex = Count - 1;

            if (Get(backIndex, out checkFrontBackItem) && comparer.Equals(checkFrontBackItem, item))
                return backIndex;

            int index;

            if (Count <= _items.Length - _frontArrayIndex)
                index = Array.IndexOf(_items, item, _frontArrayIndex, Count);
            else
            {
                index = Array.IndexOf(_items, item, _frontArrayIndex, _items.Length - _frontArrayIndex);
                if (index < 0)
                    index = Array.IndexOf(_items, item, 0, _frontArrayIndex + Count - _items.Length);
            }

            var circularIndex = (index - _frontArrayIndex + _items.Length) % _items.Length;
            return circularIndex;
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Removes the first occurrence of a specific element from the <see cref="Deque{T}" />.
        /// </summary>
        /// <param name="item">
        ///     The element to remove from the <see cref="Deque{T}" />. The value can be <c>null</c> for reference
        ///     types.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="item" /> was successfully removed; otherwise, false. This method also returns false
        ///     if <paramref name="item" /> is not found in the <see cref="Deque{T}" />.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         This method is an O(1) operation if <paramref name="item" /> is at the front or back of the
        ///         <see cref="Deque{T}" />; otherwise, this method is an O(n) operation where n is <see cref="Count" />.
        ///     </para>
        /// </remarks>
        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index == -1)
                return false;

            RemoveAt(index);
            return true;
        }

        /// <summary>
        ///     Removes the element at the specified index of the <see cref="Deque{T}" />.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <para><paramref name="index" /> is less than 0.</para>
        ///     <para>-or-</para>
        ///     <para><paramref name="index" /> is equal to or greater than <see cref="Count" />.</para>
        /// </exception>
        public void RemoveAt(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index was less than zero.");

            if (index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index was equal or greater than TotalCount.");

            if (index == 0)
            {
                RemoveFromFront();
            }
            else
            {
                if (index == Count - 1)
                {
                    RemoveFromBack();
                }
                else
                {
                    if (index < Count / 2)
                    {
                        var arrayIndex = GetArrayIndex(index);
                        // shift the array from 0 to before the index to remove by 1 to the right
                        // the element to remove is replaced by the copy
                        Array.Copy(_items, 0, _items, 1, arrayIndex);
                        // the first element in the arrya is now either a duplicate or it's default value
                        // to be safe set it to it's default value regardless of circumstance
                        _items[0] = default(T);
                        // if we shifted the front element, adjust the front index
                        if (_frontArrayIndex < arrayIndex)
                            _frontArrayIndex = (_frontArrayIndex + 1) % _items.Length;
                        // decrement the count so the back index is calculated correctly
                        Count--;
                    }
                    else
                    {
                        var arrayIndex = GetArrayIndex(index);
                        // shift the array from the center of the array to before the index to remove by 1 to the right
                        // the element to remove is replaced by the copy
                        var arrayCenterIndex = _items.Length / 2;
                        Array.Copy(_items, arrayCenterIndex, _items, arrayCenterIndex + 1, _items.Length - 1 - arrayIndex);
                        // the last element in the array is now either a duplicate or it's default value
                        // to be safe set it to it's default value regardless of circumstance
                        _items[_items.Length - 1] = default(T);
                        // if we shifted the front element, adjust the front index
                        if (_frontArrayIndex < arrayIndex)
                            _frontArrayIndex = (_frontArrayIndex + 1) % _items.Length;
                        // decrement the count so the back index is calculated correctly
                        Count--;
                    }
                }
            }
        }

        /// <summary>
        ///     Removes all elements from the <see cref="Deque{T}" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         <see cref="Count" /> is set to <c>0</c>, and references to other objects from elements of the collection are
        ///         also released.
        ///     </para>
        ///     <para>
        ///         <see cref="Capacity" /> remains unchanged. To reset the capacity of the <see cref="Deque{T}" />, call the
        ///         <see cref="TrimExcess" /> method or set the <see cref="Capacity" /> property explictly. Decreasing, or
        ///         increasing, the capacity reallocates memory and copies all the elements in the <see cref="Deque{T}" />.
        ///         Trimming an empty <see cref="Deque{T}" /> sets <see cref="Capacity" /> to the default capacity.
        ///     </para>
        ///     <para>This method is an O(n) operation, where n is <see cref="Count" />.</para>
        /// </remarks>
        public void Clear()
        {
            // allow the garbage collector to reclaim the references

            if (Count == 0)
                return;

            if (Count > _items.Length - _frontArrayIndex)
            {
                Array.Clear(_items, _frontArrayIndex, _items.Length - _frontArrayIndex);
                Array.Clear(_items, 0, _frontArrayIndex + Count - _items.Length);
            }
            else
                Array.Clear(_items, _frontArrayIndex, Count);
            Count = 0;
            _frontArrayIndex = 0;
        }

        /// <summary>
        ///     Determines whether an element is in the <see cref="Deque{T}" />.
        /// </summary>
        /// <param name="item">
        ///     The element to locate in the <see cref="Deque{T}" />. The value can be <c>null</c> for reference
        ///     types.
        /// </param>
        /// <returns><c>true</c> if <paramref name="item" /> is found in the <see cref="Deque{T}" />; otherwise, false.</returns>
        /// <remarks>
        ///     <para>
        ///         This method determines equality by using the default equality comparer, as defined by the object's
        ///         implementation
        ///         of the <see cref="IEquatable{T}.Equals(T)" /> method for the type of values in the list.
        ///     </para>
        ///     <para>
        ///         This method performs a linear search; therefore, this method is an O(n) operation, where n is
        ///         <see cref="Count" />.
        ///     </para>
        /// </remarks>
        public bool Contains(T item)
        {
            return this.Contains(item, EqualityComparer<T>.Default);
        }

        /// <summary>
        ///     Copies the entire <see cref="Deque{T}" /> to a compatible one-dimensional array, starting at the specified index of
        ///     the target array.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="Array" /> that is the destination of the elements copied from
        ///     <see cref="Deque{T}" />. The <see cref="Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array" /> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="ArgumentException">
        ///     The number of elements in the source <see cref="Deque{T}" /> is greater than the
        ///     available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.
        /// </exception>
        /// <remarks>
        ///     This method uses <see cref="Array.Copy(Array, int, Array, int, int)" /> to copy the elements. The elements are
        ///     copied to the <see cref="Array" /> in the same order in which the enumerator iterates
        ///     through the <see cref="Deque{T}" />. This method is an O(n) operation, where n is <see cref="Count" />.
        /// </remarks>
        public void CopyTo(T[] array, int arrayIndex = 0)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (array.Rank != 1)
                throw new ArgumentException("Only single dimensional arrays are supported for the requested action.");

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Index was less than the array's lower bound.");

            if (arrayIndex >= array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex),
                    "Index was greater than the array's upper bound.");
            }

            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("Destination array was not long enough.");

            if (Count == 0)
                return;


            try
            {
                var loopsAround = Count > _items.Length - _frontArrayIndex;
                if (!loopsAround)
                    Array.Copy(_items, _frontArrayIndex, array, arrayIndex, Count);
                else
                {
                    Array.Copy(_items, _frontArrayIndex, array, arrayIndex, Capacity - _frontArrayIndex);
                    Array.Copy(_items, 0, array, arrayIndex + Capacity - _frontArrayIndex,
                        _frontArrayIndex + (Count - Capacity));
                }
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException(
                    "Target array type is not compatible with the type of items in the collection.");
            }
        }

        /// <summary>
        ///     Sets the capacity to the actual number of elements in the <see cref="Deque{T}" />, if that number is less than a
        ///     threshold value.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method can be used to minimize the <see cref="Deque{T}" />'s memory overhead if no new elements will be
        ///         added. The cost of reallocating and copying the elements of a <see cref="Deque{T}" /> can be considerable.
        ///         However, the <see cref="TrimExcess" /> method does nothing if the <see cref="Count" /> is more than 90% of
        ///         <see cref="Capacity" />. This avoids incurring a large reallocation cost for a relatively small gain.
        ///     </para>
        ///     <para>
        ///         If <see cref="Count" /> is more than 90% of <see cref="Capacity" />, this method is an O(1) operation; O(n)
        ///         otherwise, where n is <see cref="Count" />.
        ///     </para>
        ///     <para>
        ///         To reset a <see cref="Deque{T}" /> to its initial state, call the <see cref="Clear" /> method before calling
        ///         the <see cref="TrimExcess" /> method. Trimming an empty <see cref="Deque{T}" /> sets <see cref="Capacity" /> to
        ///         the default capacity.
        ///     </para>
        ///     <para>The capacity can also be set using the <see cref="Capacity" /> property.</para>
        /// </remarks>
        public void TrimExcess()
        {
            if (Count > (int)(_items.Length * 0.9))
                return;
            Capacity = Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetArrayIndex(int index)
        {
            if ((index < 0) || (index >= Count))
                return -1;
            return _items.Length != 0 ? (_frontArrayIndex + index) % _items.Length : 0;
        }

        private void EnsureCapacity(int minimumCapacity)
        {
            if (_items.Length >= minimumCapacity)
                return;
            var newCapacity = _defaultCapacity;
            if (_items.Length > 0)
                newCapacity = _resizeFunction(_items.Length);
            newCapacity = Math.Max(newCapacity, minimumCapacity);
            Capacity = newCapacity;
        }

        /// <summary>
        ///     Adds an element to the beginning of the <see cref="Deque{T}" />.
        /// </summary>
        /// <param name="item">The element to add to the <see cref="Deque{T}" />. The value can be <c>null</c>.</param>
        /// <remarks>
        ///     <para>
        ///         As elements are added to a <see cref="Deque{T}" />, <see cref="Capacity" /> is automatically increased by
        ///         <see cref="ResizeFunction" /> as required by reallocating the internal circular array.
        ///     </para>
        ///     <para>
        ///         If <see cref="Count" /> is less than <see cref="Capacity" />, this method is an O(1) operation. Otherwise the
        ///         internal circular array needs to be resized to accommodate the new element and this method becomes an O(n)
        ///         operation, where n is <see cref="Count" />.
        ///     </para>
        /// </remarks>
        public void AddToFront(T item)
        {
            EnsureCapacity(Count + 1);
            _frontArrayIndex = (_frontArrayIndex - 1 + _items.Length) % _items.Length;
            _items[_frontArrayIndex] = item;
            Count++;
        }

        /// <summary>
        ///     Adds an element to the end of the <see cref="Deque{T}" />.
        /// </summary>
        /// <param name="item">The element to add to the <see cref="Deque{T}" />. The value can be <c>null</c>.</param>
        /// <remarks>
        ///     <para>
        ///         As elements are added to a <see cref="Deque{T}" />, <see cref="Capacity" /> is automatically increased by
        ///         <see cref="ResizeFunction" /> as required by reallocating the internal circular array.
        ///     </para>
        ///     <para>
        ///         If <see cref="Count" /> is less than <see cref="Capacity" />, this method is an O(1) operation. Otherwise the
        ///         internal circular array needs to be resized to accommodate the new element and this method becomes an O(n)
        ///         operation, where n is <see cref="Count" />.
        ///     </para>
        /// </remarks>
        public void AddToBack(T item)
        {
            EnsureCapacity(Count + 1);
            var index = (_frontArrayIndex + Count++) % _items.Length;
            _items[index] = item;
        }

        /// <summary>
        ///     Returns the element at the specified index of the <see cref="Deque{T}" />.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="item">
        ///     When this method returns, contains the element at the specified index of the
        ///     <see cref="Deque{T}" />, if <paramref name="index" /> was non-negative and less than <see cref="Count" />;
        ///     otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="item" /> was successfully retrieved at <paramref name="index" /> from the of the
        ///     <see cref="Deque{T}" />;
        ///     otherwise, <c>false</c> if <paramref name="index" /> was non-negative and less than <see cref="Count" />.
        /// </returns>
        public bool Get(int index, out T item)
        {
            var arrayIndex = GetArrayIndex(index);
            if (arrayIndex == -1)
            {
                item = default(T);
                return false;
            }
            item = _items[arrayIndex];
            return true;
        }

        /// <summary>
        ///     Returns the element at the beginning of the <see cref="Deque{T}" />.
        /// </summary>
        /// <param name="item">
        ///     When this method returns, contains the element at the beginning of the <see cref="Deque{T}" />, if
        ///     <see cref="Deque{T}" /> was not empty; otherwise, the default value for the type of the value parameter. This
        ///     parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="item" /> was successfully from the beginning of the <see cref="Deque{T}" />;
        ///     otherwise, <c>false</c> if the <see cref="Deque{T}" /> is empty.
        /// </returns>
        public bool GetFront(out T item)
        {
            return Get(0, out item);
        }

        /// <summary>
        ///     Returns the element at the end of the <see cref="Deque{T}" />.
        /// </summary>
        /// <param name="item">
        ///     When this method returns, contains the element at the end of the <see cref="Deque{T}" />, if
        ///     <see cref="Deque{T}" /> was not empty; otherwise, the default value for the type of the value parameter. This
        ///     parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="item" /> was successfully from the end of the <see cref="Deque{T}" />;
        ///     otherwise, <c>false</c> if the <see cref="Deque{T}" /> is empty.
        /// </returns>
        public bool GetBack(out T item)
        {
            return Get(Count - 1, out item);
        }

        /// <summary>
        ///     Removes the element at the beginning of the <see cref="Deque{T}" />.
        /// </summary>
        /// <param name="item">
        ///     When this method returns, contains the element removed from the beginning of the
        ///     <see cref="Deque{T}" />, if the <see cref="Deque{T}" /> was not empty; otherwise, the default value for the type of
        ///     the value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="item" /> was successfully removed from the beginning of the <see cref="Deque{T}" />;
        ///     otherwise, <c>false</c> if the <see cref="Deque{T}" /> is empty.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         This method is similar to the <see cref="GetFront" /> method, but <see cref="GetFront" /> does not
        ///         modify the <see cref="Deque{T}" />.
        ///     </para>
        ///     <para>
        ///         <c>null</c> can be added to the <see cref="Deque{T}" /> as a value. To distinguish between a null value and
        ///         the end of the <see cref="Deque{T}" />, check whether the return value of <see cref="RemoveFromFront(out T)" />
        ///         is
        ///         <c>false</c> or
        ///         <see cref="Count" /> is <c>0</c>.
        ///     </para>
        ///     <para>
        ///         This method is an O(1) operation.
        ///     </para>
        /// </remarks>
        public bool RemoveFromFront(out T item)
        {
            if (Count == 0)
            {
                item = default(T);
                return false;
            }

            var index = _frontArrayIndex % _items.Length;
            item = _items[index];
            _items[index] = default(T);
            _frontArrayIndex = (_frontArrayIndex + 1) % _items.Length;
            Count--;
            return true;
        }

        /// <summary>
        ///     Removes the element at the beginning of the <see cref="Deque{T}" />.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the element was successfully removed from the beginning of the <see cref="Deque{T}" />;
        ///     otherwise, <c>false</c> if the <see cref="Deque{T}" /> is empty.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         This method is similar to the <see cref="GetFront" /> method, but <see cref="GetFront" /> does not
        ///         modify the <see cref="Deque{T}" />.
        ///     </para>
        ///     <para>
        ///         <c>null</c> can be added to the <see cref="Deque{T}" /> as a value. To distinguish between a null value and
        ///         the end of the <see cref="Deque{T}" />, check whether the return value of <see cref="RemoveFromFront()" /> is
        ///         <c>false</c> or
        ///         <see cref="Count" /> is <c>0</c>.
        ///     </para>
        ///     <para>
        ///         This method is an O(1) operation.
        ///     </para>
        /// </remarks>
        public bool RemoveFromFront()
        {
            if (Count == 0)
                return false;

            var index = _frontArrayIndex % _items.Length;
            _items[index] = default(T);
            _frontArrayIndex = (_frontArrayIndex + 1) % _items.Length;
            Count--;
            return true;
        }

        /// <summary>
        ///     Removes the element at the end of the <see cref="Deque{T}" />.
        /// </summary>
        /// <param name="item">
        ///     When this method returns, contains the element removed from the end of the
        ///     <see cref="Deque{T}" />, if the <see cref="Deque{T}" /> was not empty; otherwise, the default value for the type of
        ///     the value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="item" /> was successfully removed from the end of the <see cref="Deque{T}" />;
        ///     otherwise, <c>false</c> if the <see cref="Deque{T}" /> is empty.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         This method is similar to the <see cref="GetBack" /> method, but <see cref="GetBack" /> does not
        ///         modify the <see cref="Deque{T}" />.
        ///     </para>
        ///     <para>
        ///         <c>null</c> can be added to the <see cref="Deque{T}" /> as a value. To distinguish between a null value and
        ///         the end of the <see cref="Deque{T}" />, check whether the return value of <see cref="RemoveFromBack(out T)" />
        ///         is
        ///         <c>false</c> or
        ///         <see cref="Count" /> is <c>0</c>.
        ///     </para>
        ///     <para>
        ///         This method is an O(1) operation.
        ///     </para>
        /// </remarks>
        public bool RemoveFromBack(out T item)
        {
            if (Count == 0)
            {
                item = default(T);
                return false;
            }

            var circularBackIndex = (_frontArrayIndex + (Count - 1)) % _items.Length;
            item = _items[circularBackIndex];
            _items[circularBackIndex] = default(T);
            Count--;
            return true;
        }

        /// <summary>
        ///     Removes the element at the end of the <see cref="Deque{T}" />.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the element was successfully removed from the end of the <see cref="Deque{T}" />;
        ///     otherwise, <c>false</c> if the <see cref="Deque{T}" /> is empty.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         This method is similar to the <see cref="GetBack" /> method, but <see cref="GetBack" /> does not
        ///         modify the <see cref="Deque{T}" />.
        ///     </para>
        ///     <para>
        ///         <c>null</c> can be added to the <see cref="Deque{T}" /> as a value. To distinguish between a null value and
        ///         the end of the <see cref="Deque{T}" />, check whether the return value of <see cref="RemoveFromBack()" /> is
        ///         <c>false</c> or
        ///         <see cref="Count" /> is <c>0</c>.
        ///     </para>
        ///     <para>
        ///         This method is an O(1) operation.
        ///     </para>
        /// </remarks>
        public bool RemoveFromBack()
        {
            if (Count == 0)
                return false;

            var circularBackIndex = (_frontArrayIndex + (Count - 1)) % _items.Length;
            _items[circularBackIndex] = default(T);
            Count--;
            return true;
        }

        /// <summary>
        /// Removes and returns the last item.
        /// </summary>
        /// <returns>The item that was removed</returns>
        public T Pop()
        {
            if (RemoveFromBack(out var item))
                return item;

            throw new InvalidOperationException();
        }
    }
}