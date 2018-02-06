using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    /// <summary>
    ///     A three-dimensional point defined by a 3-tuple of real numbers, (x, y, z).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A point is a position in three-dimensional space, the location of which is described in terms of a
    ///         three-dimensional coordinate system, given by a reference point, called the origin, and three coordinate axes.
    ///     </para>
    ///     <para>
    ///         A common three-dimensional coordinate system is the Cartesian (or rectangular) coordinate system where the
    ///         coordinate axes, conventionally denoted the X axis, Y axis and Z axis, are perpindicular to each other.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IEquatable{T}" />
    /// <seealso cref="IEquatableByRef{Point3}" />
    [DebuggerDisplay("{DebugDisplayString,nq}")]
    public struct Point3 : IEquatable<Point3>, IEquatableByRef<Point3>
    {
        /// <summary>
        ///     Returns a <see cref="Point3" /> with <see cref="X" /> <see cref="Y" /> and <see cref="Z" /> equal to <c>0.0f</c>.
        /// </summary>
        public static readonly Point3 Zero = new Point3();

        /// <summary>
        ///     Returns a <see cref="Point3" /> with <see cref="X" /> <see cref="Y" />  and <see cref="Z" /> set to not a number.
        /// </summary>
        public static readonly Point3 NaN = new Point3(float.NaN, float.NaN, float.NaN);

        /// <summary>
        ///     The x-coordinate of this <see cref="Point3" />.
        /// </summary>
        public float X;

        /// <summary>
        ///     The y-coordinate of this <see cref="Point3" />.
        /// </summary>
        public float Y;

        /// <summary>
        ///     The z-coordinate of this <see cref="Point3" />.
        /// </summary>
        public float Z;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Point3" /> structure from the specified coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="z">The z-coordinate.</param>
        public Point3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        ///     Compares two <see cref="Point3" /> structures. The result specifies
        ///     whether the values of the <see cref="X" /> <see cref="Y" /> and <see cref="Z" />
        ///     fields of the two <see cref="Point3" />
        ///     structures are equal.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="X" /> <see cref="Y" /> and <see cref="Z" />
        ///     fields of the two <see cref="Point3" />
        ///     structures are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Point3 first, Point3 second)
        {
            return first.Equals(ref second);
        }

        /// <summary>
        ///     Indicates whether this <see cref="Point3" /> is equal to another <see cref="Point3" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="Point3" /> is equal to the <paramref name="point" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(Point3 point)
        {
            return Equals(ref point);
        }

        /// <summary>
        ///     Indicates whether this <see cref="Point3" /> is equal to another <see cref="Point3" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="Point3" /> is equal to the <paramref name="point" /> parameter; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(ref Point3 point)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return (point.X == X) && (point.Y == Y) && (point.Z == Z);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="Point3" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this  <see cref="Point3" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Point3)
                return Equals((Point3) obj);
            return false;
        }

        /// <summary>
        ///     Compares two <see cref="Point3" /> structures. The result specifies
        ///     whether the values of the <see cref="X" /> <see cref="Y" /> or <see cref="Z" />
        ///     fields of the two <see cref="Point3" />
        ///     structures are unequal.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="X" /> <see cref="Y" /> or <see cref="Z" />
        ///     fields of the two <see cref="Point3" />
        ///     structures are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Point3 first, Point3 second)
        {
            return !(first == second);
        }

        /// <summary>
        ///     Calculates the <see cref="Point3" /> representing the addition of a <see cref="Point3" /> and a
        ///     <see cref="Vector2" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="vector">The vector.</param>
        /// <returns>
        ///     The <see cref="Point3" /> representing the addition of a <see cref="Point3" /> and a <see cref="Vector3" />.
        /// </returns>
        public static Point3 operator +(Point3 point, Vector3 vector)
        {
            return Add(point, vector);
        }

        /// <summary>
        ///     Calculates the <see cref="Point3" /> representing the addition of a <see cref="Point3" /> and a
        ///     <see cref="Vector3" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="vector">The vector.</param>
        /// <returns>
        ///     The <see cref="Point3" /> representing the addition of a <see cref="Point3" /> and a <see cref="Vector3" />.
        /// </returns>
        public static Point3 Add(Point3 point, Vector3 vector)
        {
            Point3 p;
            p.X = point.X + vector.X;
            p.Y = point.Y + vector.Y;
            p.Z = point.Z + vector.Z;
            return p;
        }

        /// <summary>
        ///     Calculates the <see cref="Point3" /> representing the subtraction of a <see cref="Point3" /> and a
        ///     <see cref="Vector3" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="vector">The vector.</param>
        /// <returns>
        ///     The <see cref="Point3" /> representing the substraction of a <see cref="Point3" /> and a <see cref="Vector3" />.
        /// </returns>
        public static Point3 operator -(Point3 point, Vector3 vector)
        {
            return Subtract(point, vector);
        }

        /// <summary>
        ///     Calculates the <see cref="Point3" /> representing the addition of a <see cref="Point3" /> and a
        ///     <see cref="Vector3" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="vector">The vector.</param>
        /// <returns>
        ///     The <see cref="Point3" /> representing the substraction of a <see cref="Point3" /> and a <see cref="Vector3" />.
        /// </returns>
        public static Point3 Subtract(Point3 point, Vector3 vector)
        {
            Point3 p;
            p.X = point.X - vector.X;
            p.Y = point.Y - vector.Y;
            p.Z = point.Z - vector.Z;
            return p;
        }

        /// <summary>
        ///     Calculates the <see cref="Vector3" /> representing the displacement of two <see cref="Point3" /> structures.
        /// </summary>
        /// <param name="point2">The second point.</param>
        /// <param name="point1">The first point.</param>
        /// <returns>
        ///     The <see cref="Vector3" /> representing the displacement of two <see cref="Point3" /> structures.
        /// </returns>
        public static Vector3 operator -(Point3 point1, Point3 point2)
        {
            return Displacement(point1, point2);
        }

        /// <summary>
        ///     Calculates the <see cref="Vector3" /> representing the displacement of two <see cref="Point3" /> structures.
        /// </summary>
        /// <param name="point2">The second point.</param>
        /// <param name="point1">The first point.</param>
        /// <returns>
        ///     The <see cref="Vector3" /> representing the displacement of two <see cref="Point3" /> structures.
        /// </returns>
        public static Vector3 Displacement(Point3 point2, Point3 point1)
        {
            Vector3 vector;
            vector.X = point2.X - point1.X;
            vector.Y = point2.Y - point1.Y;
            vector.Z = point2.Z - point1.Z;
            return vector;
        }

        /// <summary>
        ///     Translates a <see cref='Point3' /> by a given <see cref='Size3' />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Point3 operator +(Point3 point, Size3 size)
        {
            return Add(point, size);
        }

        /// <summary>
        ///     Translates a <see cref='Point3' /> by a given <see cref='Size3' />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Point3 Add(Point3 point, Size3 size)
        {
            return new Point3(point.X + size.Width, point.Y + size.Height, point.Z + size.Depth);
        }

        /// <summary>
        ///     Translates a <see cref='Point3' /> by the negative of a given <see cref='Size3' />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Point3 operator -(Point3 point, Size3 size)
        {
            return Subtract(point, size);
        }

        /// <summary>
        ///     Translates a <see cref='Point3' /> by the negative of a given <see cref='Size3' /> .
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Point3 Subtract(Point3 point, Size3 size)
        {
            return new Point3(point.X - size.Width, point.Y - size.Height, point.Z - size.Depth);
        }

        /// <summary>
        ///     Returns a hash code of this <see cref="Point3" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="Point3" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                hash = hash * 23 + Z.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        ///     Calculates the <see cref="Point3" /> that contains the minimal coordinate values from two <see cref="Point3" />
        ///     structures.
        ///     structures.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <returns>
        ///     The the <see cref="Point3" /> that contains the minimal coordinate values from two <see cref="Point3" />
        ///     structures.
        /// </returns>
        public static Point3 Minimum(Point3 first, Point3 second)
        {
            return new Point3(first.X < second.X ? first.X : second.X,
                first.Y < second.Y ? first.Y : second.Y,
                first.Z < second.Z ? first.Z : second.Z);
        }

        /// <summary>
        ///     Calculates the <see cref="Point3" /> that contains the maximal coordinate values from two <see cref="Point3" />
        ///     structures.
        ///     structures.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <returns>
        ///     The the <see cref="Point3" /> that contains the maximal coordinate values from two <see cref="Point3" />
        ///     structures.
        /// </returns>
        public static Point3 Maximum(Point3 first, Point3 second)
        {
            return new Point3(first.X > second.X ? first.X : second.X,
                first.Y > second.Y ? first.Y : second.Y,
                first.Z > second.Z ? first.Z : second.Z);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Point3" /> to a position <see cref="Vector3" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     The resulting <see cref="Vector3" />.
        /// </returns>
        public static implicit operator Vector3(Point3 point)
        {
            return new Vector3(point.X, point.Y, point.Z);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Vector3" /> to a position <see cref="Point3" />.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>
        ///     The resulting <see cref="Point3" />.
        /// </returns>
        public static implicit operator Point3(Vector3 vector)
        {
            return new Point3(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="Point3" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="Point3" />.
        /// </returns>
        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        internal string DebugDisplayString => ToString();
    }
}