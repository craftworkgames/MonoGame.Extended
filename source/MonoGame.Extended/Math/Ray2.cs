using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 3.5; A Math and Geometry Primer - Lines, Rays, and Segments. pg 53-54
    /// <summary>
    ///     A two dimensional ray defined by a starting <see cref="Vector2" /> and a direction <see cref="Vector2" />.
    /// </summary>
    /// <seealso cref="IEquatable{T}" />
    /// <seealso cref="IEquatableByRef{Ray2}" />
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct Ray2 : IEquatable<Ray2>, IEquatableByRef<Ray2>
    {
        /// <summary>
        ///     The starting <see cref="Vector2" /> of this <see cref="Ray2" />.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        ///     The direction <see cref="Vector2" /> of this <see cref="Ray2" />.
        /// </summary>
        public Vector2 Direction;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ray2" /> structure from the specified position and direction.
        /// </summary>
        /// <param name="position">The starting point.</param>
        /// <param name="direction">The direction vector.</param>
        public Ray2(Vector2 position, Vector2 direction)
        {
            Position = position;
            Direction = direction;
        }

        /// <summary>
        ///     Determines whether this <see cref="Ray2" /> intersects with a specified <see cref="BoundingRectangle" />.
        /// </summary>
        /// <param name="boundingRectangle">The bounding rectangle.</param>
        /// <param name="rayNearDistance">
        ///     When this method returns, contains the distance along the ray to the first intersection
        ///     point with the <paramref name="boundingRectangle" />, if an intersection was found; otherwise,
        ///     <see cref="float.NaN" />.
        ///     This parameter is passed uninitialized.
        /// </param>
        /// <param name="rayFarDistance">
        ///     When this method returns, contains the distance along the ray to the second intersection
        ///     point with the <paramref name="boundingRectangle" />, if an intersection was found; otherwise,
        ///     <see cref="float.NaN" />.
        ///     This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     <c>true</c> if this <see cref="Ray2" /> intersects with <paramref name="boundingRectangle" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Intersects(BoundingRectangle boundingRectangle, out float rayNearDistance, out float rayFarDistance)
        {
            // Real-Time Collision Detection, Christer Ericson, 2005. Chapter 5.3; Basic Primitive Tests - Intersecting Lines, Rays, and (Directed Segments). pg 179-181

            var minimum = boundingRectangle.Center - boundingRectangle.HalfExtents;
            var maximum = boundingRectangle.Center + boundingRectangle.HalfExtents;

            // Set to the smallest possible value so the algorithm can find the first hit along the ray
            var minimumDistanceAlongRay = float.MinValue;
            // Set to the maximum possible value so the algorithm can find the last hit along the ray
            var maximumDistanceAlongRay = float.MaxValue;

            // For all relevant slabs which in this case is two.

            // The first, horizontal, slab.
            if (!PrimitivesHelper.IntersectsSlab(Position.X, Direction.X, minimum.X, maximum.X,
                ref minimumDistanceAlongRay,
                ref maximumDistanceAlongRay))
            {
                rayNearDistance = rayFarDistance = float.NaN;
                return false;
            }

            // The second, vertical, slab.
            if (!PrimitivesHelper.IntersectsSlab(Position.Y, Direction.Y, minimum.Y, maximum.Y,
                ref minimumDistanceAlongRay,
                ref maximumDistanceAlongRay))
            {
                rayNearDistance = rayFarDistance = float.NaN;
                return false;
            }

            // Ray intersects the 2 slabs.
            rayNearDistance = minimumDistanceAlongRay < 0 ? 0 : minimumDistanceAlongRay;
            rayFarDistance = maximumDistanceAlongRay;
            return true;
        }

        /// <summary>
        ///     Compares two <see cref="Ray2" /> structures. The result specifies whether the values of the
        ///     <see cref="Position" />
        ///     and <see cref="Direction" /> fields of the two <see cref="Ray2" /> structures are equal.
        /// </summary>
        /// <param name="first">The first ray.</param>
        /// <param name="second">The second ray.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Position" /> and <see cref="Direction" />
        ///     fields of the two <see cref="Ray2" />
        ///     structures are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Ray2 first, Ray2 second)
        {
            return first.Equals(ref second);
        }

        /// <summary>
        ///     Indicates whether this <see cref="Ray2" /> is equal to another <see cref="Ray2" />.
        /// </summary>
        /// <param name="ray">The ray.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="Ray2" /> is equal to the <paramref name="ray" /> parameter; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(Ray2 ray)
        {
            return Equals(ref ray);
        }

        /// <summary>
        ///     Indicates whether this <see cref="Ray2" /> is equal to another <see cref="Ray2" />.
        /// </summary>
        /// <param name="ray">The ray.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="Ray2" /> is equal to the <paramref name="ray" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(ref Ray2 ray)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return (ray.Position == Position) && (ray.Direction == Direction);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="Ray2" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this  <see cref="Ray2" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Ray2)
                return Equals((Ray2) obj);
            return false;
        }

        /// <summary>
        ///     Compares two <see cref="Ray2" /> structures. The result specifies whether the values of the
        ///     <see cref='Position' />
        ///     and <see cref="Direction" /> fields of the two <see cref="Ray2" /> structures are unequal.
        /// </summary>
        /// <param name="first">The first ray.</param>
        /// <param name="second">The second ray.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Position" /> and <see cref="Direction" />
        ///     fields of the two <see cref="Ray2" />
        ///     structures are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Ray2 first, Ray2 second)
        {
            return !(first == second);
        }

        /// <summary>
        ///     Returns a hash code of this <see cref="Ray2" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="Ray2" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Position.GetHashCode()*397) ^ Direction.GetHashCode();
            }
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="Ray2" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="Ray2" />.
        /// </returns>
        public override string ToString()
        {
            return $"Position: {Position}, Direction: {Direction}";
        }

        internal string DebugDisplayString => ToString();
    }
}
