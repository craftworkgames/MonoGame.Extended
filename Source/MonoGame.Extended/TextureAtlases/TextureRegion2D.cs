using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.TextureAtlases
{
    public class TextureRegion2D
    {
        public string Name { get; }
        public Texture2D Texture { get; protected set; }
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }
        public Size Size => new Size(Width, Height);
        public object Tag { get; set; }
        public Size TextureSize { get; }

        public Rectangle Bounds => new Rectangle(X, Y, Width, Height);

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
            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture));
            }

            Name = name;
            Texture = texture;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            TextureSize = new Size(texture.Width, texture.Height);
        }

        public Vector2 GetTextureCoordinates()
        {
            return new Vector2((X + 0.5f) / TextureSize.Width, (Y + 0.5f) / TextureSize.Height);
        }

        public void GetTextureCoordinates(out Vector2 result)
        {
            result.X = (X + 0.5f) / TextureSize.Width;
            result.Y = (Y + 0.5f) / TextureSize.Height;
        }

        public Vector2 GetTextureCoordinates(Point offset)
        {
            return new Vector2((X + offset.X + 0.5f) / TextureSize.Width, (Y + offset.Y + 0.5f) / TextureSize.Height);
        }

        public void GetTextureCoordinates(Point offset, out Vector2 result)
        {
            result.X = (X + offset.X + 0.5f) / TextureSize.Width;
            result.Y = (Y + offset.Y + 0.5f) / TextureSize.Height;
        }

        public override string ToString()
        {
            return $"{Name ?? string.Empty} {Bounds}";
        }
    }
}
