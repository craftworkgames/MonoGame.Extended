using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Primitives
{
    // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 77 

    /// <summary>
    ///     An axis-aligned, four sided box defined by a centre <see cref="Point2" /> and a radius <see cref="Size2" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         An <see cref="BoundingBox2D" /> is categorized by having its faces oriented in such a way that its
    ///         face normals are at all times parallel with the axes of the given coordinate system.
    ///     </para>
    ///     <para>
    ///         The <see cref="BoundingBox2D" /> of a rotated <see cref="BoundingBox2D" /> will be equivalent or larger in size
    ///         than the original depending on the angle of rotation.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IEquatable{T}" />
    /// <seealso cref="IEquatableByRef{AxisAlignedBoundingBox2}" />
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct BoundingBox2D : IEquatable<BoundingBox2D>,
        IEquatableByRef<BoundingBox2D>
    {
        /// <summary>
        ///     The centre position of this <see cref="BoundingBox2D" />.
        /// </summary>
        public Point2 Centre;

        /// <summary>
        ///     The dimensions of this <see cref="BoundingBox2D" />.
        /// </summary>
        public Size2 Radius;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoundingBox2D" /> structure from the specified centre
        ///     <see cref="Point2" /> and the radius <see cref="Size2" />.
        /// </summary>
        /// <param name="centre">The centre <see cref="Point2" />.</param>
        /// <param name="radius">The radius <see cref="Size2" />.</param>
        public BoundingBox2D(Point2 centre, Size2 radius)
        {
            Centre = centre;
            Radius = radius;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoundingBox2D" /> structure from the specified centre
        ///     position coordinates and the radius dimensions.
        /// </summary>
        /// <param name="x">The centre x-coordinate.</param>
        /// <param name="y">The centre y-coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public BoundingBox2D(float x, float y, float width, float height)
            : this(new Point2(x, y), new Size2(width, height))
        {
        }

        /// <summary>
        ///     Computes the <see cref="BoundingBox2D" /> from a minimum <see cref="Point2" /> and maximum
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="minimum">The minimum point.</param>
        /// <param name="maximum">The maximum point.</param>
        /// <returns>An <see cref="BoundingBox2D" />.</returns>
        public static BoundingBox2D CreateFromMinimumMaximum(Point2 minimum, Point2 maximum)
        {
            var centre = new Point2((maximum.X + minimum.X) * 0.5f, (maximum.Y + minimum.Y) * 0.5f);
            var radius = new Size2((maximum.X - minimum.X) * 0.5f, (maximum.Y - minimum.Y) * 0.5f);
            return new BoundingBox2D(centre, radius);
        }

        /// <summary>
        ///     Computes the <see cref="BoundingBox2D" /> from a list of <see cref="Point2" /> structures.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>An <see cref="BoundingBox2D" />.</returns>
        public static BoundingBox2D CreateFromPoints(IReadOnlyList<Point2> points)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 82-84

            if ((points == null) || (points.Count == 0))
                return new BoundingBox2D();

            var minimum = new Vector2(float.MaxValue);
            var maximum = new Vector2(float.MinValue);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < points.Count; ++index)
            {
                var point = points[index];
                minimum = Point2.Minimum(minimum, point);
                maximum = Point2.Maximum(maximum, point);
            }

            return CreateFromMinimumMaximum(minimum, maximum);
        }

        /// <summary>
        ///     Updates this <see cref="BoundingBox2D" /> from a list of <see cref="Point2" /> structures.
        /// </summary>
        /// <param name="points">The points.</param>
        public void UpdateFromPoints(IReadOnlyList<Point2> points)
        {
            var boundingBox = CreateFromPoints(points);
            Centre = boundingBox.Centre;
            Radius = boundingBox.Radius;
        }

        /// <summary>
        ///     Computes the <see cref="BoundingBox2D" /> from the specified <see cref="BoundingBox2D" /> transformed by the
        ///     specified <see cref="Matrix2D" />.
        /// </summary>
        /// <param name="boundingBox">The bounding box.</param>
        /// <param name="transformMatrix">The transform matrix.</param>
        /// <returns>
        ///     The <see cref="BoundingBox2D" /> from the <paramref name="boundingBox" /> transformed by the
        ///     <paramref name="transformMatrix" />.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         If a transformed <see cref="BoundingBox2D" /> is used for <paramref name="boundingBox" /> then the
        ///         resulting <see cref="BoundingBox2D" /> will have the compounded transformation, which most likely is
        ///         not desired.
        ///     </para>
        /// </remarks>
        public static BoundingBox2D CreateFromTransformedBoundingBox(BoundingBox2D boundingBox,
            ref Matrix2D transformMatrix)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 86-87

            var centre = transformMatrix.Transform(boundingBox.Centre);
            var width = boundingBox.Radius.Height;
            var height = boundingBox.Radius.Width;
            width = width * Math.Abs(transformMatrix.M11) + width * Math.Abs(transformMatrix.M12) +
                    width * Math.Abs(transformMatrix.M31);
            height = height * Math.Abs(transformMatrix.M21) + height * Math.Abs(transformMatrix.M22) +
                     height * Math.Abs(transformMatrix.M32);
            return new BoundingBox2D(centre.X, centre.Y, width, height);
        }

        /// <summary>
        ///     Computes the <see cref="BoundingBox2D" /> that contains the two specified
        ///     <see cref="BoundingBox2D" /> structures.
        /// </summary>
        /// <param name="first">The first bounding box.</param>
        /// <param name="second">The second bounding box.</param>
        /// <returns>
        ///     An <see cref="BoundingBox2D" /> that contains both the <paramref name="first" /> and the
        ///     <paramref name="second" />.
        /// </returns>
        public static BoundingBox2D Union(BoundingBox2D first, BoundingBox2D second)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 6.5; Bounding Volume Hierarchies - Merging Bounding Volumes. pg 267

            var firstMinimum = first.Centre - first.Radius;
            var firstMaximum = first.Centre + first.Radius;
            var secondMinimum = second.Centre - second.Radius;
            var secondMaximum = second.Centre + second.Radius;

            var minimum = Point2.Minimum(firstMinimum, secondMinimum);
            var maximum = Point2.Maximum(firstMaximum, secondMaximum);

            return CreateFromMinimumMaximum(minimum, maximum);
        }

        /// <summary>
        ///     Computes the <see cref="BoundingBox2D" /> that contains both the specified
        ///     <see cref="BoundingBox2D" /> and this
        ///     <see cref="BoundingBox2D" />.
        /// </summary>
        /// <param name="axisAlignedBoundingBox">The bounding box.</param>
        /// <returns>
        ///     An <see cref="BoundingBox2D" /> that contains both the <paramref name="axisAlignedBoundingBox" /> and
        ///     this
        ///     <see cref="BoundingBox2D" />.
        /// </returns>
        public BoundingBox2D Union(BoundingBox2D axisAlignedBoundingBox)
        {
            return Union(this, axisAlignedBoundingBox);
        }

        /// <summary>
        ///     Computes the <see cref="Nullable{BoundingBox2D}" /> that is in common between the two specified
        ///     <see cref="BoundingBox2D" />
        ///     structures.
        /// </summary>
        /// <param name="first">The first bounding box.</param>
        /// <param name="second">The second bounding box.</param>
        /// <returns>
        ///     An <see cref="BoundingBox2D" /> that is in common between both the <paramref name="first" /> and
        ///     the <paramref name="second" />, if they intersect; otherwise, <code>null</code>.
        /// </returns>
        public static BoundingBox2D? Intersection(BoundingBox2D first,
            BoundingBox2D second)
        {
            var firstMinimum = first.Centre - first.Radius;
            var firstMaximum = first.Centre + first.Radius;
            var secondMinimum = second.Centre - second.Radius;
            var secondMaximum = second.Centre + second.Radius;

            var minimum = Point2.Maximum(firstMinimum, secondMinimum);
            var maximum = Point2.Minimum(firstMaximum, secondMaximum);

            if ((maximum.X < minimum.X) || (maximum.Y < minimum.Y))
                return null;

            return CreateFromMinimumMaximum(minimum, maximum);
        }

        /// <summary>
        ///     Computes the <see cref="BoundingBox2D" /> that is in common between the specified
        ///     <see cref="BoundingBox2D" /> and this <see cref="BoundingBox2D" />.
        /// </summary>
        /// <param name="axisAlignedBoundingBox">The bounding box.</param>
        /// <returns>
        ///     An <see cref="BoundingBox2D" /> that is in common between the specified
        ///     <see cref="BoundingBox2D" /> and this <see cref="BoundingBox2D" />.
        /// </returns>
        public BoundingBox2D? Intersection(BoundingBox2D axisAlignedBoundingBox)
        {
            return Intersection(this, axisAlignedBoundingBox);
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="BoundingBox2D" /> structures intersect.
        /// </summary>
        /// <param name="first">The first bounding box.</param>
        /// <param name="second">The second bounding box.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="first" /> intersects with the <see cref="second" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool Intersects(BoundingBox2D first, BoundingBox2D second)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 80

            var distance = first.Centre - second.Centre;
            var radius = first.Radius + second.Radius;
            return (Math.Abs(distance.X) <= radius.Width) && (Math.Abs(distance.Y) <= radius.Height);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="BoundingBox2D" /> intersects with this
        ///     <see cref="BoundingBox2D" />.
        /// </summary>
        /// <param name="boundingBox">The bounding box.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="boundingBox" /> intersects with this
        ///     <see cref="BoundingBox2D" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Intersects(BoundingBox2D boundingBox)
        {
            return Intersects(this, boundingBox);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="BoundingBox2D" /> contains the specified
        ///     <see cref="Point2" />.
        /// </summary>
        /// <param name="boundingBox">The bounding box.</param>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="boundingBox" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public static bool Contains(BoundingBox2D boundingBox, Point2 point)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 78

            var distance = boundingBox.Centre - point;
            var radius = boundingBox.Radius;

            return (Math.Abs(distance.X) <= radius.Width) && (Math.Abs(distance.Y) <= radius.Height);
        }

        /// <summary>
        ///     Determines whether this <see cref="BoundingBox2D" /> contains the specified <see cref="Point2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="BoundingBox2D" /> contains the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Contains(Point2 point)
        {
            return Contains(this, point);
        }

        /// <summary>
        ///     Compares two <see cref="BoundingBox2D" /> structures. The result specifies whether the values of the
        ///     <see cref="Centre" /> and <see cref="Radius" /> fields of the two <see cref="BoundingBox2D" /> structures
        ///     are equal.
        /// </summary>
        /// <param name="first">The first bounding box.</param>
        /// <param name="second">The second bounding box.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Centre" /> and <see cref="Radius" /> fields of the two
        ///     <see cref="BoundingBox2D" /> structures are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(BoundingBox2D first, BoundingBox2D second)
        {
            return first.Equals(ref second);
        }

        /// <summary>
        ///     Indicates whether this <see cref="BoundingBox2D" /> is equal to another
        ///     <see cref="BoundingBox2D" />.
        /// </summary>
        /// <param name="boundingBox">The bounding box.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="BoundingBox2D" /> is equal to the <paramref name="boundingBox" />;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(BoundingBox2D boundingBox)
        {
            return Equals(ref boundingBox);
        }

        /// <summary>
        ///     Indicates whether this <see cref="BoundingBox2D" /> is equal to another <see cref="BoundingBox2D" />.
        /// </summary>
        /// <param name="boundingBox">The ray.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="BoundingBox2D" /> is equal to the <paramref name="boundingBox" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(ref BoundingBox2D boundingBox)
        {
            return (boundingBox.Centre == Centre) && (boundingBox.Radius == Radius);
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="BoundingBox2D" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this  <see cref="BoundingBox2D" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is BoundingBox2D)
                return Equals((BoundingBox2D)obj);
            return false;
        }

        /// <summary>
        ///     Compares two <see cref="BoundingBox2D" /> structures. The result specifies whether the values of the
        ///     <see cref="Centre" /> and <see cref="Radius" /> fields of the two <see cref="BoundingBox2D" /> structures
        ///     are unequal.
        /// </summary>
        /// <param name="first">The first bounding box.</param>
        /// <param name="second">The second bounding box.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Centre" /> and <see cref="Radius" /> fields of the two
        ///     <see cref="BoundingBox2D" /> structures are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(BoundingBox2D first, BoundingBox2D second)
        {
            return !(first == second);
        }

        /// <summary>
        ///     Returns a hash code of this <see cref="BoundingBox2D" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="BoundingBox2D" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Centre.GetHashCode() * 397) ^ Radius.GetHashCode();
            }
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="BoundingBox2D" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="BoundingBox2D" />.
        /// </returns>
        public override string ToString()
        {
            return $"Centre: {Centre}, Radius: {Radius}";
        }

        internal string DebugDisplayString => ToString();
    }
}