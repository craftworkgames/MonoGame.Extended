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
                return vector2.NormalizedCopy() * maxLength;

            return vector2;
        }

        public static bool IsNaN(this Vector2 vector2)
        {
            return float.IsNaN(vector2.X) || float.IsNaN(vector2.Y);
        }

        public static float ToAngle(this Vector2 vector2)
        {
            return (float) Math.Atan2(vector2.X, -vector2.Y);
        }

        // http://stackoverflow.com/questions/473782/inline-functions-in-c
        // http://www.dotnetperls.com/inline-optimization
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DotWith(this Vector2 vector, ref Vector2 value)
        {
            // https://en.wikipedia.org/wiki/Dot_product
            // http://mathworld.wolfram.com/DotProduct.html
            // A dot B = A.x * B.x + A.y + B.y = len(A) * len(A) * cos(theta)
            return vector.X * value.X + vector.Y * value.Y;
        }

        // http://stackoverflow.com/questions/473782/inline-functions-in-c
        // http://www.dotnetperls.com/inline-optimization
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CrossWith(this Vector2 vector, ref Vector2 value)
        {
            // https://en.wikipedia.org/wiki/Cross_product
            // http://mathworld.wolfram.com/CrossProduct.html
            // A cross B = A.x * B.y - A.y * B.x = n * len(A) * len(B) * sin(theta) = det(A, B)
            // n is the unit vector perpendicular to the plane of A and B
            return vector.X * value.Y - vector.Y * value.X;
        }

        // http://stackoverflow.com/questions/473782/inline-functions-in-c
        // http://www.dotnetperls.com/inline-optimization
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ProjectOnto(this Vector2 vector, ref Vector2 value)
        {
            // https://en.wikipedia.org/wiki/Vector_projection
            // http://mathworld.wolfram.com/Projection.html
            // ((A dot B) / len(B)) * (B / len(B)) = (A dot B) / (len(B))^2 * B = (A dot B) / (B dot B) * B
            var dotNumerator = vector.X * value.X + vector.Y * value.Y;
            var dotDenominator = vector.X * vector.X + vector.Y * vector.Y;
            return dotNumerator / dotDenominator * value;
        }
    }
}