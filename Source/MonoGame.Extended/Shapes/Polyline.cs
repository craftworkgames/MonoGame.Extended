using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public class Polyline
    {
        public Polyline(IEnumerable<Vector2> points)
        {
            Points = points;
        }

        public IEnumerable<Vector2> Points { get; private set; }
        public float Left => Points.Min(p => p.X);
        public float Top => Points.Min(p => p.Y);
        public float Right => Points.Max(p => p.X);
        public float Bottom => Points.Max(p => p.Y);

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
            return false;
        }

        public bool Contains(Vector2 point)
        {
            return Contains(point.X, point.Y);
        }
    }
}