using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Sprites
{
    public class Sprite : Transform2D<Sprite>, IColorable, IRectangular, ISpriteBatchDrawable
    {
        private TextureRegion2D _textureRegion;

        public Sprite(TextureRegion2D textureRegion)
        {
            if (textureRegion == null) throw new ArgumentNullException(nameof(textureRegion));

            _textureRegion = textureRegion;

            Alpha = 1.0f;
            Color = Color.White;
            IsVisible = true;
            Scale = Vector2.One;
            Effect = SpriteEffects.None;
            OriginNormalized = new Vector2(0.5f, 0.5f);
            Depth = 0.0f;
        }

        public Sprite(Texture2D texture)
            : this(new TextureRegion2D(texture))
        {
        }

        public float Alpha { get; set; }
        public float Depth { get; set; }
        public object Tag { get; set; }

        public Vector2 OriginNormalized
        {
            get { return new Vector2(Origin.X/TextureRegion.Width, Origin.Y/TextureRegion.Height); }
            set { Origin = new Vector2(value.X*TextureRegion.Width, value.Y*TextureRegion.Height); }
        }

        public Color Color { get; set; }

        public RectangleF BoundingRectangle
        {
            get
            {
                var corners = GetCorners();
                var min = new Vector2(corners.Min(i => i.X), corners.Min(i => i.Y));
                var max = new Vector2(corners.Max(i => i.X), corners.Max(i => i.Y));
                return new RectangleF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
            }
        }

        public bool IsVisible { get; set; }
        public Vector2 Origin { get; set; }
        public SpriteEffects Effect { get; set; }

        public TextureRegion2D TextureRegion
        {
            get { return _textureRegion; }
            set
            {
                if (value == null)
                    throw new InvalidOperationException("TextureRegion cannot be null");

                // preserve the origin if the texture size changes
                var originNormalized = OriginNormalized;
                _textureRegion = value;
                OriginNormalized = originNormalized;
            }
        }

        public Vector2[] GetCorners()
        {
            var min = -Origin;
            var max = min + new Vector2(TextureRegion.Width, TextureRegion.Height);
            var offset = Position;

            if (Scale != Vector2.One)
            {
                min = min*Scale;
                max = max*Scale;
            }

            var corners = new Vector2[4];
            corners[0] = min;
            corners[1] = new Vector2(max.X, min.Y);
            corners[2] = max;
            corners[3] = new Vector2(min.X, max.Y);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Rotation != 0)
            {
                var matrix = Matrix.CreateRotationZ(Rotation);

                for (var i = 0; i < 4; i++)
                    corners[i] = Vector2.Transform(corners[i], matrix);
            }

            for (var i = 0; i < 4; i++)
                corners[i] += offset;

            return corners;
        }
    }
}