using System;

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
            return shapeA switch
                {
                    CircleF circleA => IntersectsInternal(circleA, shapeB),
                    RectangleF rectangleA => IntersectsInternal(rectangleA, shapeB),
                    OrientedRectangle orientedRectangleA => IntersectsInternal(orientedRectangleA, shapeB),
                    _ => throw new ArgumentOutOfRangeException(nameof(shapeA))
                };
        }

        private static bool IntersectsInternal(CircleF circle, IShapeF shape)
        {
            return shape switch
                {
                    CircleF otherCircle => CircleF.Intersects(circle, otherCircle),
                    RectangleF otherRectangle => Intersects(circle, otherRectangle),
                    OrientedRectangle otherOrientedRectangle => Intersects(circle, otherOrientedRectangle),
                    _ => throw new ArgumentOutOfRangeException(nameof(shape))
                };
        }

        private static bool IntersectsInternal(RectangleF rectangle, IShapeF shape)
        {
            return shape switch
                {
                    CircleF otherCircle => Intersects(otherCircle, rectangle),
                    RectangleF otherRectangle => RectangleF.Intersects(rectangle, otherRectangle),
                    OrientedRectangle otherOrientedRectangle => Intersects(rectangle, otherOrientedRectangle),
                    _ => throw new ArgumentOutOfRangeException(nameof(shape))
                };
        }

        private static bool IntersectsInternal(OrientedRectangle orientedRectangle, IShapeF shape)
        {
            return shape switch
                {
                    CircleF circleB => Intersects(circleB, orientedRectangle),
                    RectangleF rectangleB => Intersects(rectangleB, orientedRectangle),
                    OrientedRectangle orientedRectangleB => OrientedRectangle.Intersects(orientedRectangle, orientedRectangleB),
                    _ => throw new ArgumentOutOfRangeException(nameof(shape))
                };
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
        /// Checks whether a <see cref="CircleF"/> and <see cref="OrientedRectangle"/> intersects.
        /// </summary>
        /// <param name="circle"><see cref="CircleF"/>to use in intersection test.</param>
        /// <param name="orientedRectangle"><see cref="OrientedRectangle"/>to use in intersection test.</param>
        /// <returns>True if the circle and oriented bounded rectangle intersects, otherwise false.</returns>
        public static bool Intersects(CircleF circle, OrientedRectangle orientedRectangle)
        {
            var rotation = Matrix2.CreateRotationZ(-orientedRectangle.Orientation.Rotation);
            var circleCenterInRectangleSpace = rotation.Transform(circle.Center - orientedRectangle.Center);
            var circleInRectangleSpace = new CircleF(circleCenterInRectangleSpace, circle.Radius);
            var rectangleInLocalSpace = OrientedRectangle.Transform(orientedRectangle, ref rotation);
            var boundingRectangle = new BoundingRectangle(rectangleInLocalSpace.Center, rectangleInLocalSpace.Radii);
            return circleInRectangleSpace.Intersects(boundingRectangle);
        }

        /// <summary>
        /// Checks if a <see cref="RectangleF"/> and <see cref="OrientedRectangle"/> intersects.
        /// </summary>
        /// <param name="rectangleF"></param>
        /// <param name="orientedRectangle"></param>
        /// <returns>True if objects are intersecting, otherwise false.</returns>
        public static bool Intersects(RectangleF rectangleF, OrientedRectangle orientedRectangle)
        {
            return OrientedRectangle.Intersects(orientedRectangle, (OrientedRectangle)rectangleF);
        }
    }
}
