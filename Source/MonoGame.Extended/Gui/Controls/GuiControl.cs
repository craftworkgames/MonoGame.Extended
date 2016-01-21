using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Gui.Drawables;
using MonoGame.Extended.InputListeners;

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

        public virtual void LayoutChildren(Rectangle boundingRectangle)
        {
        }
        
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

        public Point Location { get; set; }
        public bool IsHovered { get; private set; }
        public int Left { get { return Location.X; } }
        public int Top { get { return Location.Y; } }
        public int Right { get { return Location.X + Width; } }
        public int Bottom {  get { return Location.Y + Height; } }
        public int Width { get { return DesiredSize.Width; } }
        public int Height { get { return DesiredSize.Height; } }
        public Size ActualSize {  get { return DesiredSize; } }
        public Rectangle BoundingRectangle { get { return new Rectangle(Location, ActualSize); } }

        public virtual Size DesiredSize
        {
            get { return GetCurrentDrawable().DesiredSize; }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rectangle)
        {
            GetCurrentDrawable().Draw(spriteBatch, BoundingRectangle);
        }
        
        public bool Contains(Vector2 point)
        {
            return BoundingRectangle.Contains(point);
        }

        public bool Contains(Point point)
        {
            return BoundingRectangle.Contains(point.ToVector2());
        }

        public bool Contains(int x, int y)
        {
            return BoundingRectangle.Contains(x, y);
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