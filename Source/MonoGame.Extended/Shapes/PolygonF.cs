using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public class PolygonF : ShapeF
    {
        public PolygonF (IEnumerable<Vector2> vertices)
        {
            _localVertices = vertices.ToArray();
            _transformedVertices = _localVertices;
        }

        private readonly Vector2[] _localVertices;
        private Vector2[] _transformedVertices;

        public Vector2[] Vertices
        {
            get { return _transformedVertices; }
        }

        public void Rotate(float rotation)
        {
            Transform(Vector2.Zero, rotation, Vector2.One);
        }

        public void Scale(Vector2 scale)
        {
            Transform(Vector2.Zero, 0, scale);
        }

        private void Transform(Vector2 translation, float rotation, Vector2 scale)
        {
            var newVertices = new Vector2[_localVertices.Length];
            var isScaled = scale != Vector2.One;

            for (var i = 0; i < _localVertices.Length; i++)
            {
                var p = _localVertices[i];
                
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

            _transformedVertices = newVertices;
        }

        public RectangleF GetBoundingRectangle()
        {
            var minX = _transformedVertices.Min(i => i.X);
            var minY = _transformedVertices.Min(i => i.Y);
            var maxX = _transformedVertices.Max(i => i.X);
            var maxY = _transformedVertices.Max(i => i.Y);

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        public override bool Contains(float x, float y)
        {
            var intersects = 0;

            for (var i = 0; i < _transformedVertices.Length; i++)
            {
                var x1 = _transformedVertices[i].X;
                var y1 = _transformedVertices[i].Y;
                var x2 = _transformedVertices[(i + 1) % _transformedVertices.Length].X;
                var y2 = _transformedVertices[(i + 1) % _transformedVertices.Length].Y;

                if ((y1 <= y && y < y2 || y2 <= y && y < y1) && x < (x2 - x1) / (y2 - y1) * (y - y1) + x1)
                    intersects++;
            }

            return (intersects & 1) == 1;
        }

        public void Offset(Vector2 amount)
        {
            throw new NotImplementedException();
        }
    }
}
