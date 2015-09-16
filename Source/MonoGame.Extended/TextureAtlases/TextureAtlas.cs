using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace MonoGame.Extended.TextureAtlases
{
    public class TextureAtlas : IEnumerable<TextureRegion2D>
    {
        public static TextureAtlas Create(Texture2D texture, Vector2 startpoint, int framewidth, int frameheight, int rows, int cols, int rowborder, int colborder, int frameCount, bool lefttop)
        {
            var ret = new TextureAtlas(texture);
            int regid = 0;

            if (lefttop)
            {
                for (int x = (int)startpoint.X; x < startpoint.X + framewidth * cols; x += framewidth + colborder)
                {
                    for (int y = (int)startpoint.Y; y < startpoint.Y + frameheight * rows; y += frameheight + rowborder)
                    {
                        ret.CreateRegion(regid.ToString(), x, y, framewidth, frameheight);
                        regid++;

                        if (regid == frameCount)
                            return ret;
                    }
                }
            }
            else
            {
                for (int y = (int)startpoint.Y; y < startpoint.Y + frameheight * rows; y += frameheight + rowborder)
                {
                    for (int x = (int)startpoint.X; x < startpoint.X + framewidth * cols; x += framewidth + colborder)
                    {
                        ret.CreateRegion(regid.ToString(), x, y, framewidth, frameheight);
                        regid++;

                        if (regid == frameCount)
                            return ret;
                    }
                }
            }

            return ret;
        }

        public TextureAtlas(Texture2D texture)
        {
            Texture = texture;
            _regions = new List<TextureRegion2D>();
            _regionMap = new Dictionary<string, int>();
        }
        
        private readonly Dictionary<string, int> _regionMap; 

        public Texture2D Texture { get; private set; }

        internal readonly List<TextureRegion2D> _regions;
        public IEnumerable<TextureRegion2D> Regions
        {
            get { return _regions; }
        }

        public int RegionCount
        {
            get { return _regions.Count; }
        }

        public TextureRegion2D CreateRegion(string name, int x, int y, int width, int height)
        {
            if (_regionMap.ContainsKey(name))
                throw new InvalidOperationException("Region {0} already exists in the texture atlas");

            var region = new TextureRegion2D(Texture, x, y, width, height);
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
                throw new IndexOutOfRangeException();

            return _regions[index];
        }

        public TextureRegion2D GetRegion(string name)
        {
            int index;

            if (_regionMap.TryGetValue(name, out index))
                return GetRegion(index);

            throw new KeyNotFoundException(name);
        }

        public TextureRegion2D this[string name]
        {
            get { return GetRegion(name); }
        }

        public TextureRegion2D this[int index]
        {
            get { return GetRegion(index); }
        }

        public IEnumerator<TextureRegion2D> GetEnumerator()
        {
            return _regions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}