using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public static class AngleHelper
    {
        public static float BetweenZeroAndTau(float value) {
            while (value < 0) {
                value += MathHelper.TwoPi;
            }
            while (value > MathHelper.TwoPi) {
                value -= MathHelper.TwoPi;
            }
            return value;
        }
        public static float BetweenMinusPiAndPi(float value) {
            while (value < -MathHelper.Pi) {
                value += MathHelper.TwoPi;
            }
            while (value > MathHelper.Pi) {
                value -= MathHelper.TwoPi;
            }
            return value;
        }
        public static bool IsBetween(float value, float min, float end) {
            return end < min ?
                value >= min || value <= end :
                value >= min && value <= end;
        }
        public static Vector2 SecondaryPoint(this Vector2 v, float angle, float distance) {
            var result = new Vector2(v.X, v.Y);
            result.X += (float)(distance * Math.Cos(angle));
            result.Y -= (float)(distance * Math.Sin(angle));
            return result;
        }

        public static float GradientIncrement = 0.00001f;
        public static float Gradient(Vector2 a, Vector2 b) {
            var ax = a.X; //to not change the values in vector a
            var ay = a.Y;
            if (ax == b.X) ax += GradientIncrement;
            if (ay == b.Y) ay += GradientIncrement;
            return (b.Y - ay) / (b.X - ax);
        }
    }
}