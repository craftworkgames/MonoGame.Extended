using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public static class RectangleExtensions
    {
        /// <summary>
        /// Gets the corners of the rectangle in a clockwise direction starting at the top left.
        /// </summary>
        public static Point[] GetCorners(this Rectangle rectangle)
        {
            var corners = new Point[4];
            corners[0] = new Point(rectangle.Top, rectangle.Left);
            corners[1] = new Point(rectangle.Top, rectangle.Right);
            corners[2] = new Point(rectangle.Bottom, rectangle.Right);
            corners[3] = new Point(rectangle.Bottom, rectangle.Left);
            return corners;
        }
    }
}