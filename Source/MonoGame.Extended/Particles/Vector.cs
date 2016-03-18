using System;

namespace MonoGame.Extended.Particles
{
    /// <summary>
    /// Defines a data structure representing a Euclidean vector facing a particular direction,
    /// including a magnitude value.
    /// </summary>
    public struct Vector
    {
        public readonly float X;
        public readonly float Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> structure.
        /// </summary>
        /// <param name="axis">The axis along which the vector points.</param>
        /// <param name="magnitude">The magnitude of the vector.</param>
        public Vector(Axis axis, float magnitude)
        {
            X = axis.X * magnitude;
            Y = axis.Y * magnitude;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> structure.
        /// </summary>
        /// <param name="x">The X component of the vector.</param>
        /// <param name="y">The Y component of the vector.</param>
        public Vector(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets the length or magnitude of the Euclidean vector.
        /// </summary>
        public float Length => (float)Math.Sqrt(X * X + Y * Y);

        public float LengthSq => X * X + Y * Y;

        /// <summary>
        /// Gets the axis in which the vector is facing.
        /// </summary>
        /// <returns>A <see cref="Axis"/> value representing the direction the vector is facing.</returns>
        public Axis Axis => new Axis(X, Y);

        public Vector Add(Vector v) => new Vector(X + v.X, Y + v.Y);
        public Vector Subtract(Vector v) => new Vector(X - v.X, Y - v.Y);
        public Vector Negate() => new Vector(-X, -Y);
        public Vector Multiply(float factor) => new Vector(X * factor, Y * factor);
        public Vector Divide(float factor) => new Vector(X / factor, Y / factor);

        /// <summary>
        /// Copies the X and Y components of the vector to the specified memory location.
        /// </summary>
        /// <param name="destination">The memory location to copy the coordinate to.</param>
        public unsafe void CopyTo(float* destination)
        {
            destination[0] = X;
            destination[1] = Y;
        }

        /// <summary>
        /// Destructures the vector, exposing the individual X and Y components.
        /// </summary>
        public void Destructure(out float x, out float y)
        {
            x = X;
            y = Y;
        }

        /// <summary>
        /// Exposes the individual X and Y components of the vector to the specified matching function.
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
        /// Exposes the individual X and Y components of the vector to the specified mapping function and returns the
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

        public static float Dot(Vector v1, Vector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public static implicit operator Axis(Vector v) => v.Axis;
        public static Vector operator +(Vector v1, Vector v2) => v1.Add(v2);
        public static Vector operator -(Vector v1, Vector v2) => v1.Subtract(v2);
        public static Vector operator -(Vector v) => v.Negate();
        public static Vector operator *(Vector value, float factor) => value.Multiply(factor);
        public static Vector operator *(float factor, Vector value) => value.Multiply(factor);
        public static Vector operator /(Vector value, float factor) => value.Divide(factor);

        public static Vector Zero => new Vector(0, 0);
        public static Vector One => new Vector(1, 1);
    }
}