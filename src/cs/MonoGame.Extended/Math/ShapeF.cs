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
                    OrientedBoundingRectangle orientedBoundingRectangleA => IntersectsInternal(orientedBoundingRectangleA, shapeB),
                    _ => throw new ArgumentOutOfRangeException(nameof(shapeA))
                };
        }

        private static bool IntersectsInternal(CircleF circle, IShapeF shape)
        {
            return shape switch
                {
                    CircleF otherCircle => CircleF.Intersects(circle, otherCircle),
                    RectangleF otherRectangle => Intersects(circle, otherRectangle),
                    OrientedBoundingRectangle otherOrientedBoundingRectangle => Intersects(circle, otherOrientedBoundingRectangle),
                    _ => throw new ArgumentOutOfRangeException(nameof(shape))
                };
        }

        private static bool IntersectsInternal(RectangleF rectangle, IShapeF shape)
        {
            return shape switch
                {
                    CircleF otherCircle => Intersects(otherCircle, rectangle),
                    RectangleF otherRectangle => RectangleF.Intersects(rectangle, otherRectangle),
                    OrientedBoundingRectangle otherOrientedBoundingRectangle => Intersects(rectangle, otherOrientedBoundingRectangle),
                    _ => throw new ArgumentOutOfRangeException(nameof(shape))
                };
        }

        private static bool IntersectsInternal(OrientedBoundingRectangle orientedBoundingRectangle, IShapeF shape)
        {
            return shape switch
                {
                    CircleF circleB => Intersects(circleB, orientedBoundingRectangle),
                    RectangleF rectangleB => Intersects(rectangleB, orientedBoundingRectangle),
                    OrientedBoundingRectangle orientedBoundingRectangleB => OrientedBoundingRectangle.Intersects(orientedBoundingRectangle, orientedBoundingRectangleB),
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
        /// Checks whether a <see cref="CircleF"/> and <see cref="OrientedBoundingRectangle"/> intersects.
        /// </summary>
        /// <param name="circle"><see cref="CircleF"/>to use in intersection test.</param>
        /// <param name="orientedBoundingRectangle"><see cref="OrientedBoundingRectangle"/>to use in intersection test.</param>
        /// <returns>True if the circle and oriented bounded rectangle intersects, otherwise false.</returns>
        public static bool Intersects(CircleF circle, OrientedBoundingRectangle orientedBoundingRectangle)
        {
            var rotation = Matrix2.CreateRotationZ(-orientedBoundingRectangle.Orientation.Rotation);
            var circleCenterInRectangleSpace = rotation.Transform(circle.Center - orientedBoundingRectangle.Center);
            var circleInRectangleSpace = new CircleF(circleCenterInRectangleSpace, circle.Radius);
            var rectangleInLocalSpace = OrientedBoundingRectangle.Transform(orientedBoundingRectangle, ref rotation);
            var boundingRectangle = new BoundingRectangle(rectangleInLocalSpace.Center, rectangleInLocalSpace.Radii);
            return circleInRectangleSpace.Intersects(boundingRectangle);
        }

        /// <summary>
        /// Checks if a <see cref="RectangleF"/> and <see cref="OrientedBoundingRectangle"/> intersects.
        /// </summary>
        /// <param name="rectangleF"></param>
        /// <param name="orientedBoundingRectangle"></param>
        /// <returns>True if objects are intersecting, otherwise false.</returns>
        public static bool Intersects(RectangleF rectangleF, OrientedBoundingRectangle orientedBoundingRectangle)
        {
            throw new NotImplementedException();
        }
    }
}
