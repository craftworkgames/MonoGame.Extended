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

        /// <summary>
        /// Gets escribed rectangle, which lying outside the shape
        /// </summary>
        RectangleF BoundingRectangle { get; }
    }

    /// <summary>
    ///     Class that implements methods for shared <see cref="IShapeF" /> methods.
    /// </summary>
    public static class Shape
    {
        /// <summary>
        ///     Check if two shapes intersect.
        /// </summary>
        /// <param name="shapeA">The first shape.</param>
        /// <param name="shapeB">The second shape.</param>
        /// <returns>True if the two shapes intersect.</returns>
        public static bool Intersects(this IShapeF shapeA, IShapeF shapeB)
        {
            switch (shapeA)
            {
                case RectangleF a when shapeB is RectangleF b:
                    return a.Intersects(b);
                case RectangleF a when shapeB is CircleF b:
                    return Intersects(b, a);
                case RectangleF a when shapeB is TriangleF b:
                    return Intersects(b, a);

                case CircleF a when shapeB is RectangleF b:
                    return Intersects(a, b);
                case CircleF a when shapeB is CircleF b:
                    return a.Intersects(b);
                case CircleF a when shapeB is TriangleF b:
                    return Intersects(b, a);

                case TriangleF a when shapeB is RectangleF b:
                    return Intersects(a, b);
                case TriangleF a when shapeB is CircleF b:
                    return Intersects(a, b);
                case TriangleF a when shapeB is TriangleF b:
                    return a.Intersects(b);
            }

            return false;
        }

        /// <summary>
        ///     Checks if a circle and rectangle intersect.
        /// </summary>
        /// <param name="circle">Circle to check intersection with rectangle.</param>
        /// <param name="rectangle">Rectangle to check intersection with circle.</param>
        /// <returns>True if the circle and rectangle intersect.</returns>
        public static bool Intersects(CircleF circle, RectangleF rectangle)
        {
            var closestPoint = rectangle.ClosestPointTo(circle.Center);
            return circle.Contains(closestPoint);
        }

        /// <summary>
        ///     Checks if a triangle and rectangle intersect.
        /// </summary>
        /// <param name="triangle">Triangle to check intersection with rectangle.</param>
        /// <param name="rectangle">Rectangle to check intersection with triangle.</param>
        /// <returns>True if the triangle and rectangle intersect.</returns>
        public static bool Intersects(TriangleF triangle, RectangleF rectangle)
        {
            var closestPoint = rectangle.ClosestPointTo(triangle.Center);
            return triangle.Contains(closestPoint);
        }

        /// <summary>
        ///     Checks if a triangle and rectangle intersect.
        /// </summary>
        /// <param name="triangle">Triangle to check intersection with rectangle.</param>
        /// <param name="circle">Rectangle to check intersection with triangle.</param>
        /// <returns>True if the triangle and rectangle intersect.</returns>
        public static bool Intersects(TriangleF triangle, CircleF circle)
        {
            var closestPoint = circle.ClosestPointTo(triangle.Center);
            return triangle.Contains(closestPoint);
        }
    }
}
