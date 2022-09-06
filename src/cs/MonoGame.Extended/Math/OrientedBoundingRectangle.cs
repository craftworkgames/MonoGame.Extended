using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.4; Bounding Volumes - Oriented Bounding Boxes (OBBs), pg 101.

    /// <summary>
    /// An oriented bounding rectangle is a rectangular block, much like a bounding rectangle
    /// <see cref="BoundingRectangle" /> but with an arbitrary orientation <see cref="Vector2" />.
    /// </summary>
    /// <seealso cref="IEquatable{T}" />
    [DebuggerDisplay($"{{{nameof(DebugDisplayString)},nq}}")]
    public struct OrientedBoundingRectangle : IEquatable<OrientedBoundingRectangle>
    {
        /// <summary>
        /// The centre position of this <see cref="OrientedBoundingRectangle" />.
        /// </summary>
        public Point2 Center;

        /// <summary>
        /// The distance from the <see cref="Center" /> point along both axes to any point on the boundary of this
        /// <see cref="OrientedBoundingRectangle" />.
        /// </summary>
        ///
        public Vector2 Radii;

        /// <summary>
        /// The rotation matrix <see cref="Matrix2" /> of the bounding rectangle <see cref="OrientedBoundingRectangle" />.
        /// </summary>
        public Matrix2 Orientation;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingRectangle" /> structure from the specified centre
        /// <see cref="Point2" /> and the radii <see cref="Size2" />.
        /// </summary>
        /// <param name="center">The centre <see cref="Point2" />.</param>
        /// <param name="radii">The radii <see cref="Vector2" />.</param>
        /// <param name="orientation">The orientation <see cref="Matrix2" />.</param>
        public OrientedBoundingRectangle(Point2 center, Size2 radii, Matrix2 orientation)
        {
            Center = center;
            Radii = radii;
            Orientation = orientation;
        }

        /// <summary>
        /// Gets a list of points defining the corner points of the oriented rectangle.
        /// </summary>
        public IReadOnlyList<Vector2> Points
        {
            get
            {
                var topLeft = -Radii;
                var bottomLeft = -new Vector2(Radii.X, -Radii.Y);
                var topRight = (Vector2)new Point2(Radii.X, -Radii.Y);
                var bottomRight = Radii;

                return new List<Vector2>
                    {
                        Vector2.Transform(topRight, Orientation) + Center,
                        Vector2.Transform(topLeft, Orientation) + Center,
                        Vector2.Transform(bottomLeft, Orientation) + Center,
                        Vector2.Transform(bottomRight, Orientation) + Center
                    };
            }
        }

        /// <summary>
        /// Computes the <see cref="OrientedBoundingRectangle"/> from the specified <paramref name="rectangle"/>
        /// transformed by <paramref name="transformMatrix"/>.
        /// </summary>
        /// <param name="rectangle">The <see cref="OrientedBoundingRectangle"/> to transform.</param>
        /// <param name="transformMatrix">The <see cref="Matrix2"/> transformation.</param>
        /// <returns>A new <see cref="OrientedBoundingRectangle"/>.</returns>
        public static OrientedBoundingRectangle Transform(OrientedBoundingRectangle rectangle, ref Matrix2 transformMatrix)
        {
            Transform(ref rectangle, ref transformMatrix, out var result);
            return result;
        }

        private static void Transform(ref OrientedBoundingRectangle rectangle, ref Matrix2 transformMatrix, out OrientedBoundingRectangle result)
        {
            PrimitivesHelper.TransformOrientedBoundingRectangle(
                ref rectangle.Center,
                ref rectangle.Orientation,
                ref transformMatrix);
            result.Center = rectangle.Center;
            result.Radii = rectangle.Radii;
            result.Orientation = rectangle.Orientation;
        }

        /// <summary>
        /// Compares to two <see cref="OrientedBoundingRectangle"/> structures. The result specifies whether the
        /// the values of the <see cref="Center"/>, <see cref="Radii"/> and <see cref="Orientation"/> are
        /// equal.
        /// </summary>
        /// <param name="left">The left <see cref="OrientedBoundingRectangle" />.</param>
        /// <param name="right">The right <see cref="OrientedBoundingRectangle" />.</param>
        /// <returns><c>true</c> if left and right argument are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(OrientedBoundingRectangle left, OrientedBoundingRectangle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares to two <see cref="OrientedBoundingRectangle"/> structures. The result specifies whether the
        /// the values of the <see cref="Center"/>, <see cref="Radii"/> or <see cref="Orientation"/> are
        /// unequal.
        /// </summary>
        /// <param name="left">The left <see cref="OrientedBoundingRectangle" />.</param>
        /// <param name="right">The right <see cref="OrientedBoundingRectangle" />.</param>
        /// <returns><c>true</c> if left and right argument are unequal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(OrientedBoundingRectangle left, OrientedBoundingRectangle right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether two instances of <see cref="OrientedBoundingRectangle"/> are equal.
        /// </summary>
        /// <param name="other">The other <see cref="OrientedBoundingRectangle"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="OrientedBoundingRectangle"/> is equal
        /// to the current <see cref="OrientedBoundingRectangle"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(OrientedBoundingRectangle other)
        {
            return Center.Equals(other.Center) && Radii.Equals(other.Radii) && Orientation.Equals(other.Orientation);
        }

        /// <summary>
        /// Determines whether two instances of <see cref="OrientedBoundingRectangle"/> are equal.
        /// </summary>
        /// <param name="obj">The <see cref="OrientedBoundingRectangle"/> to compare to.</param>
        /// <returns><c>true</c> if the specified <see cref="OrientedBoundingRectangle"/> is equal
        /// to the current <see cref="OrientedBoundingRectangle"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is OrientedBoundingRectangle other && Equals(other);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Center, Radii, Orientation);
        }

        public static explicit operator OrientedBoundingRectangle(RectangleF rectangle)
        {
            var radii = new Size2(rectangle.Width * 0.5f, rectangle.Height * 0.5f);
            var centre = new Point2(rectangle.X + radii.Width, rectangle.Y + radii.Height);

            return new OrientedBoundingRectangle(centre, radii, Matrix2.Identity);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this <see cref="OrientedBoundingRectangle" />.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this <see cref="OrientedBoundingRectangle" />.
        /// </returns>
        public override string ToString()
        {
            return $"Centre: {Center}, Radii: {Radii}, Orientation: {Orientation}";
        }

        internal string DebugDisplayString => ToString();
    }
}
