using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Particles
{
    /// <summary>
    /// An immutable data structure representing a directed fixed axis.
    /// </summary>
    public struct Axis : IEquatable<Axis>
    {
        public float X { get; }
        public float Y { get; }

        public float Angle => (float)Math.Atan2(Y, X);

        /// <summary>
        /// Initializes a new instance of the <see cref="Axis"/> structure.
        /// </summary>
        /// <param name="x">The X component of the unit vector representing the axis.</param>
        /// <param name="y">The Y component of the unit vector representing the axis.</param>
        public Axis(float x, float y)
        {
            var length = (float)Math.Sqrt(x * x + y * y);

            X = x / length;
            Y = y / length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Axis"/> structure.
        /// </summary>
        /// <param name="angle">The angle of the axis in radians.</param>
        public Axis(float angle) : this()
        {
            var x = (float)Math.Cos(angle);
            var y = (float)Math.Sin(angle);

            var length = (float)Math.Sqrt(x * x + y * y);

            X = x / length;
            Y = y / length;
        }

        /// <summary>
        /// Gets a directed axis which points to the left.
        /// </summary>
        public static Axis Left => new Axis(-1f, 0f);

        /// <summary>
        /// Gets a directed axis which points up.
        /// </summary>
        public static Axis Up => new Axis(0f, -1f);

        /// <summary>
        /// Gets a directed axis which points to the right.
        /// </summary>
        public static Axis Right => new Axis(1f, 0f);

        /// <summary>
        /// Gets a directed axis which points down.
        /// </summary>
        public static Axis Down => new Axis(0f, 1f);

        /// <summary>
        /// Multiplies the fixed axis by a magnitude value resulting in a directed vector.
        /// </summary>
        /// <param name="magnitude">The magnitude of the vector.</param>
        /// <returns>A directed vector.</returns>
        public Vector2 Multiply(float magnitude)
        {
            return ToVector2(magnitude);
        }

        public Vector2 ToVector2(float magnitude)
        {
            return new Vector2(X * magnitude, Y * magnitude);
        }

        public Vector2 ToVector2()
        {
            return ToVector2(1.0f);
        }

        /// <summary>
        /// Copies the X and Y components of the axis to the specified memory location.
        /// </summary>
        /// <param name="destination">The memory location to copy the axis to.</param>
        public unsafe void CopyTo(float* destination)
        {
            destination[0] = X;
            destination[1] = Y;
        }

        /// <summary>
        /// Destructures the axis, exposing the individual X and Y components.
        /// </summary>
        public void Destructure(out float x, out float y)
        {
            x = X;
            y = Y;
        }

        /// <summary>
        /// Exposes the individual X and Y components of the axis to the specified matching function.
        /// </summary>
        /// <param name="callback">The function which matches the individual X and Y components.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the value passed to the <paramref name="callback"/> parameter is <c>null</c>.
        /// </exception>
        public void Match(Action<float, float> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            callback(X, Y);
        }

        /// <summary>
        /// Exposes the individual X and Y components of the axis to the specified mapping function and returns the
        /// result;
        /// </summary>
        /// <typeparam name="T">The type being mapped to.</typeparam>
        /// <param name="map">
        /// A function which maps the X and Y values to an instance of <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        /// The result of the <paramref name="map"/> function when passed the individual X and Y components.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the value passed to the <paramref name="map"/> parameter is <c>null</c>.
        /// </exception>
        public T Map<T>(Func<float, float, T> map)
        {
            if (map == null)
                throw new ArgumentNullException(nameof(map));

            return map(X, Y);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Axis other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to.</param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is Axis && Equals((Axis)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();

                hashCode = (hashCode * 397) ^ Y.GetHashCode();

                return hashCode;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"({X.ToString("F4")}, {Y.ToString("F4")})";
        }

        public static implicit operator Vector2(Axis a) => a.ToVector2();

        public static bool operator ==(Axis x, Axis y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Axis x, Axis y)
        {
            return !x.Equals(y);
        }

        public static Vector2 operator *(Axis axis, float magnitude)
        {
            return axis.ToVector2(magnitude);
        }
    }
}