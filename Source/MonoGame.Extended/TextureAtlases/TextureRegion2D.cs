using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    public class TextureRegion2D
    {
        public TextureRegion2D(Texture2D texture, int x, int y, int width, int height)
        {
            if (texture == null) throw new ArgumentNullException("texture");

            Texture = texture;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public TextureRegion2D(Texture2D texture)
            : this(texture, 0, 0, texture.Width, texture.Height)
        {
        }

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
    }
}
