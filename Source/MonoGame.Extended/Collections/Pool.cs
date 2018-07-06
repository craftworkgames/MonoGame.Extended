using System;

namespace MonoGame.Extended.Collections
{
    public class Pool<T>
        where T : class
    {
        private readonly Func<T> _createItem;
        private readonly Action<T> _resetItem;
        private readonly Deque<T> _freeItems;
        private readonly int _maximum;
        
        public Pool(Func<T> createItem, Action<T> resetItem, int capacity = 16, int maximum = int.MaxValue)
        {
            _createItem = createItem;
            _resetItem = resetItem;
            _maximum = maximum;
            _freeItems = new Deque<T>(capacity);
        }

        public Pool(Func<T> createItem, int capacity = 16, int maximum = int.MaxValue)
            : this(createItem, _ => { }, capacity, maximum)
        {
        }

        public int AvailableCount => _freeItems.Count;

        public T Obtain()
        {
            if (_freeItems.Count > 0)
                return _freeItems.Pop();

            return _createItem();
        }

        public void Free(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (_freeItems.Count < _maximum)
                _freeItems.AddToBack(item);

            _resetItem(item);
        }

        public void Clear()
        {
            _freeItems.Clear();
        }
    }
}