using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public delegate float EasingFunction(float value);

    public static class EasingFunctions
    {
        public static float Linear(float value) => value;
        public static float CubicEaseIn(float value) => Power.EaseIn(value, 2);
        public static float CubicEaseOut(float value) => Power.EaseOut(value, 2);
        public static float CubicEaseInOut(float value) => Power.EaseInOut(value, 2);
        public static float QuadraticEaseIn(float value) => Power.EaseIn(value, 3);
        public static float QuadraticEaseOut(float value) => Power.EaseOut(value, 3);
        public static float QuadraticEaseInOut(float value) => Power.EaseInOut(value, 3);
        public static float QuarticEaseIn(float value) => Power.EaseIn(value, 4);
        public static float QuarticEaseOut(float value) => Power.EaseOut(value, 4);
        public static float QuarticEaseInOut(float value) => Power.EaseInOut(value, 4);
        public static float QuinticEaseIn(float value) => Power.EaseIn(value, 5);
        public static float QuinticEaseOut(float value) => Power.EaseOut(value, 5);
        public static float QuinticEaseInOut(float value) => Power.EaseInOut(value, 5);
        public static float SineEaseIn(float value) => (float)Math.Sin(value * MathHelper.PiOver2 - MathHelper.PiOver2) + 1;
        public static float SineEaseOut(float value) => (float)Math.Sin(value * MathHelper.PiOver2);
        public static float SineEaseInOut(float value) => (float)(Math.Sin(value * MathHelper.Pi - MathHelper.PiOver2) + 1) / 2;

        private static class Power
        {
            public static float EaseIn(double s, int power)
            {
                return (float)Math.Pow(s, power);
            }

            public static float EaseOut(double s, int power)
            {
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign * (Math.Pow(s - 1, power) + sign));
            }

            public static float EaseInOut(double s, int power)
            {
                s *= 2;

                if (s < 1)
                    return EaseIn(s, power) / 2;

                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign / 2.0 * (Math.Pow(s - 2, power) + sign * 2));
            }
        }
    }
}