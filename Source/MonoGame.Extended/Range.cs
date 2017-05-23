using System;

namespace MonoGame.Extended
{
    /// <summary>
    ///     Represents a closed interval defined by a minimum and a maximum value of a give type.
    /// </summary>
    public struct Range<T> : IEquatable<Range<T>> where T : IComparable<T>
    {
        public Range(T min, T max)
        {
            if (min.CompareTo(max) > 0 || max.CompareTo(min) < 0)
                throw new ArgumentException("Min has to be smaller than or equal to max.");

            Min = min;
            Max = max;
        }

        public Range(T value)
            : this(value, value)
        {
        }

        /// <summary>
        ///     Gets the minium value of the <see cref="Range{T}" />.
        /// </summary>
        public T Min { get; }

        /// <summary>
        ///     Gets the maximum value of the <see cref="Range{T}" />.
        /// </summary>
        public T Max { get; }


        /// <summary>
        ///     Returns wheter or not this <see cref="Range{T}" /> is degenerate.
        ///     (Min and Max are the same)
        /// </summary>
        public bool IsDegenerate => Min.Equals(Max);

        /// <summary>
        ///     Returns wheter or not this <see cref="Range{T}" /> is proper.
        ///     (Min and Max are not the same)
        /// </summary>
        public bool IsProper => !Min.Equals(Max);

        public bool Equals(Range<T> value) => Min.Equals(value.Min) && Max.Equals(value.Max);

        public override bool Equals(object obj) => obj is Range<T> && Equals((Range<T>) obj);

        public override int GetHashCode() => Min.GetHashCode() ^ Max.GetHashCode();

        public static bool operator ==(Range<T> value1, Range<T> value2) => value1.Equals(value2);

        public static bool operator !=(Range<T> value1, Range<T> value2) => !value1.Equals(value2);

        public static implicit operator Range<T>(T value) => new Range<T>(value, value);

        public override string ToString() => $"Range<{typeof(T).Name}> [{Min} {Max}]";

        /// <summary>
        ///     Returns wheter or not the value falls in this <see cref="Range{T}" />.
        /// </summary>
        public bool IsInBetween(T value, bool minValueExclusive = false, bool maxValueExclusive = false)
        {
            if (minValueExclusive)
            {
                if (value.CompareTo(Min) <= 0)
                    return false;
            }

            if (value.CompareTo(Min) < 0)
                return false;

            if (maxValueExclusive)
            {
                if (value.CompareTo(Max) >= 0)
                    return false;
            }

            return value.CompareTo(Max) <= 0;
        }
    }
}