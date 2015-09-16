using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    public class TextureRegion2D
    {
        public TextureRegion2D(Texture2D texture, int x, int y, int width, int height)
            : this(null, texture, x, y, width, height)
        {
        }

        public TextureRegion2D(Texture2D texture)
            : this(texture.Name, texture, 0, 0, texture.Width, texture.Height)
        {
        }

        public TextureRegion2D(string name, Texture2D texture, int x, int y, int width, int height)
        {
            if (texture == null) throw new ArgumentNullException("texture");

            Name = name;
            Texture = texture;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public string Name { get; private set; }
        public Texture2D Texture { get; protected set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public object Tag { get; set; }

        public Rectangle Bounds
        {
            get { return new Rectangle(X, Y, Width, Height); }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name ?? string.Empty, Bounds);
        }
    }
}
