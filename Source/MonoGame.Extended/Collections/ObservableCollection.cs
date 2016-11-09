using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MonoGame.Extended.Collections
{
    public class ObservableCollection<T> : Collection<T>, IObservableCollection<T>
    {
        /// <summary>
        ///     Initializes a new instance of the ObservableCollection class that is empty.
        /// </summary>
        public ObservableCollection()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the ObservableCollection class as a wrapper
        ///     for the specified list.
        /// </summary>
        /// <param name="list">The list that is wrapped by the new collection.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     List is null.
        /// </exception>
        public ObservableCollection(IList<T> list) : base(list)
        {
        }

        /// <summary>Raised when an item has been added to the collection</summary>
        public event EventHandler<ItemEventArgs<T>> ItemAdded;

        /// <summary>Raised when an item is removed from the collection</summary>
        public event EventHandler<ItemEventArgs<T>> ItemRemoved;

        /// <summary>Raised when the collection is about to be cleared</summary>
        /// <remarks>
        ///     This could be covered by calling ItemRemoved for each item currently
        ///     contained in the collection, but it is often simpler and more efficient
        ///     to process the clearing of the entire collection as a special operation.
        /// </remarks>
        public event EventHandler Clearing;

        /// <summary>Raised when the collection has been cleared</summary>
        public event EventHandler Cleared;

        /// <summary>Removes all elements from the Collection</summary>
        protected override void ClearItems()
        {
            OnClearing();
            base.ClearItems();
            OnCleared();
        }

        /// <summary>
        ///     Inserts an element into the ObservableCollection at the specified index
        /// </summary>
        /// <param name="index">
        ///     The object to insert. The value can be null for reference types.
        /// </param>
        /// <param name="item">The zero-based index at which item should be inserted</param>
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            OnAdded(item);
        }

        /// <summary>
        ///     Removes the element at the specified index of the ObservableCollection
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove</param>
        protected override void RemoveItem(int index)
        {
            var item = base[index];
            base.RemoveItem(index);
            OnRemoved(item);
        }

        /// <summary>Replaces the element at the specified index</summary>
        /// <param name="index">
        ///     The new value for the element at the specified index. The value can be null
        ///     for reference types
        /// </param>
        /// <param name="item">The zero-based index of the element to replace</param>
        protected override void SetItem(int index, T item)
        {
            var oldItem = base[index];
            base.SetItem(index, item);
            OnRemoved(oldItem);
            OnAdded(item);
        }

        /// <summary>Fires the 'ItemAdded' event</summary>
        /// <param name="item">Item that has been added to the collection</param>
        protected virtual void OnAdded(T item)
        {
            ItemAdded?.Invoke(this, new ItemEventArgs<T>(item));
        }

        /// <summary>Fires the 'ItemRemoved' event</summary>
        /// <param name="item">Item that has been removed from the collection</param>
        protected virtual void OnRemoved(T item)
        {
            ItemRemoved?.Invoke(this, new ItemEventArgs<T>(item));
        }

        /// <summary>Fires the 'Clearing' event</summary>
        protected virtual void OnClearing()
        {
            Clearing?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Fires the 'Cleared' event</summary>
        protected virtual void OnCleared()
        {
            Cleared?.Invoke(this, EventArgs.Empty);
        }
    }
}