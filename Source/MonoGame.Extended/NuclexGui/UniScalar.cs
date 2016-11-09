namespace MonoGame.Extended.NuclexGui
{
    /// <summary>Stores a size or location on one axis</summary>
    /// <remarks>
    ///     <para>
    ///         Any position or size in GUI uses a combined position consisting
    ///         of a fraction and an offset. The fraction specifies the position or size as a
    ///         fraction of the parent frame's bounds and usually is in the range between 0.0 and
    ///         1.0. The offset simply is the number of pixels to divert from the thusly
    ///         determined location.
    ///     </para>
    ///     <para>
    ///         Through the use of both fraction and offset, any kind of anchoring behavior can be
    ///         achieved that normally would require a complex anchoring and docking system as can
    ///         be seen in System.Windows.Forms.
    ///     </para>
    ///     <para>
    ///         If you, for example, wanted to always place a control 20 pixels from the right
    ///         border of its parent container, set the fraction of its position to 1.0 (always
    ///         on the right border) and the offset to -20.0 (go 20 units to the left from there).
    ///     </para>
    ///     <para>
    ///         You can achieve traditional absolute positioning by leaving the fraction at 0.0,
    ///         which is equivalent to the upper or left border of the parent container.
    ///     </para>
    /// </remarks>
    public struct UniScalar
    {
        /// <summary>A scalar that has been initialized to zero</summary>
        public static readonly UniScalar Zero = new UniScalar();

        /// <summary>Position of the scalar as fraction of the parent frame's bounds</summary>
        /// <remarks>
        ///     The relative part is normally in the 0.0 .. 1.0 range, denoting the
        ///     fraction of the parent container's size the scalar will indicate.
        /// </remarks>
        public float Fraction;

        /// <summary>Offset of the scalar in pixels relative to its fractional position</summary>
        /// <remarks>
        ///     This part is taken literally without paying attention to the size of
        ///     the parent container the coordinate is used in.
        /// </remarks>
        public float Offset;

        /// <summary>Initializes a new scalar from an offset only</summary>
        /// <param name="offset">Offset in pixels this scalar indicates</param>
        public UniScalar(float offset) : this(0.0f, offset)
        {
        }

        /// <summary>Initializes a new dimension from an absolute and a relative part</summary>
        /// <param name="fraction">Fractional position within the parent frame</param>
        /// <param name="offset">Offset in pixels from the fractional position</param>
        public UniScalar(float fraction, float offset)
        {
            Fraction = fraction;
            Offset = offset;
        }

        /// <summary>Implicitely constructs a scalar using a float as the absolute part</summary>
        /// <param name="offset">Float that will be used for the scalar's absolute value</param>
        /// <returns>
        ///     A new scalar constructed with the original float as its absolute part
        /// </returns>
        public static implicit operator UniScalar(float offset)
        {
            return new UniScalar(offset);
        }

        /// <summary>Converts the scalar into a pure offset position</summary>
        /// <param name="containerSize">
        ///     Absolute dimension of the parent that the relative coordinate relates to
        /// </param>
        /// <returns>
        ///     The absolute position in the parent container denoted by the dimension
        /// </returns>
        public float ToOffset(float containerSize)
        {
            return Fraction*containerSize + Offset;
        }

        /// <summary>Adds one scalar to another</summary>
        /// <param name="scalar">Base scalar to add to</param>
        /// <param name="summand">Scalar to add to the base</param>
        /// <returns>The result of the addition</returns>
        public static UniScalar operator +(UniScalar scalar, UniScalar summand)
        {
            return new UniScalar(
                scalar.Fraction + summand.Fraction,
                scalar.Offset + summand.Offset
            );
        }

        /// <summary>Subtracts one scalar from another</summary>
        /// <param name="scalar">Base scalar to subtract from</param>
        /// <param name="subtrahend">Scalar to subtract from the base</param>
        /// <returns>The result of the subtraction</returns>
        public static UniScalar operator -(UniScalar scalar, UniScalar subtrahend)
        {
            return new UniScalar(
                scalar.Fraction - subtrahend.Fraction,
                scalar.Offset - subtrahend.Offset
            );
        }

        /// <summary>Divides one scalar by another</summary>
        /// <param name="scalar">Base scalar to be divided</param>
        /// <param name="divisor">Divisor to divide by</param>
        /// <returns>The result of the division</returns>
        public static UniScalar operator /(UniScalar scalar, UniScalar divisor)
        {
            return new UniScalar(
                scalar.Fraction/divisor.Fraction,
                scalar.Offset/divisor.Offset
            );
        }

        /// <summary>Multiplies one scalar with another</summary>
        /// <param name="scalar">Base scalar to be multiplied</param>
        /// <param name="factor">Factor to multiply by</param>
        /// <returns>The result of the multiplication</returns>
        public static UniScalar operator *(UniScalar scalar, UniScalar factor)
        {
            return new UniScalar(
                scalar.Fraction*factor.Fraction,
                scalar.Offset*factor.Offset
            );
        }

        /// <summary>Checks two scalars for inequality</summary>
        /// <param name="first">First scalar to be compared</param>
        /// <param name="second">Second scalar to be compared</param>
        /// <returns>True if the instances differ or exactly one reference is set to null</returns>
        public static bool operator !=(UniScalar first, UniScalar second)
        {
            return !(first == second);
        }

        /// <summary>Checks two scalars for equality</summary>
        /// <param name="first">First scalar to be compared</param>
        /// <param name="second">Second scalar to be compared</param>
        /// <returns>True if both instances are equal or both references are null</returns>
        public static bool operator ==(UniScalar first, UniScalar second)
        {
            // For a struct, neither 'first' nor 'second' can be null
            return first.Equals(second);
        }

        /// <summary>Checks whether another instance is equal to this instance</summary>
        /// <param name="other">Other instance to compare to this instance</param>
        /// <returns>True if the other instance is equal to this instance</returns>
        public override bool Equals(object other)
        {
            if (!(other is UniScalar))
                return false;

            return Equals((UniScalar) other);
        }

        /// <summary>Checks whether another instance is equal to this instance</summary>
        /// <param name="other">Other instance to compare to this instance</param>
        /// <returns>True if the other instance is equal to this instance</returns>
        public bool Equals(UniScalar other)
        {
            // For a struct, 'other' cannot be null
            return (Fraction == other.Fraction) && (Offset == other.Offset);
        }

        /// <summary>Obtains a hash code of this instance</summary>
        /// <returns>The hash code of the instance</returns>
        public override int GetHashCode()
        {
            return Fraction.GetHashCode() ^ Offset.GetHashCode();
        }

        /// <summary>
        ///     Returns a human-readable string representation for the unified scalar
        /// </summary>
        /// <returns>The human-readable string representation of the unified scalar</returns>
        public override string ToString()
        {
            return string.Format(
                "{{{0}% {1}{2}}}",
                Fraction*100.0f,
                Offset >= 0.0f ? "+" : string.Empty,
                Offset
            );
        }
    }
}