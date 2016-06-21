using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class GuiControl : IMovable
    {
        private readonly GuiContentService _contentService;

        protected GuiControl(GuiContentService contentService, GuiControlStyle style)
        {
            _contentService = contentService;

            Style = style;
            IsEnabled = true;

            ApplySpriteStyle(style.Normal);
            Size = TextureRegion.Size;
        }

        public string Name { get; set; }
        public GuiControlStyle Style { get; set; }
        public TextureRegion2D TextureRegion { get; set; }
        public Color Color { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public float Left => Position.X;
        public float Top => Position.Y;
        public float Right => Position.X + Width;
        public float Bottom => Position.Y + Height;
        public float Width => Size.X;
        public float Height => Size.Y;
        public Vector2 Center => new Vector2(Position.X + Width * 0.5f, Position.Y + Height * 0.5f);
        public RectangleF BoundingRectangle => new RectangleF(Position, Size);
        public bool IsFocused { get; internal set; }
        public bool IsHovered { get; private set; }
        public bool IsEnabled { get; set; }

        protected abstract GuiSpriteStyle GetCurrentStyle();

        private void ApplySpriteStyle(GuiSpriteStyle spriteStyle)
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

        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;

        public virtual void Update(GameTime gameTime)
        {
            ApplySpriteStyle(GetCurrentStyle());
        }

        public virtual void OnMouseDown(object sender, MouseEventArgs args)
        {
            MouseDown.Raise(this, args);
        }

        public virtual void OnMouseUp(object sender, MouseEventArgs args)
        {
            MouseUp.Raise(this, args);
        }

        public virtual void OnKeyTyped(object sender, KeyboardEventArgs args)
        {
        }

        public virtual void OnMouseLeave(object sender, MouseEventArgs args)
        {
            IsHovered = false;
        }

        public virtual void OnMouseEnter(object sender, MouseEventArgs args)
        {
            IsHovered = true;
        }

        public bool Contains(Vector2 point)
        {
            return BoundingRectangle.Contains(point);
        }

        public bool Contains(Point point)
        {
            return BoundingRectangle.Contains(point);
        }
    }
}