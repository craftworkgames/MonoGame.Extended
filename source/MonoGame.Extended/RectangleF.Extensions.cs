using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended;

/// <summary>
/// Provides extension methods for the <see cref="RectangleF"/> structure.
/// </summary>
public static class RectangleFExtensions
{
    /// <summary>
    /// Gets the corners of the rectangle in a clockwise direction starting at the top left.
    /// </summary>
    /// <param name="rectangle">The rectangle to get the corners of.</param>
    /// <returns>An array of <see cref="Vector2"/> elements representing the corners of the rectangle.</returns>
    public static Vector2[] GetCorners(this RectangleF rectangle)
    {
        var corners = new Vector2[4];
        corners[0] = new Vector2(rectangle.Left, rectangle.Top);
        corners[1] = new Vector2(rectangle.Right, rectangle.Top);
        corners[2] = new Vector2(rectangle.Right, rectangle.Bottom);
        corners[3] = new Vector2(rectangle.Left, rectangle.Bottom);
        return corners;
    }

    /// <summary>
    /// Converts the specified <see cref="RectangleF"/> to a <see cref="Rectangle"/>.
    /// </summary>
    /// <param name="rectangle">The rectangle to convert.</param>
    /// <returns>The converted <see cref="Rectangle"/>.</returns>
    public static Rectangle ToRectangle(this RectangleF rectangle)
    {
        return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
    }

    /// <summary>
    /// Clips the specified rectangle against the specified clipping rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle to clip.</param>
    /// <param name="clippingRectangle">The rectangle to clip against.</param>
    /// <returns>The clipped rectangle, or <see cref="RectangleF.Empty"/> if the rectangles do not intersect.</returns>
    public static RectangleF Clip(this RectangleF rectangle, RectangleF clippingRectangle)
    {
        var clip = clippingRectangle;
        rectangle.X = clip.X > rectangle.X ? clip.X : rectangle.X;
        rectangle.Y = clip.Y > rectangle.Y ? clip.Y : rectangle.Y;
        rectangle.Width = rectangle.Right > clip.Right ? clip.Right - rectangle.X : rectangle.Width;
        rectangle.Height = rectangle.Bottom > clip.Bottom ? clip.Bottom - rectangle.Y : rectangle.Height;

        if (rectangle.Width <= 0 || rectangle.Height <= 0)
            return RectangleF.Empty;

        return rectangle;
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
    public static RectangleF GetRelativeRectangle(this RectangleF source, float x, float y, float width, float height)
    {
        float absoluteX = source.X + x;
        float absoluteY = source.Y + y;

        RectangleF relative;
        relative.X = MathHelper.Clamp(absoluteX, source.Left, source.Right);
        relative.Y = MathHelper.Clamp(absoluteY, source.Top, source.Bottom);
        relative.Width = Math.Max(Math.Min(absoluteX + width, source.Right) - relative.X, 0);
        relative.Height = Math.Max(Math.Min(absoluteY + height, source.Bottom) - relative.Y, 0);

        return relative;
    }
}
