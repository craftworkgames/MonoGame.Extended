using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public class PolygonF 
    {
        public PolygonF (Vector2[] vertices)
        {
            _vertices = vertices;
        }

        private readonly Vector2[] _vertices;
        public Vector2[] Vertices
        {
            get { return _vertices; }
        }

        public Polygon Transform(Vector2 position, Vector2 origin, float rotation, Vector2 scale)
        {
            var transformedVertices = new Vector2[_vertices.Length];
            var isScaled = scale != Vector2.One;

            for (var i = 0; i <= _vertices.Length; i++)
            {
                var x = _vertices[i].X - origin.X;
                var y = _vertices[i].Y - origin.Y;

                // scale if needed
                if (isScaled)
                {
                    x *= scale.X;
                    y *= scale.Y;
                }

                // rotate if needed
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (rotation != 0)
                {
                    var cos = (float)Math.Cos(rotation); // degrees?
                    var sin = (float)Math.Sin(rotation);
                    var oldX = x;
                    x = cos * x - sin * y;
                    y = sin * oldX + cos * y;
                }

                transformedVertices[i].X = position.X + x + origin.X;
                transformedVertices[i].Y = position.Y + y + origin.Y;
            }

            return new Polygon(transformedVertices);
        }

        public RectangleF GetBoundingRectangle()
        {
            var minX = _vertices.Min(i => i.X);
            var minY = _vertices.Min(i => i.Y);
            var maxX = _vertices.Max(i => i.X);
            var maxY = _vertices.Max(i => i.Y);

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        public bool Contains(float x, float y)
        {
            var intersects = 0;

            for (var i = 0; i < _vertices.Length; i++)
            {
                var x1 = _vertices[i].X;
                var y1 = _vertices[i].Y;
                var x2 = _vertices[(i + 1) % _vertices.Length].X;
                var y2 = _vertices[(i + 1) % _vertices.Length].Y;

                if ((y1 <= y && y < y2 || y2 <= y && y < y1) && x < (x2 - x1) / (y2 - y1) * (y - y1) + x1)
                    intersects++;
            }

            return (intersects & 1) == 1;
        }

        public bool Contains(Vector2 point)
        {
            return Contains(point.X, point.Y);
        }
    }
}
