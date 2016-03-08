using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    public class TextureAtlas : IEnumerable<TextureRegion2D>
    {
        private readonly Dictionary<string, int> _regionMap;

        private readonly List<TextureRegion2D> _regions;

        public TextureRegion2D this[string name] => GetRegion(name);

        public TextureRegion2D this[int index] => GetRegion(index);

        public int RegionCount => _regions.Count;
        public IEnumerable<TextureRegion2D> Regions => _regions;

        public Texture2D Texture { get; }

        public TextureAtlas(Texture2D texture)
        {
            Texture = texture;
            _regions = new List<TextureRegion2D>();
            _regionMap = new Dictionary<string, int>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TextureRegion2D> GetEnumerator()
        {
            return _regions.GetEnumerator();
        }

        public TextureRegion2D CreateRegion(string name, int x, int y, int width, int height)
        {
            if (_regionMap.ContainsKey(name))
            {
                throw new InvalidOperationException("Region {0} already exists in the texture atlas");
            }

            var region = new TextureRegion2D(name, Texture, x, y, width, height);
            var index = _regions.Count;
            _regions.Add(region);
            _regionMap.Add(name, index);
            return region;
        }

        public void RemoveRegion(int index)
        {
            _regions.RemoveAt(index);
        }

        public void RemoveRegion(string name)
        {
            int index;

            if (_regionMap.TryGetValue(name, out index))
            {
                RemoveRegion(index);
                _regionMap.Remove(name);
            }
        }

        public TextureRegion2D GetRegion(int index)
        {
            if (index < 0 || index >= _regions.Count)
            {
                throw new IndexOutOfRangeException();
            }

            return _regions[index];
        }

        public TextureRegion2D GetRegion(string name)
        {
            int index;

            if (_regionMap.TryGetValue(name, out index))
            {
                return GetRegion(index);
            }

            throw new KeyNotFoundException(name);
        }

        public static TextureAtlas Create(Texture2D texture, int regionWidth, int regionHeight, int maxRegionCount = int.MaxValue, int margin = 0, int spacing = 0)
        {
            var textureAtlas = new TextureAtlas(texture);
            var count = 0;
            var width = texture.Width - margin;
            var height = texture.Height - margin;
            var xIncrement = regionWidth + spacing;
            var yIncrement = regionHeight + spacing;

            for (var y = margin; y < height; y += yIncrement)
            {
                for (var x = margin; x < width; x += xIncrement)
                {
                    var regionName = $"{texture.Name ?? "region"}{count}";
                    textureAtlas.CreateRegion(regionName, x, y, regionWidth, regionHeight);
                    count++;

                    if (count >= maxRegionCount)
                    {
                        return textureAtlas;
                    }
                }
            }

            return textureAtlas;
        }
    }
}