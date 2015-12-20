using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended
{
    public static class Vector2Extensions
    {
        public static Vector2 Rotate(this Vector2 vector2, float radians)
        {
            var cos = (float) Math.Cos(radians);
            var sin = (float) Math.Sin(radians);
            return new Vector2(vector2.X * cos - vector2.Y * sin, vector2.X * sin + vector2.Y * cos);
        }

        public static Vector2 NormalizedCopy(this Vector2 vector2)
        {
            var newVector2 = new Vector2(vector2.X, vector2.Y);
            newVector2.Normalize();
            return newVector2;
        }
    }
}