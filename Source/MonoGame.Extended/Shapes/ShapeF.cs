using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public interface IShapeF
    {
        float Left { get; }
        float Top { get; }
        float Right { get; }
        float Bottom { get; }

        RectangleF GetBoundingRectangle();
        
        bool Contains(Vector2 point);
    }

    public static class ShapeFExtensions
    {
        public static bool Contains(this IShapeF shape, float x, float y) {
            return shape.Contains(new Vector2(x, y));
        }
        public static void Contains(this IShapeF shape, ref Vector2 point, out bool result) {
            result = shape.Contains(point);
        }
        public static bool Contains(this IShapeF shape, Point point) {
            return shape.Contains(new Vector2(point.X, point.Y));
        }
        public static void Contains(this IShapeF shape, ref Point point, out bool result) {
            result = Contains(shape, point);
        }
    }
}