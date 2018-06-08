using System;

namespace MonoGame.Extended
{
    public interface IShapeF
    {
        bool Intersects(IShapeF other);
    }

    public static class Shape
    {
        public static bool Intersects(IShapeF a, IShapeF b)
        {
            var intersects = false;

            if (a is RectangleF rectA && b is RectangleF rectB)
            {
                intersects = rectA.Intersects(rectB);
            } else if (a is CircleF circA && b is CircleF circB)
            {
                intersects = circA.Intersects(circB);
            } else if (a is RectangleF rect1 && b is CircleF circ1)
            {
                return Intersects(circ1, rect1);
            } else if (a is CircleF circ2 && b is RectangleF rect2)
            {
                return Intersects(circ2, rect2);
            }

            return intersects;
        }

        public static bool Intersects(CircleF circ, RectangleF rect)
        {
            var boundingRect = new BoundingRectangle(rect.Center, rect.Size / 2);
            return circ.Intersects(boundingRect);
        }
    }
}