using System;

namespace MonoGame.Extended.Collections
{
    /// <summary>Interface for collections that can be observed</summary>
    /// <typeparam name="T">Type of items managed in the collection</typeparam>
    public interface IObservableCollection<T>
    {
        /// <summary>Raised when an item has been added to the collection</summary>
        event EventHandler<ItemEventArgs<T>> ItemAdded;

        /// <summary>Raised when an item is removed from the collection</summary>
        event EventHandler<ItemEventArgs<T>> ItemRemoved;

        /// <summary>Raised when the collection is about to be cleared</summary>
        /// <remarks>
        ///     This could be covered by calling ItemRemoved for each item currently
        ///     contained in the collection, but it is often simpler and more efficient
        ///     to process the clearing of the entire collection as a special operation.
        /// </remarks>
        event EventHandler Clearing;

        /// <summary>Raised when the collection has been cleared of its items</summary>
        event EventHandler Cleared;
    }
}