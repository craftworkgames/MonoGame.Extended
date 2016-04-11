using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public interface IShapeBase
    {
        RectangleF GetBoundingRectangle();
        /// <summary>
        /// Returns a point on the outline of the shape, origin at topleft going clockwise
        /// </summary>
        /// <param name="t">A value between 0 and 1</param>
        Vector2 PointOnOutline(float t);
    }

    public interface IShapeF : IShapeBase
    {
        float Left { get; }
        float Top { get; }
        float Right { get; }
        float Bottom { get; }
        /// <summary>
        /// Returns a point inside the shape, chosen randomly
        /// </summary>
        Vector2 RandomPointInside();
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
            result = shape.Contains(point);
        }
    }
}