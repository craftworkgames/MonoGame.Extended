using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended.Collections
{
    /// <summary>
    ///     Represents a collection of objects which elements are instantiated ahead of time and re-used by clearing their
    ///     state.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the pool.</typeparam>
    /// <remarks>
    ///     <para>
    ///         Requesting a free element from the <see cref="Pool{T}" /> is a O(1) operation. Returning a used element back to
    ///         the <see cref="Pool{T}" /> is an O(1) operation for the oldest or newest elements or O(n) otherwise, where n is
    ///         the <see cref="Count" />.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     The following example demonstrates how to request and return a simple object from and to a
    ///     <see cref="Pool{T}" />.
    ///     <code>
    /// <![CDATA[ 
    /// // Create the pool
    /// var pool = new Pool<MyPoolable>();
    /// ...
    /// // Get an object from the pool
    /// var myPoolableInstance = pool.Request();
    /// ...
    /// // Return the object back to the pool
    /// myPoolableInstance.Return();
    /// ...
    /// private class MyPoolable : IPoolable
    /// {
    ///     // The return delegate responsible for invoking the internal method in the pool class
    ///     private ReturnToPoolDelegate _returnFunction;
    ///   
    ///     // Called by the Pool when this object is requested for use
    ///     public void Initialize(ReturnToPoolDelegate returnFunction)
    ///     {
    ///         // Copy the reference of the delegate instance so we can use it later
    ///         _returnFunction = returnFunction;
    /// 
    ///         // You could also reset the state of the object instance here instead of when returned
    ///         // Reset()
    ///     }
    /// 
    ///     // Helper method for this instance that should reset the state in preperation for the next time it is requested
    ///     public void Reset()
    ///     {
    ///         // TODO: Reset the state of your object here
    ///     }
    /// 
    ///     // Called by you or the Pool when this object is to be returned
    ///     public void Return()
    ///     {
    ///         // Check if we already used the return delegate
    ///         if (_returnFunction == null)
    ///         {
    ///             // We already used the return delegate; exit this method early
    ///             return;
    ///         }
    /// 
    ///         // Call the reset helper method before we return this instance back to the pool
    ///         Reset();
    /// 
    ///         // We didn't use the return delegate yet; use it now
    ///         _returnFunction.Invoke(this);
    ///         // Set the delegate instance reference to null so we know we can't use it again
    ///         _returnFunction = null;
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public class Pool<T> : ICollection<T>
        where T : class, IPoolable
    {
        // since we are constraining T to classes, these two deqeues are just really two object reference arrays
        // on a 32-bit machine, a reference is 4 bytes
        // on a 64-bit machine, a reference is 8 bytes
        private readonly Deque<T> _freeItems;
        private readonly Func<int, T> _instantiationFunction;
        private readonly Deque<T> _usedItems;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Pool{T}" /> class that has a specified capacity and an element
        ///     instantiating delegate.
        /// </summary>
        /// <param name="capacity">The number of elements that the new <see cref="Pool{T}" /> can store.</param>
        /// <param name="instantiationFunction">The delegate responsible for instantiating elements.</param>
        public Pool(int capacity, Func<int, T> instantiationFunction)
        {
            if (instantiationFunction == null)
                throw new ArgumentNullException(nameof(instantiationFunction));

            _instantiationFunction = instantiationFunction;
            _freeItems = new Deque<T>(capacity);
            _usedItems = new Deque<T>(capacity);
            for (var i = 0; i < capacity; i++)
            {
                var poolable = CreateObject(i);
                _freeItems.AddToBack(poolable);
            }
        }

        /// <summary>
        ///     Gets or sets the total number of elements the internal data structure can hold.
        /// </summary>
        /// <returns>The number of elements that the <see cref="Pool{T}" /> can contain.</returns>
        /// <remarks>
        ///     Once set, <see cref="Capacity" /> can not be changed.
        /// </remarks>
        public int Capacity => _usedItems.Count;

        /// <summary>
        ///     Gets the number of elements contained in the <see cref="Pool{T}" />.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="Pool{T}" />.</returns>
        public int Count => _usedItems.Count;

        bool ICollection<T>.IsReadOnly => true;

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

        /// <summary>
        ///     Returns all in use elements back to the <see cref="Pool{T}" />.
        /// </summary>
        /// <remarks>
        ///     <para><see cref="Count" /> is set to 0.</para>
        ///     <para><see cref="Capacity" /> remains unchanged.</para>
        ///     <para>
        ///         The order which elements are returned is the same as the order they were requested using
        ///         <see cref="Request" />.
        ///     </para>
        ///     <para>This method is an O(n) operation where n is <see cref="Count" />.</para>
        /// </remarks>
        public void Clear()
        {
            while (_usedItems.Count > 0)
            {
                T item;
                _usedItems.GetFront(out item);
                item.Return();
            }
        }

        bool ICollection<T>.Contains(T item)
        {
            return _usedItems.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            _usedItems.CopyTo(array, arrayIndex);
        }

        private T CreateObject(int index)
        {
            var newObject = _instantiationFunction(index);
            if (newObject == null)
                throw new NullReferenceException($"The created pooled object of type '{typeof(T).Name}' is null.");
            return newObject;
        }

        /// <summary>
        ///     Get a free element from the <see cref="Pool{T}" />.
        /// </summary>
        /// <param name="killExistingObjectIfFull">
        ///     <c>true</c> to forcibly kill an existing, in use, element in the <see cref="Pool{T}" /> if <see cref="Count" /> is
        ///     equal to <see cref="Capacity" />; otherwise, <c>false</c>.
        /// </param>
        /// <returns>A free <see cref="T" /> element from the <see cref="Pool{T}" />.</returns>
        /// <remarks>
        ///     <para>This method is an O(1) operation.</para>
        /// </remarks>
        public T Request(bool killExistingObjectIfFull = false)
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

                if (!killExistingObjectIfFull)
                    return null;
                if (!_usedItems.GetFront(out poolable))
                    return null;

                poolable.Return();
                killExistingObjectIfFull = false;
            }
        }

        private void Return(IPoolable poolable)
        {
            var poolable1 = (T) poolable;
            _freeItems.AddToBack(poolable1);
            _usedItems.Remove(poolable1);
        }
    }
}