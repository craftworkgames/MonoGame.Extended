namespace MonoGame.Extended
{
    /// <summary>
    ///     Base class for shapes.
    /// </summary>
    /// <remakarks>
    ///     Created to allow checking intersection between shapes of different types.
    /// </remakarks>
    public interface IShapeF
    {
        /// <summary>
        /// Gets or sets the position of the shape.
        /// </summary>
        Point2 Position { get; set; }
    }

    /// <summary>
    ///     Class that implements methods for shared <see cref="IShapeF" /> methods.
    /// </summary>
    public static class Shape
    {
        /// <summary>
        ///     Check if two shapes intersect.
        /// </summary>
        /// <param name="a">The first shape.</param>
        /// <param name="b">The second shape.</param>
        /// <returns>True if the two shapes intersect.</returns>
        public static bool Intersects(this IShapeF a, IShapeF b)
        {
            var intersects = false;

            if (a is RectangleF rectA && b is RectangleF rectB)
            {
                intersects = rectA.Intersects(rectB);
            }
            else if (a is CircleF circA && b is CircleF circB)
            {
                intersects = circA.Intersects(circB);
            }
            else if (a is RectangleF rect1 && b is CircleF circ1)
            {
                return Intersects(circ1, rect1);
            }
            else if (a is CircleF circ2 && b is RectangleF rect2)
            {
                return Intersects(circ2, rect2);
            }

            return intersects;
        }

        /// <summary>
        ///     Checks if a circle and rectangle intersect.
        /// </summary>
        /// <param name="circ">Circle to check intersection with rectangle.</param>
        /// <param name="rect">Rectangle to check intersection with circle.</param>
        /// <returns>True if the circle and rectangle intersect.</returns>
        public static bool Intersects(CircleF circ, RectangleF rect)
        {
            var closestPoint = rect.ClosestPointTo(circ.Center);
            return circ.Contains(closestPoint);
        }
    }
}