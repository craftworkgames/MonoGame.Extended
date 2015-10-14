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

        /// <summary>
        /// Gets the corners of the rectangle in a clockwise direction starting at the top left.
        /// </summary>
        public static Vector2[] GetCorners(this RectangleF rectangle)
        {
            var corners = new Vector2[4];
            corners[0] = new Vector2(rectangle.Top, rectangle.Left);
            corners[1] = new Vector2(rectangle.Top, rectangle.Right);
            corners[2] = new Vector2(rectangle.Bottom, rectangle.Right);
            corners[3] = new Vector2(rectangle.Bottom, rectangle.Left);
            return corners;
        }

        public static Rectangle ToRectangle(this RectangleF rectangle)
        {
            return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
        }

        public static RectangleF ToRectangleF(this Rectangle rectangle)
        {
            return new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
    }
}