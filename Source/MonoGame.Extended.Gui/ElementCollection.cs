using System;
using System.Collections;
using System.Collections.Generic;
using MonoGame.Extended.Gui.Controls;

namespace MonoGame.Extended.Gui
{
    public abstract class ElementCollection<TChild, TParent> : IList<TChild>
        where TParent : class, IRectangular
        where TChild : Element<TParent>
    {
        private readonly TParent _parent;
        private readonly List<TChild> _list = new List<TChild>();

        public Action<TChild> ItemAdded { get; set; }
        public Action<TChild> ItemRemoved { get; set; }

        protected ElementCollection(TParent parent)
        {
            _parent = parent;
        }

        public IEnumerator<TChild> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        public void Add(TChild item)
        {
            item.Parent = _parent;
            _list.Add(item);
            ItemAdded?.Invoke(item);
        }

        public void Clear()
        {
            foreach (var child in _list)
            {
                child.Parent = null;
                ItemRemoved?.Invoke(child);
            }

            _list.Clear();
        }

        public bool Contains(TChild item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(TChild[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(TChild item)
        {
            item.Parent = null;
            ItemRemoved?.Invoke(item);
            return _list.Remove(item);
        }

        public int Count => _list.Count;

        public bool IsReadOnly => ((ICollection<Control>)_list).IsReadOnly;

        public int IndexOf(TChild item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, TChild item)
        {
            item.Parent = _parent;
            _list.Insert(index, item);
            ItemAdded?.Invoke(item);
        }

        public void RemoveAt(int index)
        {
            var child = _list[index];
            child.Parent = null;
            _list.RemoveAt(index);
            ItemRemoved?.Invoke(child);
        }

        public TChild this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }
    }
}