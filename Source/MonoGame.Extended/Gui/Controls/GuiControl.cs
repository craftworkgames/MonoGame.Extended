using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class GuiControl : IMovable, ISizable
    {
        protected GuiControl(GuiTemplate style)
        {
            Style = style;
            IsEnabled = true;
        }

        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public SizeF Size { get; set; }
        public float Left => Position.X;
        public float Top => Position.Y;
        public float Right => Position.X + Width;
        public float Bottom => Position.Y + Height;
        public float Width => Size.Width;
        public float Height => Size.Height;
        public Vector2 Center => new Vector2(Position.X + Width * 0.5f, Position.Y + Height * 0.5f);
        public RectangleF BoundingRectangle => new RectangleF(Left, Top, Width, Height);
        public bool IsFocused { get; internal set; }
        public bool IsHovered { get; private set; }
        public bool IsEnabled { get; set; }

        public GuiTemplate Style { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            var destinationRectangle = GetDestinationRectangle();
            Style.Draw(spriteBatch, destinationRectangle.ToRectangleF());
        }

        private Rectangle GetDestinationRectangle()
        {
            //var size = ResizeToFit(TextureRegion.Size, new SizeF(Size.Width, Size.Height));
            //var x = Position.X + Size.Width * 0.5f - size.X * 0.5f;
            //var y = Position.Y + Size.Height * 0.5f - size.Y * 0.5f;
            //var controlRectangle = new Rectangle((int)x, (int)y, size.X, size.Y);
            //return controlRectangle;
            return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.Width, (int)Size.Height);
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