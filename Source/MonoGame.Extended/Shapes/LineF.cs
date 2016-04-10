using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Shapes
{
    public struct LineF : IShapeF
    {
        private Vector2 _pointA;
        private Vector2 _pointB;

        public Vector2 PointA
        {
            get { return _pointA; }
            set
            {
                _pointA = value;
                Stuff();
            }
        }

        private void Stuff() {
            Left = MathHelper.Min(_pointA.X, _pointB.X);
            Right = MathHelper.Max(_pointA.X, _pointB.X);
            Top = MathHelper.Min(_pointA.Y, _pointB.Y);
            Bottom = MathHelper.Max(_pointA.Y, _pointB.Y);
        }

        public Vector2 PointB
        {
            get { return _pointB; }
            set
            {
                _pointB = value;
                Stuff();
            }
        }

        public float Left { get; private set; }
        public float Top { get; private set; }
        public float Right { get; private set; }
        public float Bottom { get; private set; }
        public RectangleF GetBoundingRectangle() => new RectangleF(Left, Top, Right - Left, Bottom - Top);

        public bool Contains(Vector2 point) => ((point.X - Left) * (Bottom - Top) - (point.Y - Top) * (Right - Left)) < float.Epsilon;

        public Vector2 RandomPointInside() {
            var difference = PointB - PointA;
            return PointA + difference * FastRand.NextSingle(0, difference.Length());
        }

        public Vector2 PointOnOutline(float t) => PointA + (PointB - PointA) * t;
    }
}