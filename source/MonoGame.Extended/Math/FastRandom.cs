using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{

    /// <summary>
    /// Represents a pseudo-random number generator using a linear congruential generator algorithm.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     This implementation uses the same constants as Microsoft Visual C++ rand() function:
    ///
    ///     a=214013
    ///     c=2531011
    ///     m=2^31
    /// </para>
    /// <para>
    ///     It provides high performance and speed, but comes at the price of having lower statistical quality, or true
    ///     'randomness' compared to modern algorithms.  The algorithm is deterministic based on the initial seed
    ///     value, making it suitable for reproducible sequences.
    ///</para>
    ///<para>
    ///     Note: This pseudo-random number generator exhibits noticeable patterns and should not be used for
    ///     cryptographic purposes or when a high-quality random distribution is critical.  Consider using
    ///     <see cref="System.Random"/> for better statistical properties.
    /// </para>
    /// </remarks>
    public class FastRandom
    {
        private readonly IFastRandomImpl _impl;

        /// <summary>
        /// Provides a thread-safe <see cref="FastRandom"/> instance that may be used concurrently from any thread.
        /// </summary>
        public static FastRandom Shared { get; } = new FastRandom(new ThreadSafeFastRandomImpl());

        /// <summary>
        /// Initializes a new instance of the <see cref="FastRandom"/> class using the default seed value.
        /// </summary>
        public FastRandom() : this(1)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastRandom"/> class using the specified seed value.
        /// </summary>
        /// <param name="seed">A number used to calculate a starting value for the pseudo-random number sequence.</param>
        public FastRandom(int seed)
        {
            _impl = new LinearCongruentialGeneratorImpl(seed);
        }

        private FastRandom(IFastRandomImpl impl)
        {
            ArgumentNullException.ThrowIfNull(impl);
            _impl = impl;
        }

        /// <summary>
        /// Returns a non-negative random integer.
        /// </summary>
        /// <returns>A 32-bit signed integer that is greater than or equal to 0 and less than 32768.</returns>
        public int Next()
        {
            return _impl.Next();
        }

        /// <summary>
        /// Returns a non-negative random integer that is less than or equal to the specified maximum.
        /// </summary>
        /// <param name="max">The inclusive upper bound of the random number to be generated.</param>
        /// <returns>
        /// A 32-bit signed integer that is greater than or equal to 0 and less than or equal to <paramref name="max"/>.
        /// </returns>
        public int Next(int max)
        {
            return _impl.Next(max);
        }

        /// <summary>
        /// Returns a random integer that is within a specified range.
        /// </summary>
        /// <param name="min">The inclusive lower bound of the random number returned.</param>
        /// <param name="max">The inclusive upper bound of the random number returned.</param>
        /// <returns>
        /// A 32-bit signed integer that is greater than or equal to <paramref name="min"/> and less than or equal to
        /// <paramref name="max"/>.
        /// </returns>
        public int Next(int min, int max)
        {
            return _impl.Next(min, max);
        }

        /// <summary>
        /// Returns a random integer that is within a specified range.
        /// </summary>
        /// <param name="range">
        /// A range representing the inclusive lower and upper bound of the random number to return.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that is greater than or equal to the <see cref="Range{T}.Min"/> and less than or
        /// equal to the <see cref="Range{T}.Max"/> value of <paramref name="range"/>.
        /// </returns>
        [Obsolete("Use Next(Interval<int>).  Range<T> will be removed in 6.0")]
        public int Next(Range<int> range)
        {
            return _impl.Next(range);
        }

        /// <summary>
        /// Returns a random integer that is within a closed interval.
        /// </summary>
        /// <param name="interval">
        /// A closed interval representing the lower and upper bounds of the random number to return.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer that is greater than or equal to the <see cref="Interval{T}.Min"/> value and less
        /// than or equal to the <see cref="Interval{T}.Max"/> value of <paramref name="interval"/>.
        /// </returns>
        public int Next(Interval<int> interval)
        {
            return _impl.Next(interval);
        }

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0 and less than 1.0.
        /// </summary>
        /// <returns>
        /// A single-precision floating point number that is greater than or equal to 0.0 and less than 1.0.
        /// </returns>
        public float NextSingle()
        {
            return _impl.NextSingle();
        }

        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0 and less than the
        /// specified maximum.
        /// </summary>
        /// <param name="max">The exclusive upper bond of the random number generated.</param>
        /// <returns>
        /// A single precision floating-point number that is greater than or equal to 0.0 and less than
        /// <paramref name="max"/>.
        /// </returns>
        public float NextSingle(float max)
        {
            return _impl.NextSingle(max);
        }

        /// <summary>
        /// Returns a random floating-point number that is within a specified range.
        /// </summary>
        /// <param name="min">The inclusive lower bound of the random number returned.</param>
        /// <param name="max">The exclusive upper bound of the random number returned.</param>
        /// <returns>
        /// A single-precision floating point number that is greater than or equal to <paramref name="min"/> and
        /// less than <paramref name="max"/>.
        /// </returns>
        public float NextSingle(float min, float max)
        {
            return _impl.NextSingle(min, max);
        }

        /// <summary>
        /// Returns a random floating-point number that is within a specified range.
        /// </summary>
        /// <param name="range">
        /// A range representing the inclusive lower and exclusive upper bound of the random number returned.
        /// </param>
        /// <returns>
        /// A single-precision floating point number that is greater than or equal to the <see cref="Range{T}.Min"/>
        /// and less than the <see cref="Range{T}.Max"/> value of <paramref name="range"/>
        /// </returns>
        [Obsolete("Use Next(Interval<float>).  Range<T> will be removed in 6.0")]
        public float NextSingle(Range<float> range)
        {
            return _impl.NextSingle(range);
        }

        /// <summary>
        /// Returns a random floating-point number that is within a closed interval.
        /// </summary>
        /// <param name="interval">
        /// A closed interval representing the lower and upper bounds of the random number to return.
        /// </param>
        /// <returns>
        /// A single-precision floating point number that is greater than or equal to the <see cref="Interval{T}.Min"/>
        /// value and less than or equal to the <see cref="Interval{T}.Max"/> value of <paramref name="interval"/>.
        /// </returns>
        public float NextSingle(Interval<float> interval)
        {
            return _impl.NextSingle(interval);
        }

        /// <summary>
        /// Returns a random angle between -π and π.
        /// </summary>
        /// <returns>
        /// A random angle value in radians.
        /// </returns>
        public float NextAngle()
        {
            return _impl.NextAngle();
        }

        /// <summary>
        /// Gets a random unit vector.
        /// </summary>
        /// <param name="vector">When this method returns, contains a unit vector with a random direction.</param>
        public void NextUnitVector(out Vector2 vector)
        {
            _impl.NextUnitVector(out vector);
        }

        /// <summary>
        /// Gets a random unit vector.
        /// </summary>
        /// <param name="vector">A pointer to the Vector2 where the random unit vector will be stored.</param>
        public unsafe void NextUnitVector(Vector2* vector)
        {
            _impl.NextUnitVector(vector);
        }

        #region IFastRandomImplementation
        private interface IFastRandomImpl
        {
            int Next();
            int Next(int max);
            int Next(int min, int max);
            [Obsolete("Use Next(Interval<int>).  Range<T> will be removed in 6.0")]
            int Next(Range<int> range);
            int Next(Interval<int> interval);
            float NextSingle();
            float NextSingle(float max);
            float NextSingle(float min, float max);
            [Obsolete("Use Next(Interval<float>).  Range<T> will be removed in 6.0")]
            float NextSingle(Range<float> range);
            float NextSingle(Interval<float> interval);
            float NextAngle();
            void NextUnitVector(out Vector2 vector);
            unsafe void NextUnitVector(Vector2* vector);
        }
        #endregion

        #region Linear Congruential Generator
        private sealed class LinearCongruentialGeneratorImpl : IFastRandomImpl
        {
            private const int MULTIPLIER = 214013;
            private const int INCREMENT = 2531011;

            private int _state;

            public LinearCongruentialGeneratorImpl() : this(1)
            {

            }

            public LinearCongruentialGeneratorImpl(int seed)
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(seed);
                _state = seed;
            }

            public int Next()
            {
                _state = MULTIPLIER * _state + INCREMENT;
                return (_state >> 16) & 0x7FFF;
            }

            public int Next(int max)
            {
                return (int)(max * NextSingle() + 0.5f);
            }

            public int Next(int min, int max)
            {
                return (int)((max - min) * NextSingle() + 0.5f) + min;
            }

            [Obsolete("Use Next(Interval<int>).  Range<T> will be removed in 6.0")]
            public int Next(Range<int> range)
            {
                return Next(range.Min, range.Max);
            }

            public int Next(Interval<int> interval)
            {
                return Next(interval.Min, interval.Max);
            }

            public float NextSingle()
            {
                return Next() / (float)short.MaxValue;
            }

            public float NextSingle(float max)
            {
                return max * NextSingle();
            }

            public float NextSingle(float min, float max)
            {
                return (max - min) * NextSingle() + min;
            }

            [Obsolete("Use Next(Interval<float>).  Range<T> will be removed in 6.0")]
            public float NextSingle(Range<float> range)
            {
                return NextSingle(range.Min, range.Max);
            }

            public float NextSingle(Interval<float> interval)
            {
                return NextSingle(interval.Min, interval.Max);
            }

            public float NextAngle()
            {
                return NextSingle(-MathHelper.Pi, MathHelper.Pi);
            }

            public void NextUnitVector(out Vector2 vector)
            {
                float angle = NextAngle();
                vector.X = MathF.Cos(angle);
                vector.Y = MathF.Sin(angle);
            }

            public unsafe void NextUnitVector(Vector2* vector)
            {
                float angle = NextAngle();
                vector->X = MathF.Cos(angle);
                vector->Y = MathF.Sin(angle);
            }
        }
        #endregion

        #region ThreadSafeImpl
        private sealed class ThreadSafeFastRandomImpl : IFastRandomImpl
        {
            [ThreadStatic]
            private static LinearCongruentialGeneratorImpl t_random;

            private static LinearCongruentialGeneratorImpl LocalRandom => t_random ?? Create();

            [MethodImpl(MethodImplOptions.NoInlining)]
            private static LinearCongruentialGeneratorImpl Create()
            {
                t_random = new LinearCongruentialGeneratorImpl();
                return t_random;
            }

            public int Next()
            {
                return LocalRandom.Next();
            }

            public int Next(int max)
            {
                return LocalRandom.Next(max);
            }

            public int Next(int min, int max)
            {
                return LocalRandom.Next(min, max);
            }

            [Obsolete("Use Next(Interval<int>).  Range<T> will be removed in 6.0")]
            public int Next(Range<int> range)
            {
                return LocalRandom.Next(range.Min, range.Max);
            }

            public int Next(Interval<int> interval)
            {
                return LocalRandom.Next(interval.Min, interval.Max);
            }

            public float NextSingle()
            {
                return LocalRandom.NextSingle();
            }

            public float NextSingle(float max)
            {
                return LocalRandom.NextSingle(max);
            }

            public float NextSingle(float min, float max)
            {
                return LocalRandom.NextSingle(min, max);
            }

            [Obsolete("Use Next(Interval<float>).  Range<T> will be removed in 6.0")]
            public float NextSingle(Range<float> range)
            {
                return LocalRandom.NextSingle(range.Min, range.Max);
            }

            public float NextSingle(Interval<float> interval)
            {
                return LocalRandom.NextSingle(interval.Min, interval.Max);
            }

            public float NextAngle()
            {
                return LocalRandom.NextAngle();
            }

            public void NextUnitVector(out Vector2 vector)
            {
                LocalRandom.NextUnitVector(out vector);
            }

            public unsafe void NextUnitVector(Vector2* vector)
            {
                LocalRandom.NextUnitVector(vector);
            }
        }
        #endregion
    }
}
