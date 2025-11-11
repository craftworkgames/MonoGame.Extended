using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended;

/// <summary>
/// Provides extension methods for the <see cref="Rectangle"/> structure.
/// </summary>
public static class RectangleExtensions
{
    /// <summary>
    /// Gets the corners of the rectangle in a clockwise direction starting at the top left.
    /// </summary>
    /// <param name="rectangle">The rectangle to get the corners of.</param>
    /// <returns>An array of <see cref="Point"/> elements representing the corners of the rectangle.</returns>
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
    /// Converts the specified <see cref="Rectangle"/> to a <see cref="RectangleF"/>.
    /// </summary>
    /// <param name="rectangle">The rectangle to convert.</param>
    /// <returns>The converted <see cref="RectangleF"/>.</returns>
    public static RectangleF ToRectangleF(this Rectangle rectangle)
    {
        return new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    }

    /// <summary>
    /// Clips the specified rectangle against the specified clipping rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle to clip.</param>
    /// <param name="clippingRectangle">The rectangle to clip against.</param>
    /// <returns>The clipped rectangle, or <see cref="Rectangle.Empty"/> if the rectangles do not intersect.</returns>
    public static Rectangle Clip(this Rectangle rectangle, Rectangle clippingRectangle)
    {
        int left = Math.Max(rectangle.Left, clippingRectangle.Left);
        int top = Math.Max(rectangle.Top, clippingRectangle.Top);
        int right = Math.Min(rectangle.Right, clippingRectangle.Right);
        int bottom = Math.Min(rectangle.Bottom, clippingRectangle.Bottom);

        int width = right - left;
        int height = bottom - top;

        if (width <= 0 || height <= 0)
            return Rectangle.Empty;

        return new Rectangle(left, top, width, height);
    }

    /// <summary>
    /// Gets a rectangle that is relative to the specified source rectangle, with the specified offsets and dimensions.
    /// </summary>
    /// <param name="source">The source rectangle.</param>
    /// <param name="x">The x-coordinate of the relative rectangle, relative to the source rectangle.</param>
    /// <param name="y">The y-coordinate of the relative rectangle, relative to the source rectangle.</param>
    /// <param name="width">The width, in pixels, of the relative rectangle.</param>
    /// <param name="height">The height, in pixels, of the relative rectangle.</param>
    /// <returns>The relative rectangle, clipped to the source rectangle.</returns>
    public static Rectangle GetRelativeRectangle(this Rectangle source, int x, int y, int width, int height)
    {
        int absoluteX = source.X + x;
        int absoluteY = source.Y + y;

        Rectangle relative;
        relative.X = (int)MathHelper.Clamp(absoluteX, source.Left, source.Right);
        relative.Y = (int)MathHelper.Clamp(absoluteY, source.Top, source.Bottom);
        relative.Width = Math.Max(Math.Min(absoluteX + width, source.Right) - relative.X, 0);
        relative.Height = Math.Max(Math.Min(absoluteY + height, source.Bottom) - relative.Y, 0);

        return relative;
    }

#if FNA
    // MomoGame compatibility layer

    /// <summary>
    /// Deconstruction method for Rectangle.
    /// </summary>
    public static void Deconstruct(this Rectangle rectangle, out int x, out int y, out int width, out int height)
    {
        x = rectangle.X;
        y = rectangle.Y;
        width = rectangle.Width;
        height = rectangle.Height;

    }
#endif

    /// <summary>
    /// Normalizes the specified <see cref="Rectangle"/> so that the <see cref="Rectangle.Width"/> and
    /// <see cref="Rectangle.Height"/> are positive without changing the location of the rectangle.
    /// </summary>
    /// <param name="rectangle">The <see cref="Rectangle"/> to normalize.</param>
    /// <returns>A <see cref="Rectangle"/> with positive width and height.</returns>
    public static Rectangle Normalize(Rectangle rectangle)
    {
        if (rectangle.Width < 0)
        {
            rectangle.X += rectangle.Width;
            rectangle.Width = -rectangle.Width;
        }

        if (rectangle.Height < 0)
        {
            rectangle.Y += rectangle.Height;
            rectangle.Height = -rectangle.Height;
        }

        return rectangle;
    }

    /// <summary>
    /// Normalizes a <see cref="Rectangle"/> so that the <see cref="Rectangle.Width"/> and
    /// <see cref="Rectangle.Height"/> are positive without changing the location of the rectangle.
    /// </summary>
    /// <param name="rectangle">The source <see cref="Rectangle"/>.</param>
    /// <param name="result">
    /// When this method returns, contains the a normalized <see cref="Rectangle"/> with positive width and height.
    /// </param>
    public static void Normalize(ref this Rectangle rectangle, out Rectangle result)
    {
        result.X = rectangle.X;
        result.Width = rectangle.Width;

        if (result.Width < 0)
        {
            result.X += result.Width;
            result.Width = -result.Width;
        }

        result.Y = rectangle.Y;
        result.Height = rectangle.Height;

        if (result.Height < 0)
        {
            result.Y += result.Height;
            result.Height = -result.Height;
        }
    }
}
