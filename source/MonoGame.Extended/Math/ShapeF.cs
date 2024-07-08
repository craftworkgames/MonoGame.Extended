using System;
using Microsoft.Xna.Framework;

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
        Vector2 Position { get; set; }

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
        /// <param name="a">The first shape.</param>
        /// <param name="b">The second shape.</param>
        /// <returns>True if the two shapes intersect.</returns>
        public static bool Intersects(this IShapeF a, IShapeF b)
        {
            return a switch
                {
                    CircleF circleA when b is CircleF circleB => circleA.Intersects(circleB),
                    CircleF circleA when b is RectangleF rectangleB => circleA.Intersects(rectangleB),
                    CircleF circleA when b is OrientedRectangle orientedRectangleB => Intersects(circleA, orientedRectangleB),

                    RectangleF rectangleA when b is CircleF circleB => Intersects(circleB, rectangleA),
                    RectangleF rectangleA when b is RectangleF rectangleB => rectangleA.Intersects(rectangleB),
                    RectangleF rectangleA when b is OrientedRectangle orientedRectangleB => Intersects(rectangleA, orientedRectangleB).Intersects,

                    OrientedRectangle orientedRectangleA when b is CircleF circleB => Intersects(circleB, orientedRectangleA),
                    OrientedRectangle orientedRectangleA when b is RectangleF rectangleB => Intersects(rectangleB, orientedRectangleA).Intersects,
                    OrientedRectangle orientedRectangleA when b is OrientedRectangle orientedRectangleB
                        => OrientedRectangle.Intersects(orientedRectangleA, orientedRectangleB).Intersects,

                    _ => throw new ArgumentOutOfRangeException(nameof(a))
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
            var rotation = Matrix3x2.CreateRotationZ(orientedRectangle.Orientation.Rotation);
            var circleCenterInRectangleSpace = rotation.Transform(circle.Center - orientedRectangle.Center);
            var circleInRectangleSpace = new CircleF(circleCenterInRectangleSpace, circle.Radius);
            var boundingRectangle = new BoundingRectangle(new Vector2(), orientedRectangle.Radii);
            return circleInRectangleSpace.Intersects(boundingRectangle);
        }

        /// <summary>
        /// Checks if a <see cref="RectangleF"/> and <see cref="OrientedRectangle"/> intersects.
        /// </summary>
        /// <param name="rectangleF"></param>
        /// <param name="orientedRectangle"></param>
        /// <returns>True if objects are intersecting, otherwise false.</returns>
        public static (bool Intersects, Vector2 MinimumTranslationVector) Intersects(RectangleF rectangleF, OrientedRectangle orientedRectangle)
        {
            return OrientedRectangle.Intersects(orientedRectangle, (OrientedRectangle)rectangleF);
        }
    }
}
