using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    /// <summary>
    ///     A three dimensional size defined by two real numbers, a width a height and a depth.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A size is a subspace of three-dimensional space, the area of which is described in terms of a three-dimensional
    ///         coordinate system, given by a reference point and three coordinate axes.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IEquatable{T}" />
    /// <seealso cref="IEquatableByRef{Size3}" />
    public struct Size3 : IEquatable<Size3>, IEquatableByRef<Size3>
    {
        /// <summary>
        ///     Returns a <see cref="Size3" /> with <see cref="Width" />  <see cref="Height" /> and <see cref="Depth" /> equal to <c>0.0f</c>.
        /// </summary>
        public static readonly Size3 Empty = new Size3();

        /// <summary>
        ///     The horizontal component of this <see cref="Size3" />.
        /// </summary>
        public float Width;

        /// <summary>
        ///     The vertical component of this <see cref="Size3" />.
        /// </summary>
        public float Height;

        /// <summary>
        ///     The vertical component of this <see cref="Size3" />.
        /// </summary>
        public float Depth;

        /// <summary>
        ///     Gets a value that indicates whether this <see cref="Size3" /> is empty.
        /// </summary>
        // ReSharper disable CompareOfFloatsByEqualityOperator
        public bool IsEmpty => (Width == 0) && (Height == 0) && (Depth == 0);

        // ReSharper restore CompareOfFloatsByEqualityOperator

        /// <summary>
        ///     Initializes a new instance of the <see cref="Size3" /> structure from the specified dimensions.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        public Size3(float width, float height, float depth)
        {
            Width = width;
            Height = height;
            Depth = depth;
        }

        /// <summary>
        ///     Compares two <see cref="Size3" /> structures. The result specifies
        ///     whether the values of the <see cref="Width" /> <see cref="Height" /> and <see cref="Depth" />
        ///     fields of the two <see cref="Point3" /> structures are equal.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Width" /> <see cref="Height" /> and <see cref="Depth" />
        ///     fields of the two <see cref="Point3" /> structures are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Size3 first, Size3 second)
        {
            return first.Equals(ref second);
        }

        /// <summary>
        ///     Indicates whether this <see cref="Size3" /> is equal to another <see cref="Size3" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="Point3" /> is equal to the <paramref name="size" /> parameter; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(Size3 size)
        {
            return Equals(ref size);
        }

        /// <summary>
        ///     Indicates whether this <see cref="Size3" /> is equal to another <see cref="Size3" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     <c>true</c> if this <see cref="Point3" /> is equal to the <paramref name="size" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(ref Size3 size)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return (Width == size.Width) && (Height == size.Height) && (Depth == size.Depth);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        ///     Returns a value indicating whether this <see cref="Size3" /> is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to make the comparison with.</param>
        /// <returns>
        ///     <c>true</c> if this  <see cref="Size3" /> is equal to <paramref name="obj" />; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Size3)
                return Equals((Size3) obj);
            return false;
        }

        /// <summary>
        ///     Compares two <see cref="Size3" /> structures. The result specifies
        ///     whether the values of the <see cref="Width" /> <see cref="Height" /> or <see cref="Depth" />
        ///     fields of the two <see cref="Size3" /> structures are unequal.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Width" /> <see cref="Height" /> or <see cref="Depth" />
        ///     fields of the two <see cref="Size3" /> structures are unequal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Size3 first, Size3 second)
        {
            return !(first == second);
        }

        /// <summary>
        ///     Calculates the <see cref="Size3" /> representing the vector addition of two <see cref="Size3" /> structures as if
        ///     they were <see cref="Vector3" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="Size3" /> representing the vector addition of two <see cref="Size3" /> structures as if they
        ///     were <see cref="Vector3" /> structures.
        /// </returns>
        public static Size3 operator +(Size3 first, Size3 second)
        {
            return Add(first, second);
        }

        /// <summary>
        ///     Calculates the <see cref="Size3" /> representing the vector addition of two <see cref="Size3" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="Size3" /> representing the vector addition of two <see cref="Size3" /> structures.
        /// </returns>
        public static Size3 Add(Size3 first, Size3 second)
        {
            Size3 size;
            size.Width = first.Width + second.Width;
            size.Height = first.Height + second.Height;
            size.Depth = first.Depth + second.Depth;
            return size;
        }

        /// <summary>
        /// Calculates the <see cref="Size3" /> representing the vector subtraction of two <see cref="Size3" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="Size3" /> representing the vector subtraction of two <see cref="Size3" /> structures.
        /// </returns>
        public static Size3 operator -(Size3 first, Size3 second)
        {
            return Subtract(first, second);
        }

        public static Size3 operator /(Size3 size, float value)
        {
            return new Size3(size.Width / value, size.Height / value, size.Depth / value);
        }

        public static Size3 operator *(Size3 size, float value)
        {
            return new Size3(size.Width * value, size.Height * value, size.Depth * value);
        }

        /// <summary>
        ///     Calculates the <see cref="Size3" /> representing the vector subtraction of two <see cref="Size3" /> structures.
        /// </summary>
        /// <param name="first">The first size.</param>
        /// <param name="second">The second size.</param>
        /// <returns>
        ///     The <see cref="Size3" /> representing the vector subtraction of two <see cref="Size3" /> structures.
        /// </returns>
        public static Size3 Subtract(Size3 first, Size3 second)
        {
            Size3 size;
            size.Width = first.Width - second.Width;
            size.Height = first.Height - second.Height;
            size.Depth = first.Depth - second.Depth;
            return size;
        }

        /// <summary>
        ///     Returns a hash code of this <see cref="Size3" /> suitable for use in hashing algorithms and data
        ///     structures like a hash table.
        /// </summary>
        /// <returns>
        ///     A hash code of this <see cref="Point3" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + Width.GetHashCode();
                    hash = hash * 23 + Height.GetHashCode();
                    hash = hash * 23 + Depth.GetHashCode();
                    return hash;
                }
            }
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Point3" /> to a <see cref="Size3" />.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///     The resulting <see cref="Size3" />.
        /// </returns>
        public static implicit operator Size3(Point3 point)
        {
            return new Size3(point.X, point.Y, point.Z);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Point3" /> to a <see cref="Size3" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The resulting <see cref="Point3" />.
        /// </returns>
        public static implicit operator Point3(Size3 size)
        {
            return new Point3(size.Width, size.Height, size.Depth);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Size3" /> to a <see cref="Vector3" />.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>
        ///     The resulting <see cref="Vector3" />.
        /// </returns>
        public static implicit operator Vector3(Size3 size)
        {
            return new Vector3(size.Width, size.Height, size.Depth);
        }

        /// <summary>
        ///     Performs an implicit conversion from a <see cref="Vector3" /> to a <see cref="Size3" />.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>
        ///     The resulting <see cref="Size3" />.
        /// </returns>
        public static implicit operator Size3(Vector3 vector)
        {
            return new Size3(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this <see cref="Size3" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this <see cref="Size3" />.
        /// </returns>
        public override string ToString()
        {
            return $"Width: {Width}, Height: {Height}, Depth: {Depth}";
        }

        internal string DebugDisplayString => ToString();
    }
}