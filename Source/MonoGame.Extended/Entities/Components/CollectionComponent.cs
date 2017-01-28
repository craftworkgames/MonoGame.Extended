using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MonoGame.Extended.Entities.Components
{
    public abstract class CollectionComponent<T, TEventArgs> : EntityComponent, ICollection<T>
        where TEventArgs : CollectionComponentEventArgs<T>
    {
        private readonly List<T> _collection = new List<T>();

        public event EventHandler<TEventArgs> ItemAdded;
        public event EventHandler<TEventArgs> ItemRemoved;

        /// <summary>
        /// Marked as protected, so child classes can publicly name their collection appropriately.
        /// </summary>
        protected IReadOnlyCollection<T> Collection => new ReadOnlyCollection<T>(_collection);

        public int Count => _collection.Count;
        public bool IsReadOnly => false;

        public void Add(T item)
        {
            _collection.Add(item);
            ItemAdded?.Invoke(this, CreateEventArgs(item));
        }

        public void Clear()
        {
            foreach (var sprite in _collection)
                ItemRemoved?.Invoke(this, CreateEventArgs(sprite));
            _collection.Clear();
        }

        public bool Contains(T item)
        {
            return _collection.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            foreach (var sprite in array)
                ItemAdded?.Invoke(this, CreateEventArgs(sprite));
            _collection.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        public bool Remove(T item)
        {
            bool removed = _collection.Remove(item);
            if (removed)
                ItemRemoved?.Invoke(this, CreateEventArgs(item));
            return removed;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        protected abstract TEventArgs CreateEventArgs(T item);
    }
}
