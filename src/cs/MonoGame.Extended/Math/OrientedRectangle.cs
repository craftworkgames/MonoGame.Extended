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
    public struct OrientedRectangle : IEquatable<OrientedRectangle>, IShapeF
    {
        /// <summary>
        /// The centre position of this <see cref="OrientedRectangle" />.
        /// </summary>
        public Point2 Center;

        /// <summary>
        /// The distance from the <see cref="Center" /> point along both axes to any point on the boundary of this
        /// <see cref="OrientedRectangle" />.
        /// </summary>
        ///
        public Vector2 Radii;

        /// <summary>
        /// The rotation matrix <see cref="Matrix2" /> of the bounding rectangle <see cref="OrientedRectangle" />.
        /// </summary>
        public Matrix2 Orientation;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingRectangle" /> structure from the specified centre
        /// <see cref="Point2" /> and the radii <see cref="Size2" />.
        /// </summary>
        /// <param name="center">The centre <see cref="Point2" />.</param>
        /// <param name="radii">The radii <see cref="Vector2" />.</param>
        /// <param name="orientation">The orientation <see cref="Matrix2" />.</param>
        public OrientedRectangle(Point2 center, Size2 radii, Matrix2 orientation)
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

        public Point2 Position
        {
            get => Vector2.Transform(-Radii, Orientation) + Center;
            set => throw new NotImplementedException();
        }

        public RectangleF BoundingRectangle => (RectangleF)this;

        /// <summary>
        /// Computes the <see cref="OrientedRectangle"/> from the specified <paramref name="rectangle"/>
        /// transformed by <paramref name="transformMatrix"/>.
        /// </summary>
        /// <param name="rectangle">The <see cref="OrientedRectangle"/> to transform.</param>
        /// <param name="transformMatrix">The <see cref="Matrix2"/> transformation.</param>
        /// <returns>A new <see cref="OrientedRectangle"/>.</returns>
        public static OrientedRectangle Transform(OrientedRectangle rectangle, ref Matrix2 transformMatrix)
        {
            Transform(ref rectangle, ref transformMatrix, out var result);
            return result;
        }

        private static void Transform(ref OrientedRectangle rectangle, ref Matrix2 transformMatrix, out OrientedRectangle result)
        {
            PrimitivesHelper.TransformOrientedRectangle(
                ref rectangle.Center,
                ref rectangle.Orientation,
                ref transformMatrix);
            result = new OrientedRectangle();
            result.Center = rectangle.Center;
            result.Radii = rectangle.Radii;
            result.Orientation = rectangle.Orientation;
        }

        /// <summary>
        /// Compares to two <see cref="OrientedRectangle"/> structures. The result specifies whether the
        /// the values of the <see cref="Center"/>, <see cref="Radii"/> and <see cref="Orientation"/> are
        /// equal.
        /// </summary>
        /// <param name="left">The left <see cref="OrientedRectangle" />.</param>
        /// <param name="right">The right <see cref="OrientedRectangle" />.</param>
        /// <returns><c>true</c> if left and right argument are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(OrientedRectangle left, OrientedRectangle right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares to two <see cref="OrientedRectangle"/> structures. The result specifies whether the
        /// the values of the <see cref="Center"/>, <see cref="Radii"/> or <see cref="Orientation"/> are
        /// unequal.
        /// </summary>
        /// <param name="left">The left <see cref="OrientedRectangle" />.</param>
        /// <param name="right">The right <see cref="OrientedRectangle" />.</param>
        /// <returns><c>true</c> if left and right argument are unequal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(OrientedRectangle left, OrientedRectangle right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether two instances of <see cref="OrientedRectangle"/> are equal.
        /// </summary>
        /// <param name="other">The other <see cref="OrientedRectangle"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="OrientedRectangle"/> is equal
        /// to the current <see cref="OrientedRectangle"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(OrientedRectangle other)
        {
            return Center.Equals(other.Center) && Radii.Equals(other.Radii) && Orientation.Equals(other.Orientation);
        }

        /// <summary>
        /// Determines whether two instances of <see cref="OrientedRectangle"/> are equal.
        /// </summary>
        /// <param name="obj">The <see cref="OrientedRectangle"/> to compare to.</param>
        /// <returns><c>true</c> if the specified <see cref="OrientedRectangle"/> is equal
        /// to the current <see cref="OrientedRectangle"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is OrientedRectangle other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this object instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Center, Radii, Orientation);
        }

        /// <summary>
        /// Performs an implicit conversion from a <see cref="RectangleF" /> to <see cref="OrientedRectangle" />.
        /// </summary>
        /// <param name="rectangle">The rectangle to convert.</param>
        /// <returns>The resulting <see cref="OrientedRectangle" />.</returns>
        public static explicit operator OrientedRectangle(RectangleF rectangle)
        {
            var radii = new Size2(rectangle.Width * 0.5f, rectangle.Height * 0.5f);
            var centre = new Point2(rectangle.X + radii.Width, rectangle.Y + radii.Height);

            return new OrientedRectangle(centre, radii, Matrix2.Identity);
        }

        public static explicit operator RectangleF(OrientedRectangle orientedRectangle)
        {
            var topLeft = -orientedRectangle.Radii;
            var rectangle = new RectangleF(topLeft, orientedRectangle.Radii * 2);
            var orientation = orientedRectangle.Orientation * Matrix2.CreateTranslation(orientedRectangle.Center);
            return RectangleF.Transform(rectangle, ref orientation);
        }

        /// <summary>
        /// See https://www.flipcode.com/archives/2D_OBB_Intersection.shtml
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool Intersects(OrientedRectangle rectangle, OrientedRectangle other)
        {
            var corners = rectangle.Points;
            var otherCorners = other.Points;
            return IntersectsOneWay(corners, otherCorners) && IntersectsOneWay(otherCorners, corners);

            bool IntersectsOneWay(IReadOnlyList<Vector2> source, IReadOnlyList<Vector2> target)
            {
                var axis = new[]
                    {
                        source[1] - source[0],
                        source[3] - source[0]
                    };
                var origin = new float[2];

                // Make the length of each axis 1/edge length so we know any
                // dot product must be less than 1 to fall within the edge.
                for (var a = 0; a < 2; a++)
                {
                    axis[a] /= axis[a].LengthSquared();
                    origin[a] = source[0].Dot(axis[a]);
                }

                for (var a = 0; a < 2; a++) {

                    var t = target[0].Dot(axis[a]);

                    // Find the extent of box 2 on axis a
                    var tMin = t;
                    var tMax = t;

                    for (var c = 1; c < 4; c++)
                    {
                        t = target[c].Dot(axis[a]);

                        if (t < tMin)
                        {
                            tMin = t;
                        }
                        else if (t > tMax)
                        {
                            tMax = t;
                        }
                    }

                    // We have to subtract off the origin

                    // See if [tMin, tMax] intersects [0, 1]
                    if (tMin > 1 + origin[a] || tMax < origin[a])
                    {
                        // There was no intersection along this dimension;
                        // the boxes cannot possibly overlap.
                        return false;
                    }
                }

                // There was no dimension along which there is no intersection.
                // Therefore the boxes overlap.
                return true;
            }
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this <see cref="OrientedRectangle" />.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this <see cref="OrientedRectangle" />.
        /// </returns>
        public override string ToString()
        {
            return $"Centre: {Center}, Radii: {Radii}, Orientation: {Orientation}";
        }

        internal string DebugDisplayString => ToString();
    }
}
