using Microsoft.Xna.Framework;

namespace MonoGame.Extended.NuclexGui
{
    /// <summary>Stores a two-dimensional position or size</summary>
    public struct UniVector
    {
        /// <summary>A vector that has been initialized to zero</summary>
        public static readonly UniVector Zero = new UniVector();

        /// <summary>The vector's X coordinate</summary>
        public UniScalar X;

        /// <summary>The vector's Y coordinate</summary>
        public UniScalar Y;

        /// <summary>Initializes a new vector from the provided components</summary>
        /// <param name="x">Absolute and relative X coordinate of the vector</param>
        /// <param name="y">Absolute and relative Y coordinate of the vector</param>
        public UniVector(UniScalar x, UniScalar y)
        {
            X = x;
            Y = y;
        }

        /// <summary>Converts the vector into pure offset coordinates</summary>
        /// <param name="containerSize">
        ///     Dimensions of the container the relative part of the vector counts for
        /// </param>
        /// <returns>An XNA vector with the pure offset coordinates of the vector</returns>
        public Vector2 ToOffset(Vector2 containerSize)
        {
            return ToOffset(containerSize.X, containerSize.Y);
        }

        /// <summary>Converts the vector into pure offset coordinates</summary>
        /// <param name="containerWidth">
        ///     Width of the container the fractional part of the vector counts for
        /// </param>
        /// <param name="containerHeight">
        ///     Height of the container the fractional part of the vector counts for
        /// </param>
        /// <returns>An XNA vector with the pure offset coordinates of the vector</returns>
        public Vector2 ToOffset(float containerWidth, float containerHeight)
        {
            return new Vector2(
                X.Fraction*containerWidth + X.Offset,
                Y.Fraction*containerHeight + Y.Offset
            );
        }

        /// <summary>Adds one vector to another</summary>
        /// <param name="vector">Base vector to add to</param>
        /// <param name="summand">Vector to add to the base</param>
        /// <returns>The result of the addition</returns>
        public static UniVector operator +(UniVector vector, UniVector summand)
        {
            return new UniVector(vector.X + summand.X, vector.Y + summand.Y);
        }

        /// <summary>Subtracts one vector from another</summary>
        /// <param name="vector">Base vector to subtract from</param>
        /// <param name="subtrahend">Vector to subtract from the base</param>
        /// <returns>The result of the subtraction</returns>
        public static UniVector operator -(UniVector vector, UniVector subtrahend)
        {
            return new UniVector(vector.X - subtrahend.X, vector.Y - subtrahend.Y);
        }

        /// <summary>Divides one vector by another</summary>
        /// <param name="vector">Base vector to be divided</param>
        /// <param name="divisor">Divisor to divide by</param>
        /// <returns>The result of the division</returns>
        public static UniVector operator /(UniVector vector, UniVector divisor)
        {
            return new UniVector(vector.X/divisor.X, vector.Y/divisor.Y);
        }

        /// <summary>Multiplies one vector with another</summary>
        /// <param name="vector">Base vector to be multiplied</param>
        /// <param name="factor">Factor to multiply by</param>
        /// <returns>The result of the multiplication</returns>
        public static UniVector operator *(UniVector vector, UniVector factor)
        {
            return new UniVector(vector.X*factor.X, vector.Y*factor.Y);
        }

        /// <summary>Scales a vector by a scalar factor</summary>
        /// <param name="factor">Factor by which to scale the vector</param>
        /// <param name="vector">Vector to be Scaled</param>
        /// <returns>The result of the multiplication</returns>
        public static UniVector operator *(UniScalar factor, UniVector vector)
        {
            return new UniVector(factor*vector.X, factor*vector.Y);
        }

        /// <summary>Scales a vector by a scalar factor</summary>
        /// <param name="vector">Vector to be Scaled</param>
        /// <param name="factor">Factor by which to scale the vector</param>
        /// <returns>The result of the multiplication</returns>
        public static UniVector operator *(UniVector vector, UniScalar factor)
        {
            return new UniVector(vector.X*factor, vector.Y*factor);
        }

        /// <summary>Checks two vectors for inequality</summary>
        /// <param name="first">First vector to be compared</param>
        /// <param name="second">Second vector to be compared</param>
        /// <returns>True if the instances differ or exactly one reference is set to null</returns>
        public static bool operator !=(UniVector first, UniVector second)
        {
            return !(first == second);
        }

        /// <summary>Checks two vectors for equality</summary>
        /// <param name="first">First vector to be compared</param>
        /// <param name="second">Second vector to be compared</param>
        /// <returns>True if both instances are equal or both references are null</returns>
        public static bool operator ==(UniVector first, UniVector second)
        {
            // For a struct, neither 'first' nor 'second' can be null
            return first.Equals(second);
        }

        /// <summary>Checks whether another instance is equal to this instance</summary>
        /// <param name="other">Other instance to compare to this instance</param>
        /// <returns>True if the other instance is equal to this instance</returns>
        public override bool Equals(object other)
        {
            if (!(other is UniVector))
                return false;

            return Equals((UniVector) other);
        }

        /// <summary>Checks whether another instance is equal to this instance</summary>
        /// <param name="other">Other instance to compare to this instance</param>
        /// <returns>True if the other instance is equal to this instance</returns>
        public bool Equals(UniVector other)
        {
            // For a struct, 'other' cannot be null
            return (X == other.X) && (Y == other.Y);
        }

        /// <summary>Obtains a hash code of this instance</summary>
        /// <returns>The hash code of the instance</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        /// <summary>
        ///     Returns a human-readable string representation for the unified vector
        /// </summary>
        /// <returns>The human-readable string representation of the unified vector</returns>
        public override string ToString()
        {
            return string.Format(
                "{{X:{0}, Y:{1}}}",
                X,
                Y
            );
        }
    }
}