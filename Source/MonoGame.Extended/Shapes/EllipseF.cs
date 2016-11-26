using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public class EllipseF : IShapeF
    {
        public EllipseF(Vector2 center, float radiusX, float radiusY)
        {
            Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        public Vector2 Center { get; set; }
        public float RadiusX { get; set; }
        public float RadiusY { get; set; }
        public float Left => Center.X - RadiusX;
        public float Top => Center.Y - RadiusY;
        public float Right => Center.X + RadiusX;
        public float Bottom => Center.Y + RadiusY;

        public RectangleF BoundingRectangle
        {
            get
            {
                var minX = Left;
                var minY = Top;
                var maxX = Right;
                var maxY = Bottom;
                return new RectangleF(minX, minY, maxX - minX, maxY - minY);
            }
        }

        public bool Contains(float x, float y)
        {
            float xCalc = (float) (Math.Pow(x - Center.X, 2) / Math.Pow(RadiusX, 2));
            float yCalc = (float) (Math.Pow(y - Center.Y, 2) / Math.Pow(RadiusY, 2));

            return xCalc + yCalc <= 1;
        }

        public bool Contains(Vector2 point)
        {
            return Contains(point.X, point.Y);
        }
    }
}