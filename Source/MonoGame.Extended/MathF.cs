using System;
using System.Runtime.CompilerServices;

namespace MonoGame.Extended
{
    public static class MathF
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(ref float firstValue, ref float secondValue)
        {
            var temp = firstValue;
            firstValue = secondValue;
            secondValue = temp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Maximum(float value, float otherValue)
        {
            return otherValue > value ? otherValue : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Minimum(float value, float otherValue)
        {
            return otherValue < value ? otherValue : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(float value)
        {
            return (float)Math.Sqrt(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float value)
        {
            return (float)Math.Sin(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float value)
        {
            return (float)Math.Cos(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Exp(float power)
        {
            return (float)Math.Exp(power);
        }
    }
}
