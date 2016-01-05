using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public abstract class ShapeF
    {
        public abstract float Left { get; }
        public abstract float Top { get; }
        public abstract float Right { get; }
        public abstract float Bottom { get; }

        public RectangleF GetBoundingRectangle()
        {
            var minX = Left;
            var minY = Top;
            var maxX = Right;
            var maxY = Bottom;

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        public abstract bool Contains(float x, float y);

        public bool Contains(Vector2 point)
        {
            return Contains(point.X, point.Y);
        }
    }
}