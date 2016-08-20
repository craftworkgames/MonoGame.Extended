using System;
using System.Runtime.CompilerServices;

namespace MonoGame.Extended
{
    public static class FloatHelper
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(ref float firstValue, ref float secondValue)
        {
            var temp = firstValue;
            firstValue = secondValue;
            secondValue = temp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Maximum(this float value, float otherValue)
        {
            return otherValue > value ? otherValue : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Minimum(this float value, float otherValue)
        {
            return otherValue < value ? otherValue : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(this float value)
        {
            return (float)Math.Sqrt(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(this float value)
        {
            return (float)Math.Sin(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(this float value)
        {
            return (float)Math.Cos(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Exp(this float power)
        {
            return (float)Math.Exp(power);
        }
    }
}