using Microsoft.Xna.Framework;

namespace MonoGame.Extended
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

        public static RectangleF ToRectangleF(this Rectangle rectangle)
        {
            return new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static Rectangle Clip(this Rectangle rectangle, Rectangle clippingRectangle)
        {
            var clip = clippingRectangle;
            rectangle.X = clip.X > rectangle.X ? clip.X : rectangle.X;
            rectangle.Y = clip.Y > rectangle.Y ? clip.Y : rectangle.Y;
            rectangle.Width = rectangle.Right > clip.Right ? clip.Right - rectangle.X : rectangle.Width;
            rectangle.Height = rectangle.Bottom > clip.Bottom ? clip.Bottom - rectangle.Y : rectangle.Height;

            if (rectangle.Width <= 0 || rectangle.Height <= 0)
                return Rectangle.Empty;

            return rectangle;
        }
    }
}
