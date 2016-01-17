using Microsoft.Xna.Framework;
using MonoGame.Extended.InputListeners;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Gui
{
    public abstract class GuiControl : IUpdate
    {
        protected GuiControl()
        {
            IsHovered = false;
        }

        public bool IsHovered { get; private set; }

        public abstract IShapeF Shape { get; set; }

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