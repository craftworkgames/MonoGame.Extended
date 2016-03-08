using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public static class Vector2Extensions
    {
        public static bool EqualsWithTolerance(this Vector2 vector2, Vector2 other, float tolerance = 0.00001f)
        {
            return Math.Abs(vector2.X - other.X) <= tolerance && Math.Abs(vector2.Y - other.Y) <= tolerance;
        }

        public static Vector2 Rotate(this Vector2 vector2, float radians)
        {
            var cos = (float)Math.Cos(radians);
            var sin = (float)Math.Sin(radians);
            return new Vector2(vector2.X * cos - vector2.Y * sin, vector2.X * sin + vector2.Y * cos);
        }

        public static Vector2 NormalizedCopy(this Vector2 vector2)
        {
            var newVector2 = new Vector2(vector2.X, vector2.Y);
            newVector2.Normalize();
            return newVector2;
        }

        public static Vector2 PerpendicularClockwise(this Vector2 vector2)
        {
            return new Vector2(-vector2.Y, vector2.X);
        }

        public static Vector2 PerpendicularCounterClockwise(this Vector2 vector2)
        {
            return new Vector2(vector2.Y, -vector2.X);
        }

        public static Vector2 Truncate(this Vector2 vector2, float maxLength)
        {
            if (vector2.LengthSquared() > maxLength * maxLength)
            {
                return vector2.NormalizedCopy() * maxLength;
            }

            return vector2;
        }

        public static bool IsNaN(this Vector2 vector2)
        {
            return float.IsNaN(vector2.X) || float.IsNaN(vector2.Y);
        }

        public static float ToAngle(this Vector2 vector2)
        {
            return (float)Math.Atan2(vector2.X, -vector2.Y);
        }
    }
}