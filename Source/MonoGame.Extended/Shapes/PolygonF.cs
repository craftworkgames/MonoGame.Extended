using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public class PolygonF : ShapeF<PolygonF>
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

        public override PolygonF Transform(Vector2 translation, float rotation, Vector2 scale)
        {
            var newVertices = new Vector2[_vertices.Length];
            var isScaled = scale != Vector2.One;

            for (var i = 0; i < _vertices.Length; i++)
            {
                var p = _vertices[i];
                
                if (isScaled)
                    p *= scale;

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (rotation != 0)
                {
                    var cos = (float) Math.Cos(rotation);
                    var sin = (float) Math.Sin(rotation);
                    p = new Vector2(cos * p.X - sin * p.Y, sin * p.X + cos * p.Y);
                }

                newVertices[i] = p + translation;
            }

            return new PolygonF(newVertices);
        }


        public RectangleF GetBoundingRectangle()
        {
            var minX = _vertices.Min(i => i.X);
            var minY = _vertices.Min(i => i.Y);
            var maxX = _vertices.Max(i => i.X);
            var maxY = _vertices.Max(i => i.Y);

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        public override bool Contains(float x, float y)
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
    }
}
