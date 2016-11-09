using System;

namespace MonoGame.Extended.Collections
{
    /// <summary>
    ///   Arguments class for collections wanting to hand over an item in an event
    /// </summary>
    public class ItemEventArgs<ItemType> : EventArgs
    {
        /// <summary>Initializes a new event arguments supplier</summary>
        /// <param name="item">Item to be supplied to the event handler</param>
        public ItemEventArgs(ItemType item)
        {
            this.item = item;
        }

        /// <summary>Obtains the collection item the event arguments are carrying</summary>
        public ItemType Item
        {
            get { return this.item; }
        }

        /// <summary>Item to be passed to the event handler</summary>
        private ItemType item;
    }
}
