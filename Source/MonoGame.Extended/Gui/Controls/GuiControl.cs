using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui.Styles;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Gui.Controls
{
    public abstract class GuiControl : IUpdate
    {
        protected GuiControl()
        {
            IsHovered = false;
        }

        public bool IsHovered { get; private set; }

        public IShapeF Shape
        {
            get
            {
                var desiredSize = GetDesiredSize();
                var size = new Vector2(desiredSize.Width, desiredSize.Height);
                return new RectangleF(Position, size);
            }
        }

        protected virtual Size GetDesiredSize()
        {
            return GetCurrentDrawable().Size;
        }

        protected abstract IGuiDrawable GetCurrentDrawable();
        
        public Vector2 Position { get; set; }

        public void Draw(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            var drawable = GetCurrentDrawable();
            drawable.Draw(spriteBatch, rectangle);
        }

        public virtual void Update(GameTime gameTime) { }

        public bool Contains(Vector2 point)
        {
            return Shape.Contains(point);
        }

        public bool Contains(Point point)
        {
            return Shape.Contains(point.ToVector2());
        }

        public bool Contains(int x, int y)
        {
            return Shape.Contains(x, y);
        }

        public virtual void OnMouseEnter(object sender, MouseEventArgs args)
        {
            IsHovered = true;
        }

        public virtual void OnMouseLeave(object sender, MouseEventArgs args)
        {
            IsHovered = false;
        }

        public virtual void OnMouseMoved(object sender, MouseEventArgs args) { }
        public virtual void OnMouseDown(object sender, MouseEventArgs args) { }
        public virtual void OnMouseUp(object sender, MouseEventArgs args) { }
    }
}