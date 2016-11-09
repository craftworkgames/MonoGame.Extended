using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Primitives
{
    // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 77 

    /// <summary>
    ///     An axis-aligned, four sided, two dimensional box defined by a centre <see cref="Point2" /> and a radii
    ///     <see cref="Vector2" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         An <see cref="BoundingRectangle" /> is categorized by having its faces oriented in such a way that its
    ///         face normals are at all times parallel with the axes of the given coordinate system.
    ///     </para>
    ///     <para>
    ///         The <see cref="BoundingRectangle" /> of a rotated <see cref="BoundingRectangle" /> will be equivalent or larger
    ///         in size
    ///         than the original depending on the angle of rotation.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IEquatable{T}" />
    /// <seealso cref="IEquatableByRef{T}" />
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct BoundingRectangle : IEquatable<BoundingRectangle>,
        IEquatableByRef<BoundingRectangle>
    {
        /// <summary>
        ///     The centre position of this <see cref="BoundingRectangle" />.
        /// </summary>
        public Point2 Centre;

        /// <summary>
        ///     The distance from the <see cref="Centre" /> point along both axes to any point on the boundary of this
        ///     <see cref="BoundingRectangle" />.
        /// </summary>
        public Vector2 Radii;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoundingRectangle" /> structure from the specified centre
        ///     <see cref="Point2" /> and the radii <see cref="Size2" />.
        /// </summary>
        /// <param name="centre">The centre <see cref="Point2" />.</param>
        /// <param name="radii">The radii <see cref="Vector2" />.</param>
        public BoundingRectangle(Point2 centre, Vector2 radii)
        {
            Centre = centre;
            Radii = radii;
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> from a minimum <see cref="Point2" /> and maximum
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="minimum">The minimum point.</param>
        /// <param name="maximum">The maximum point.</param>
        /// <returns>An <see cref="BoundingRectangle" />.</returns>
        public static BoundingRectangle CreateFrom(Point2 minimum, Point2 maximum)
        {
            var centre = new Point2((maximum.X + minimum.X)*0.5f, (maximum.Y + minimum.Y)*0.5f);
            var radii = new Vector2((maximum.X - minimum.X)*0.5f, (maximum.Y - minimum.Y)*0.5f);
            return new BoundingRectangle(centre, radii);
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> from a list of <see cref="Point2" /> structures.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>An <see cref="BoundingRectangle" />.</returns>
        public static BoundingRectangle CreateFrom(IReadOnlyList<Point2> points)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 82-84

            if ((points == null) || (points.Count == 0))
                return new BoundingRectangle();

            var minimum = new Vector2(float.MaxValue);
            var maximum = new Vector2(float.MinValue);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < points.Count; ++index)
            {
                var point = points[index];
                minimum = Point2.Minimum(minimum, point);
                maximum = Point2.Maximum(maximum, point);
            }

            return CreateFrom(minimum, maximum);
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> from the specified <see cref="BoundingRectangle" /> transformed by
        ///     the
        ///     specified <see cref="Matrix2D" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <param name="transformMatrix">The transform matrix.</param>
        /// <returns>
        ///     The <see cref="BoundingRectangle" /> from the <paramref name="boundingRectangle" /> transformed by the
        ///     <paramref name="transformMatrix" />.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If a transformed <see cref="BoundingRectangle" /> is used for <paramref name="boundingRectangle" /> then the
        ///         resulting <see cref="BoundingRectangle" /> will have the compounded transformation, which most likely is
        ///         not desired.
        ///     </para>
        /// </remarks>
        public static BoundingRectangle CreateFrom(BoundingRectangle boundingRectangle,
            ref Matrix2D transformMatrix)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 86-87

            var centre = transformMatrix.Transform(boundingRectangle.Centre);
            var radii = boundingRectangle.Radii;
            radii.X = radii.X*Math.Abs(transformMatrix.M11) + radii.X*Math.Abs(transformMatrix.M12) +
                      radii.X*Math.Abs(transformMatrix.M31);
            radii.Y = radii.Y*Math.Abs(transformMatrix.M21) + radii.Y*Math.Abs(transformMatrix.M22) +
                      radii.Y*Math.Abs(transformMatrix.M32);
            return new BoundingRectangle(centre, radii);
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> that contains the two specified
        ///     <see cref="BoundingRectangle" /> structures.
        /// </summary>
        /// <param name="first">The first bounding rectangle.</param>
        /// <param name="second">The second bounding rectangle.</param>
        /// <returns>
        ///     An <see cref="BoundingRectangle" /> that contains both the <paramref name="first" /> and the
        ///     <paramref name="second" />.
        /// </returns>
        public static BoundingRectangle Union(BoundingRectangle first, BoundingRectangle second)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 6.5; Bounding Volume Hierarchies - Merging Bounding Volumes. pg 267

            var firstMinimum = first.Centre - first.Radii;
            var firstMaximum = first.Centre + first.Radii;
            var secondMinimum = second.Centre - second.Radii;
            var secondMaximum = second.Centre + second.Radii;

            var minimum = Point2.Minimum(firstMinimum, secondMinimum);
            var maximum = Point2.Maximum(firstMaximum, secondMaximum);

            return CreateFrom(minimum, maximum);
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> that contains both the specified
        ///     <see cref="BoundingRectangle" /> and this <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <returns>
        ///     An <see cref="BoundingRectangle" /> that contains both the <paramref name="boundingRectangle" /> and
        ///     this
        ///     <see cref="BoundingRectangle" />.
        /// </returns>
        public BoundingRectangle Union(BoundingRectangle boundingRectangle)
        {
            return Union(this, boundingRectangle);
        }

        /// <summary>
        ///     Computes the <see cref="Nullable{BoundingRectangle}" /> that is in common between the two specified
        ///     <see cref="BoundingRectangle" /> structures.
        /// </summary>
        /// <param name="first">The first bounding rectangle.</param>
        /// <param name="second">The second bounding rectangle.</param>
        /// <returns>
        ///     An <see cref="BoundingRectangle" /> that is in common between both the <paramref name="first" /> and
        ///     the <paramref name="second" />, if they intersect; otherwise, <code>null</code>.
        /// </returns>
        public static BoundingRectangle? Intersection(BoundingRectangle first,
            BoundingRectangle second)
        {
            var firstMinimum = first.Centre - first.Radii;
            var firstMaximum = first.Centre + first.Radii;
            var secondMinimum = second.Centre - second.Radii;
            var secondMaximum = second.Centre + second.Radii;

            var minimum = Point2.Maximum(firstMinimum, secondMinimum);
            var maximum = Point2.Minimum(firstMaximum, secondMaximum);

            if ((maximum.X < minimum.X) || (maximum.Y < minimum.Y))
                return null;

            return CreateFrom(minimum, maximum);
        }

        /// <summary>
        ///     Updates this <see cref="BoundingRectangle" /> from a list of <see cref="Point2" /> structures.
        /// </summary>
        /// <param name="points">The points.</param>
        public void UpdateFromPoints(IReadOnlyList<Point2> points)
        {
            var boundingRectangle = CreateFrom(points);
            Centre = boundingRectangle.Centre;
            Radii = boundingRectangle.Radii;
        }

        /// <summary>
        ///     Computes the <see cref="BoundingRectangle" /> that is in common between the specified
        ///     <see cref="BoundingRectangle" /> and this <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <returns>
        ///     An <see cref="BoundingRectangle" /> that is in common between the specified
        ///     <see cref="BoundingRectangle" /> and this <see cref="BoundingRectangle" />.
        /// </returns>
        public BoundingRectangle? Intersection(BoundingRectangle boundingRectangle)
        {
            return Intersection(this, boundingRectangle);
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="BoundingRectangle" /> structures intersect.
        /// </summary>
        /// <param name="first">The first bounding rectangle.</param>
        /// <param name="second">The second bounding rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="first" /> intersects with the <see cref="second" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool Intersects(BoundingRectangle first, BoundingRectangle second)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 80

            var distance = first.Centre - second.Centre;
            var radii = first.Radii + second.Radii;
            return (Math.Abs(distance.X) <= radii.X) && (Math.Abs(distance.Y) <= radii.Y);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="BoundingRectangle" /> intersects with this
        ///     <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="boundingRectangle" /> intersects with this
        ///     <see cref="BoundingRectangle" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Intersects(BoundingRectangle boundingRectangle)
        {
            return Intersects(this, boundingRectangle);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="BoundingRectangle" /> contains the specified
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="boundingRectangle" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool Contains(BoundingRectangle boundingRectangle, Point2 point)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 78

            var distance = boundingRectangle.Centre - point;
            var radii = boundingRectangle.Radii;

            return (Math.Abs(distance.X) <= radii.X) && (Math.Abs(distance.Y) <= radii.Y);
        }

        /// <summary>
        ///     Determines whether this <see cref="BoundingRectangle" /> contains the specified <see cref="Point2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="BoundingRectangle" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Contains(Point2 point)
        {
            return Contains(this, point);
        }


        /// <summary>
        ///     Computes the closest <see cref="Point2" /> on this <see cref="BoundingRectangle" /> to a specified
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The closest <see cref="Point2" /> on this <see cref="BoundingRectangle" /> to the <paramref name="point" />.</returns>
        public Point2 ClosestPointTo(Point2 point)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 5.1.2; Basic Primitive Tests - Closest-point Computations. pg 130-131

            var minimum = Centre - Radii;
            var maximum = Centre + Radii;
            var result = point;

            // For each coordinate axis, if the point coordinate value is outside box, clamp it to the box, else keep it as is
            if (result.X < minimum.X)
                result.X = minimum.X;
            else if (result.X > maximum.X)
                result.X = maximum.X;

            if (result.Y < minimum.Y)
                result.Y = minimum.Y;
            else if (result.Y > maximum.Y)
                result.Y = maximum.Y;

            return result;
        }

        /// <summary>
        ///     Compares two <see cref="BoundingRectangle" /> structures. The result specifies whether the values of the
        ///     <see cref="Centre" /> and <see cref="Radii" /> fields of the two <see cref="BoundingRectangle" /> structures
        ///     are equal.
        /// </summary>
        /// <param name="first">The first bounding rectangle.</param>
        /// <param name="second">The second bounding rectangle.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Centre" /> and <see cref="Radii" /> fields of the two
        ///     <see cref="BoundingRectangle" /> structures are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(BoundingRectangle first, BoundingRectangle second)
        {
            return first.Equals(ref second);
        }

        /// <summary>
        ///     Indicates whether this <see cref="BoundingRectangle" /> is equal to another
        ///     <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="BoundingRectangle" /> is equal to the <paramref name="boundingRectangle" />;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(BoundingRectangle boundingRectangle)
        {
            return Equals(ref boundingRectangle);
        }

        /// <summary>
        ///     Indicates whether this <see cref="BoundingRectangle" /> is equal to another <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="BoundingRectangle" /> is equal to the <paramref name="boundingRectangle" />;
        ///     otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(ref BoundingRectangle boundingRectangle)
        {
            return (boundingRectangle.Centre == Centre) && (boundingRectangle.Radii == Radii);
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="BoundingRectangle" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this  <see cref="BoundingRectangle" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is BoundingRectangle)
                return Equals((BoundingRectangle) obj);
            return false;
        }

        /// <summary>
        ///     Compares two <see cref="BoundingRectangle" /> structures. The result specifies whether the values of the
        ///     <see cref="Centre" /> and <see cref="Radii" /> fields of the two <see cref="BoundingRectangle" /> structures
        ///     are unequal.
        /// </summary>
        /// <param name="first">The first bounding box.</param>
        /// <param name="second">The second bounding box.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Centre" /> and <see cref="Radii" /> fields of the two
        ///     <see cref="BoundingRectangle" /> structures are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(BoundingRectangle first, BoundingRectangle second)
        {
            return !(first == second);
        }

        /// <summary>
        ///     Returns a hash code of this <see cref="BoundingRectangle" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="BoundingRectangle" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Centre.GetHashCode()*397) ^ Radii.GetHashCode();
            }
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Rectangle" /> to a <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="rectangle">The rectangle.</param>
        /// <returns>
        ///     The resulting <see cref="BoundingRectangle" />.
        /// </returns>
        public static implicit operator BoundingRectangle(Rectangle rectangle)
        {
            var radii = new Size2(rectangle.Width/2f, rectangle.Height/2f);
            var centre = new Point2(rectangle.X + radii.Width, rectangle.Y + radii.Height);
            return new BoundingRectangle(centre, radii);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="BoundingRectangle" /> to a <see cref="Rectangle" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <returns>
        ///     The resulting <see cref="Rectangle" />.
        /// </returns>
        public static implicit operator Rectangle(BoundingRectangle boundingRectangle)
        {
            var minimum = boundingRectangle.Centre - boundingRectangle.Radii;
            return new Rectangle((int) minimum.X, (int) minimum.Y, (int) boundingRectangle.Radii.X*2,
                (int) boundingRectangle.Radii.Y*2);
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="BoundingRectangle" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="BoundingRectangle" />.
        /// </returns>
        public override string ToString()
        {
            return $"Centre: {Centre}, Radii: {Radii}";
        }

        internal string DebugDisplayString => ToString();
    }
}