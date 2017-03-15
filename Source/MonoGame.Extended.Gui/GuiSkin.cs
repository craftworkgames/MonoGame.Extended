using System;
using System.Collections;
using System.Collections.Generic;
using MonoGame.Extended.BitmapFonts;
using System.Linq;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui
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

    

    public class GuiSkin
    {
        public GuiSkin()
        {
            TextureAtlases = new List<TextureAtlas>();
            Fonts = new List<BitmapFont>();
            NinePatches = new List<NinePatchRegion2D>();
            _styles = new KeyedCollection<string, GuiControlStyle>(s => s.Name);
        }

        private readonly KeyedCollection<string, GuiControlStyle> _styles;

        [JsonProperty(Order = 0)]
        public string Name { get; set; }

        [JsonProperty(Order = 1)]
        public IList<TextureAtlas> TextureAtlases { get; set; }

        [JsonProperty(Order = 2)]
        public IList<BitmapFont> Fonts { get; set; }

        [JsonProperty(Order = 3)]
        public IList<NinePatchRegion2D> NinePatches { get; set; }

        [JsonProperty(Order = 4)]
        public BitmapFont DefaultFont => Fonts.FirstOrDefault();

        [JsonProperty(Order = 5)]
        public GuiCursor Cursor { get; set; }

        [JsonProperty(Order = 6)]
        public ICollection<GuiControlStyle> Styles => _styles;

        public GuiControlStyle GetStyle(string name)
        {
            return _styles[name];
        }
    }
}