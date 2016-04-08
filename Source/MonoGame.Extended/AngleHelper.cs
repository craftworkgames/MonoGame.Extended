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
    }
}