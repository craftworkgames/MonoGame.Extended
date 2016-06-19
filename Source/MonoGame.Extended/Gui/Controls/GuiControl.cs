using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class GuiControl : IMovable
    {
        private readonly GuiContentService _contentService;

        protected GuiControl(GuiContentService contentService, GuiControlStyle style)
        {
            Style = style;
            _contentService = contentService;

            ApplySpriteStyle(style.Normal);
            Size = TextureRegion.Size;
        }

        public string Name { get; set; }
        public GuiControlStyle Style { get; set; }
        public TextureRegion2D TextureRegion { get; set; }
        public Color Color { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public RectangleF BoundingRectangle => new RectangleF(Position, Size);

        protected void ApplySpriteStyle(GuiSpriteStyle spriteStyle)
        {
            TextureRegion = _contentService.GetTextureRegion(spriteStyle.TextureRegion);
            Color = spriteStyle.Color;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var destinationRectangle = GetDestinationRectangle();
            spriteBatch.Draw(TextureRegion, destinationRectangle, Color * (Color.A / 255f));
        }

        private Rectangle GetDestinationRectangle()
        {
            var size = ResizeToFit(TextureRegion.Size, new SizeF(Size.X, Size.Y));
            var x = Position.X + Size.X * 0.5f - size.X * 0.5f;
            var y = Position.Y + Size.Y * 0.5f - size.Y * 0.5f;
            var controlRectangle = new Rectangle((int)x, (int)y, size.X, size.Y);
            return controlRectangle;
        }

        private static Point ResizeToFit(Size imageSize, SizeF boxSize)
        {
            var widthScale = boxSize.Width / imageSize.Width;
            var heightScale = boxSize.Height / imageSize.Height;
            var scale = Math.Min(widthScale, heightScale);
            return new Point((int)Math.Round(imageSize.Width * scale), (int)Math.Round(imageSize.Height * scale));
        }

        public abstract void OnMouseMove();
    }
}