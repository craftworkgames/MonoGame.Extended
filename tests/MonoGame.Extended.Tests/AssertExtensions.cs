using System;

namespace MonoGame.Extended.Tests
{
    public static class AssertExtensions
    {
        public static bool AreApproximatelyEqual(Point2 firstPoint, Point2 secondPoint)
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
    }
}