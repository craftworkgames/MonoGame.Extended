using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public static class RectangleExtensions
    {
        /// <summary>
        ///     Gets the corners of the rectangle in a clockwise direction starting at the top left.
        /// </summary>
        public static Point[] GetCorners(this Rectangle rectangle)
        {
            var corners = new Point[4];
            corners[0] = new Point(rectangle.Left, rectangle.Top);
            corners[1] = new Point(rectangle.Right, rectangle.Top);
            corners[2] = new Point(rectangle.Right, rectangle.Bottom);
            corners[3] = new Point(rectangle.Left, rectangle.Bottom);
            return corners;
        }

        /// <summary>
        ///     Gets the corners of the rectangle in a clockwise direction starting at the top left.
        /// </summary>
        public static Vector2[] GetCorners(this RectangleF rectangle)
        {
            var corners = new Vector2[4];
            corners[0] = new Vector2(rectangle.Left, rectangle.Top);
            corners[1] = new Vector2(rectangle.Right, rectangle.Top);
            corners[2] = new Vector2(rectangle.Right, rectangle.Bottom);
            corners[3] = new Vector2(rectangle.Left, rectangle.Bottom);
            return corners;
        }

        public static Rectangle ToRectangle(this RectangleF rectangle)
        {
            return new Rectangle((int) rectangle.X, (int) rectangle.Y, (int) rectangle.Width, (int) rectangle.Height);
        }

        public static RectangleF ToRectangleF(this Rectangle rectangle)
        {
            return new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
    }
}