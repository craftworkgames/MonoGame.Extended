using System;

namespace MonoGame.Extended.Collections
{
    /// <summary>
    ///     Arguments class for collections wanting to hand over an item in an event
    /// </summary>
    public class ItemEventArgs<T> : EventArgs
    {
        /// <summary>Initializes a new event arguments supplier</summary>
        /// <param name="item">Item to be supplied to the event handler</param>
        public ItemEventArgs(T item)
        {
            Item = item;
        }

        /// <summary>Obtains the collection item the event arguments are carrying</summary>
        public T Item { get; }
    }
}