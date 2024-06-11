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

    /// <summary>
    /// Gets a rectangle that is relative to the specified source rectangle, with the specified offsets and dimensions.
    /// </summary>
    /// <param name="source">The source rectangle.</param>
    /// <param name="x">The x-coordinate of the relative rectangle, relative to the source rectangle.</param>
    /// <param name="y">The y-coordinate of the relative rectangle, relative to the source rectangle.</param>
    /// <param name="width">The width, in pixels, of the relative rectangle.</param>
    /// <param name="height">The height, in pixels, of the relative rectangle.</param>
    /// <returns>The relative rectangle, clipped to the source rectangle.</returns>
    public static Rectangle GetRelativeRectangle(Rectangle source, int x, int y, int width, int height)
    {
        int absoluteX = source.X + x;
        int absoluteY = source.Y + y;

        Rectangle relative;
        relative.X = MathHelper.Clamp(absoluteX, source.Left, source.Right);
        relative.Y = MathHelper.Clamp(absoluteY, source.Top, source.Bottom);
        relative.Width = Math.Max(Math.Min(absoluteX + width, source.Right) - relative.X, 0);
        relative.Height = Math.Max(Math.Min(absoluteY + height, source.Bottom) - relative.Y, 0);

        return relative;
    }
}
