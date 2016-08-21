using System;
using System.Runtime.CompilerServices;

namespace MonoGame.Extended
{
    public static class FloatExtensions
    {
        public const float Epsilon = 0.00001f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(this float value)
        {
            return (float)Math.Round(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Floor(this float value)
        {
            return (float)Math.Floor(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToCeilingInteger(this float value)
        {
            return (int)Math.Ceiling(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToFloorInteger(this float value)
        {
            return (int)Math.Floor(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToRoundInteger(this float value)
        {
            return (int)Math.Round((double)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToTruncateInteger(this float value)
        {
            return (int)Math.Truncate(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(this float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }
            return value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyEqualTo(this float value, float otherValue, float tolerance = Epsilon)
        {
            return Math.Abs(value - otherValue) <= tolerance;
        }
    }
}