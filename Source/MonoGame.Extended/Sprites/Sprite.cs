using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Sprites
{
    public class Sprite
    {
        public Sprite(TextureRegion2D textureRegion)
        {
            if (textureRegion == null) throw new ArgumentNullException("textureRegion");

            TextureRegion = textureRegion;
            Color = Color.White;
            IsVisible = true;
            Scale = Vector2.One;
            Effect = SpriteEffects.None;
            OriginNormalized = new Vector2(0.5f, 0.5f);
        }

        public Sprite(Texture2D texture)
            : this(new TextureRegion2D(texture))
        {
        }

        public Color Color { get; set; }
        public bool IsVisible { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public SpriteEffects Effect { get; set; }
        public object Tag { get; set; }

        private TextureRegion2D _textureRegion;
        public TextureRegion2D TextureRegion
        {
            get { return _textureRegion; }
            set
            {
                if (value == null)
                    throw new InvalidOperationException("TextureRegion cannot be null");

                _textureRegion = value;
            }
        }

        public Vector2 OriginNormalized
        {
            get { return new Vector2(Origin.Y / TextureRegion.Width, Origin.Y / TextureRegion.Height); }
            set { Origin = new Vector2(value.X * TextureRegion.Width, value.Y * TextureRegion.Height); }
        }

        public Rectangle GetBoundingRectangle()
        {
            // TODO: the bounding rectangle should take origin, scaling and rotation into account
            // http://stackoverflow.com/questions/622140/calculate-bounding-box-coordinates-from-a-rotated-rectangle-picture-inside
            var x = (int) (Position.X - Origin.X);
            var y = (int) (Position.Y - Origin.Y);
            var width = TextureRegion.Width;
            var height = TextureRegion.Height;
            return new Rectangle(x, y, width, height);
        }
    }
}
