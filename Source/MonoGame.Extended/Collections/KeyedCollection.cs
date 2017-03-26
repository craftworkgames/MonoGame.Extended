using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended.Collections
{
    public class KeyedCollection<TKey, TValue> : ICollection<TValue>
    {
        private readonly Func<TValue, TKey> _getKey;
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public KeyedCollection(Func<TValue, TKey> getKey)
        {
            _getKey = getKey;
        }

        public TValue this[TKey key] => _dictionary[key];
        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;
        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        public void Add(TValue item)
        {
            _dictionary.Add(_getKey(item), item);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(TValue item)
        {
            return _dictionary.ContainsKey(_getKey(item));
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(TValue item)
        {
            return _dictionary.Remove(_getKey(item));
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }
    }
}