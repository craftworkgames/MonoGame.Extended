using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui.Drawables;
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

        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<MouseEventArgs> MouseMoved;

        protected abstract IGuiDrawable GetCurrentDrawable();
        public virtual void Update(GameTime gameTime) { }

        public virtual void OnMouseMoved(object sender, MouseEventArgs args)
        {
            MouseMoved.Raise(this, args);
        }

        public virtual void OnMouseDown(object sender, MouseEventArgs args)
        {
            MouseDown.Raise(this, args);
        }

        public virtual void OnMouseUp(object sender, MouseEventArgs args)
        {
            MouseUp.Raise(this, args);
        }

        public GuiHorizontalAlignment HorizontalAlignment { get; set; }
        public GuiVerticalAlignment VerticalAlignment { get; set; }

        public Vector2 Position { get; set; }
        public bool IsHovered { get; private set; }

        private IShapeF _shape;
        public IShapeF Shape
        {
            get
            {
                if (_shape != null)
                    return _shape;

                var desiredSize = DesiredSize;
                var size = new Vector2(desiredSize.Width, desiredSize.Height);
                return new RectangleF(Position, size);
            }
            set { _shape = value; }
        }

        public virtual Size DesiredSize
        {
            get { return GetCurrentDrawable().DesiredSize; }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            var drawable = GetCurrentDrawable();
            var size = DesiredSize;
            var clientRectangle = new Rectangle((int)Position.X, (int)Position.Y, size.Width, size.Height);
            drawable.Draw(spriteBatch, clientRectangle);
        }
        
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
    }
}