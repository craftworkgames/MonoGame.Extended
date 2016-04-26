using System;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Tweening.Interpolators;

namespace MonoGame.Extended.Tweening
{
    public struct Range<T> : IEquatable<Range<T>> where T : IComparable<T>
    {
        private static readonly Interpolator<T> INTERPOLATOR;

        static Range() {
            INTERPOLATOR = InterpolatorStore.GetRegistered<T>();
        }

        public Range(T min, T max) {
            if (min.CompareTo(max) > 0 || max.CompareTo(min) < 0) {
                throw new ArgumentException("Min has to be smaller than or equal to max.");
            }
            Min = min;
            Max = max;
        }

        public T Min { get; }

        public T Max { get; }

        public T Random() {
            return INTERPOLATOR == null
                ? (T)DynamicInterpolator.Singleton.Interpolate(Min, Max, FastRand.NextSingle())
                : INTERPOLATOR.Interpolate(Min, Max, FastRand.NextSingle());
        }

        public bool IsDegenerate =>
            Min.Equals(Max);

        public bool IsProper =>
            !Min.Equals(Max);

        public bool Equals(Range<T> value) =>
            Min.Equals(value.Min) && Max.Equals(value.Max);

        public override bool Equals(object obj) =>
            obj is Range<T> && Equals((Range<T>)obj);

        public override int GetHashCode() =>
            Min.GetHashCode() ^ Max.GetHashCode();

        public static bool operator ==(Range<T> value1, Range<T> value2) =>
            value1.Equals(value2);

        public static bool operator !=(Range<T> value1, Range<T> value2) =>
            !value1.Equals(value2);

        public static implicit operator Range<T>(T value) =>
            new Range<T>(value, value);

        public override string ToString() =>
            $"Range<{nameof(T)}> {Min} {Max}";

        public bool IsInBetween(T value, bool minValueExclusive = false, bool maxValueExclusive = false) {
            if (minValueExclusive) {
                if (value.CompareTo(Min) <= 0) return false;
            }
            if (value.CompareTo(Min) < 0) return false;

            if (maxValueExclusive) {
                if (value.CompareTo(Max) >= 0) return false;
            }
            return value.CompareTo(Max) <= 0;
        }
    }
}