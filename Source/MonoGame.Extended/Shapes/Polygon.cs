using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public class Polygon : IEquatable<Polygon>
    {
        public Polygon(IEnumerable<Vector2> vertices)
        {
            _localVertices = vertices.ToArray();
            _transformedVertices = _localVertices;
            _offset = Vector2.Zero;
            _rotation = 0;
            _scale = Vector2.One;
            _isDirty = false;
        }

        private readonly Vector2[] _localVertices;
        private Vector2[] _transformedVertices;
        private Vector2 _offset;
        private float _rotation;
        private Vector2 _scale;
        private bool _isDirty;

        public Vector2[] Vertices
        {
            get
            {
                if (_isDirty)
                {
                    _transformedVertices = GetTransformedVertices();
                    _isDirty = false;
                }

                return _transformedVertices;
            }
        }

        public float Left
        {
            get { return Vertices.Min(v => v.X); }
        }

        public float Right
        {
            get { return Vertices.Max(v => v.X); }
        }

        public float Top
        {
            get { return Vertices.Min(v => v.Y); }
        }

        public float Bottom
        {
            get { return Vertices.Max(v => v.Y); }
        }

        public void Offset(Vector2 amount)
        {
            _offset += amount;
            _isDirty = true;
        }

        public void Rotate(float amount)
        {
            _rotation += amount;
            _isDirty = true;
        }

        public void Scale(Vector2 amount)
        {
            _scale += amount;
            _isDirty = true;
        }

        private Vector2[] GetTransformedVertices()
        {
            var newVertices = new Vector2[_localVertices.Length];
            var isScaled = _scale != Vector2.One;

            for (var i = 0; i < _localVertices.Length; i++)
            {
                var p = _localVertices[i];

                if (isScaled)
                    p *= _scale;

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_rotation != 0)
                {
                    var cos = (float) Math.Cos(_rotation);
                    var sin = (float) Math.Sin(_rotation);
                    p = new Vector2(cos*p.X - sin*p.Y, sin*p.X + cos*p.Y);
                }

                newVertices[i] = p + _offset;
            }

            return newVertices;
        }

        public Polygon TransformedCopy(Vector2 offset, float rotation, Vector2 scale)
        {
            var polygon = new Polygon(_localVertices);
            polygon.Offset(offset);
            polygon.Rotate(rotation);
            polygon.Scale(scale - Vector2.One);
            return new Polygon(polygon.Vertices);
        }

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

        public bool Contains(Vector2 point)
        {
            return Contains(point.X, point.Y);
        }

        public bool Contains(float x, float y)
        {
            var intersects = 0;
            var vertices = Vertices;

            for (var i = 0; i < vertices.Length; i++)
            {
                var x1 = vertices[i].X;
                var y1 = vertices[i].Y;
                var x2 = vertices[(i + 1)%vertices.Length].X;
                var y2 = vertices[(i + 1)%vertices.Length].Y;

                if ((((y1 <= y) && (y < y2)) || ((y2 <= y) && (y < y1))) && (x < (x2 - x1)/(y2 - y1)*(y - y1) + x1))
                    intersects++;
            }

            return (intersects & 1) == 1;
        }

        public static bool operator ==(Polygon a, Polygon b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Polygon a, Polygon b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Polygon && Equals((Polygon) obj);
        }

        public bool Equals(Polygon other)
        {
            return Vertices.SequenceEqual(other.Vertices);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Vertices.Aggregate(27, (current, v) => current + 13*current + v.GetHashCode());
            }
        }
    }
}