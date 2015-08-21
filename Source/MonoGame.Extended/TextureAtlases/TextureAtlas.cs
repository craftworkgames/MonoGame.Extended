using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    public class TextureAtlas
    {
        public TextureAtlas(Texture2D texture)
        {
            Texture = texture;
            _regionMap = new Dictionary<string, TextureRegion2D>();
        }

        private readonly Dictionary<string, TextureRegion2D> _regionMap; 

        public Texture2D Texture { get; private set; }

        public IEnumerable<TextureRegion2D> Regions
        {
            get { return _regionMap.Values; }
        }

        public TextureRegion2D CreateRegion(string name, int x, int y, int width, int height)
        {
            if (_regionMap.ContainsKey(name))
                throw new InvalidOperationException("Region {0} already exists in the texture atlas");

            var region = new TextureRegion2D(Texture, x, y, width, height);
            _regionMap.Add(name, region);
            return region;
        }

        public void RemoveRegion(string name)
        {
            _regionMap.Remove(name);
        }

        public TextureRegion2D GetRegion(string name)
        {
            TextureRegion2D region;
            return _regionMap.TryGetValue(name, out region) ? region : null;
        }
    }
}