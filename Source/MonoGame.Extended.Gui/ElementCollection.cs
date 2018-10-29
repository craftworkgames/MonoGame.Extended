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
            if (item != null)
            {
                item.Parent = _parent;
                _list.Add(item);
                OnItemAdded(item);
            }
        }

        public void Add(TChild[] items)
        {
            foreach (TChild item in items)
            {
                if (item != null)
                {
                    item.Parent = _parent;
                    _list.Add(item);
                    OnItemAdded(item);
                }
            }
        }

        public void Clear()
        {
            foreach (var child in _list)
            {
                child.Parent = null;
                OnItemRemoved(child);
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
            var result = _list.Remove(item);
            OnItemRemoved(item);
            return result;
        }

        public int Count => _list.Count;

        public bool IsReadOnly => ((ICollection<Control>)_list).IsReadOnly;

        public int IndexOf(TChild item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, TChild item)
        {
            if (item != null)
            {
                item.Parent = _parent;
                _list.Insert(index, item);
                OnItemAdded(item);
            }
        }

        public void RemoveAt(int index)
        {
            var child = _list[index];
            child.Parent = null;
            _list.RemoveAt(index);
            OnItemRemoved(child);
        }

        public TChild this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        public virtual void OnItemAdded(TChild child)
        {
            ItemAdded?.Invoke(child);
        }

        public virtual void OnItemRemoved(TChild child)
        {
            ItemRemoved?.Invoke(child);
        }
    }
}