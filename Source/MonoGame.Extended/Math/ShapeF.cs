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
        public static bool Intersects<TShapeA, TShapeB>(this TShapeA shapeA, TShapeB shapeB)
            where TShapeA : struct, IShapeF
            where TShapeB : struct, IShapeF
        {
            var intersects = false;

            if (shapeA is RectangleF rectangleA && shapeB is RectangleF rectangleB)
            {
                intersects = rectangleA.Intersects(rectangleB);
            }
            else if (shapeA is CircleF circleA && shapeB is CircleF circleB)
            {
                intersects = circleA.Intersects(circleB);
            }
            else if (shapeA is RectangleF rect1 && shapeB is CircleF circ1)
            {
                return Intersects(circ1, rect1);
            }
            else if (shapeA is CircleF circ2 && shapeB is RectangleF rect2)
            {
                return Intersects(circ2, rect2);
            }

            return intersects;
        }

        public static RectangleF GetEnclosingRectangle<TShape>(this TShape shape)
            where TShape : struct, IShapeF
        {
            if (shape is RectangleF rectangleA)
            {
                return rectangleA;
            }
            else if (shape is CircleF circleA)
            {
                return circleA.ToRectangleF();
            }
            else
            {
                throw new NotImplementedException();
            }
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
    }
}
