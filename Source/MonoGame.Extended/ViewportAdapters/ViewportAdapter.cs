using Microsoft.Xna.Framework;

// ReSharper disable once CheckNamespace
namespace MonoGame.Extended.ViewportAdapters
{
    public abstract class ViewportAdapter
    {
        protected ViewportAdapter()
        {
        }

        public abstract int VirtualWidth { get; }
        public abstract int VirtualHeight { get; }
        public abstract int ViewportWidth { get; }
        public abstract int ViewportHeight { get; }
        public abstract Matrix GetScaleMatrix();

        public Rectangle BoundingRectangle
        {
            get { return new Rectangle(0, 0, VirtualWidth, ViewportHeight); }
        }

        public Point PointToScreen(Point point)
        {
            return PointToScreen(point.X, point.Y);
        }

        public virtual Point PointToScreen(int x, int y)
        {
            var scaleMatrix = GetScaleMatrix();
            var invertedMatrix = Matrix.Invert(scaleMatrix);
            return Vector2.Transform(new Vector2(x, y), invertedMatrix).ToPoint();
        }
    }
}