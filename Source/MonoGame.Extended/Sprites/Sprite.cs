using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoGame.Extended.Sprites
{
    public class Sprite
    {
        public TextureRegion2D TextureRegion;
        public Color Color { get; set; }
        public bool IsVisible { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }

        public Sprite(Texture2D texture)
        {
            TextureRegion = new TextureRegion2D(texture);
            Scale = Vector2.One;
            IsVisible = true;
        }

        public Sprite(Texture2D texture, Vector2 position)
            : this(texture)
        {
            Position = position;
        }

        public Sprite(Texture2D texture, Vector2 position, Color color)
            : this(texture, position)
        {
            Color = color;
        }

        public Sprite(Texture2D texture, Vector2 position, Color? color, float? rotation, Vector2? scale, bool isVisible)
            : this(texture, position)
        {
            // If any parameters are null, provide them with suitable defaults.
            Color = color ?? Color.White;
            Rotation = rotation ?? 0;
            Scale = scale ?? Vector2.One;
            IsVisible = isVisible;
        }

        public Sprite(TextureRegion2D texture)
        {
            TextureRegion = texture;
            Scale = Vector2.One;
            IsVisible = true;
        }

        public Sprite(TextureRegion2D texture, Vector2 position)
            : this(texture)
        {
            Position = position;
        }

        public Sprite(TextureRegion2D texture, Vector2 position, Color color)
            : this(texture, position)
        {
            Color = color;
        }

        public Sprite(TextureRegion2D texture, Vector2 position, Color? color, float? rotation, Vector2? scale, bool isVisible)
            : this(texture, position)
        {
            // If any parameters are null, provide them with suitable defaults.
            Color = color ?? Color.White;
            Rotation = rotation ?? 0;
            Scale = scale ?? Vector2.One;
            IsVisible = isVisible;
        }

        public Rectangle GetBoundingRectangle()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, TextureRegion.Bounds.Width, TextureRegion.Bounds.Height);
        }
    }
}
