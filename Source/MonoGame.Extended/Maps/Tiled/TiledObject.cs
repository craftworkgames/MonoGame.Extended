using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledObject
    {
        public TiledObject(int id, int x, int y, int width, int height)
        {
            Id = id;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Properties = new TiledProperties();
        }

        public int Id { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public TiledProperties Properties { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}", Id);
        }
    }
}
