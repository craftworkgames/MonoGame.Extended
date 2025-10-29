using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tests
{
    public static class AssertExtensions
    {
        public static bool AreApproximatelyEqual(Vector2 firstPoint, Vector2 secondPoint)
        {
            return Math.Abs(firstPoint.X - secondPoint.X) < float.Epsilon &&
                   Math.Abs(firstPoint.Y - secondPoint.Y) < float.Epsilon;
        }

        public static bool AreApproximatelyEqual(RectangleF firstRectangle, RectangleF secondRectangle)
        {
            return Math.Abs(firstRectangle.X - secondRectangle.X) < float.Epsilon &&
                   Math.Abs(firstRectangle.Y - secondRectangle.Y) < float.Epsilon &&
                   Math.Abs(firstRectangle.Width - secondRectangle.Width) < float.Epsilon &&
                   Math.Abs(firstRectangle.Height - secondRectangle.Height) < float.Epsilon;
        }

        public static void Equal(Matrix expected, Matrix actual, int precision = 5)
        {
            Assert.Equal(expected.M11, actual.M11, precision);
            Assert.Equal(expected.M12, actual.M12, precision);
            Assert.Equal(expected.M13, actual.M13, precision);
            Assert.Equal(expected.M14, actual.M14, precision);

            Assert.Equal(expected.M21, actual.M21, precision);
            Assert.Equal(expected.M22, actual.M22, precision);
            Assert.Equal(expected.M23, actual.M23, precision);
            Assert.Equal(expected.M24, actual.M24, precision);

            Assert.Equal(expected.M31, actual.M31, precision);
            Assert.Equal(expected.M32, actual.M32, precision);
            Assert.Equal(expected.M33, actual.M33, precision);
            Assert.Equal(expected.M34, actual.M34, precision);

            Assert.Equal(expected.M41, actual.M41, precision);
            Assert.Equal(expected.M42, actual.M42, precision);
            Assert.Equal(expected.M43, actual.M41, precision);
            Assert.Equal(expected.M44, actual.M44, precision);
        }
    }
}
