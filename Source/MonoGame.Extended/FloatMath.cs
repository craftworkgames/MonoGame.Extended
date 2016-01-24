using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoGame.Extended
{
    public static class FloatMath
    {
        public const float PI  = 3.1415927f;
        public const float TAU = PI*2;
        public const float E = 2.7182818f;

        public const float RadiansToDegrees = 180f / PI;
        public const float RadDeg = RadiansToDegrees;

        public const float DegreesToRadians = PI / 180;
        public const float DegRad = DegreesToRadians;

        public static float Asin(float value)
        {
            return (float) Math.Asin(value);
        }

        public static float Sin(float value)
        {
            return (float)Math.Sin(value);
        }

        public static float Sinh(float value)
        {
            return (float)Math.Sinh(value);
        }

        public static float Acos(float value)
        {
            return (float)Math.Acos(value);
        }

        public static float Cos(float value)
        {
            return (float)Math.Cos(value);
        }

        public static float Cosh(float value)
        {
            return (float)Math.Cosh(value);
        }

        public static float Atan(float value)
        {
            return (float)Math.Atan(value);
        }

        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        public static float Tan(float value)
        {
            return (float)Math.Tan(value);
        }

        public static float Tanh(float value)
        {
            return (float)Math.Tanh(value);
        }

        public static float Abs(float value)
        {
            return (float)Math.Abs(value);
        }

        public static float Ceiling(float value)
        {
            return (float)Math.Ceiling(value);
        }

        public static float Floor(float value)
        {
            return (float)Math.Floor(value);
        }

        public static float Round(float value)
        {
            return (float)Math.Round(value);
        }

        public static float Round(float value, int digits)
        {
            return (float)Math.Round(value, digits);
        }

        public static float Sqrt(float value)
        {
            return (float)Math.Sqrt(value);
        }

        public static float Log(float value, float newBase)
        {
            return (float)Math.Log(value, newBase);
        }

        public static float Log(float value)
        {
            return (float)Math.Log(value);
        }

        public static float Log10(float value)
        {
            return (float)Math.Log10(value);
        }

        public static float Log2(float value)
        {
            return Log(value, 2);
        }

        public static float Exp(float value)
        {
            return (float)Math.Exp(value);
        }

        public static float Pow(float x, float y)
        {
            return (float)Math.Pow(x, y);
        }

        public static float Min(float val1, float val2)
        {
            if (val1 < val2)
                return val1;

            return float.IsNaN(val1) ? val1 : val2;
        }

        public static float Min(float val1, float val2, float val3)
        {
            return Min(val1, Min(val2, val3));
        }

        public static float Min(float val1, params float[] valTail)
        {
            return valTail.Aggregate(val1, Min);
        }

        public static float Max(float val1, float val2)
        {
            if (val1 > val2)
                return val1;

            return float.IsNaN(val1) ? val1 : val2;
        }

        public static float Max(float val1, float val2, float val3)
        {
            return Max(val1, Max(val2, val3));
        }

        public static float Max(float val1, params float[] valTail)
        {
            return valTail.Aggregate(val1, Max);
        }
        
        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static float Lerp(float start, float end, float percentage)
        {
            return start + (end - start) * percentage;
        }

        public static int Sign(float value)
        {
            if (value < 0)
                return -1;

            if (value > 0)
                return 1;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (value == 0)
                return 0;

            // value is NaN
            throw new ArithmeticException();
        }

        public static bool IsZero(float value, float tolerance)
        {
            return Abs(value) <= tolerance;
        }

        public static float ToDegree(float radians)
        {
            return radians * RadiansToDegrees;
        }

        public static float ToRadians(float degree)
        {
            return degree * DegreesToRadians;
        }
    }
}
