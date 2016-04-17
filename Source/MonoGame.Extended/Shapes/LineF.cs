using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Shapes.Curves;

namespace MonoGame.Extended.Shapes
{
    public struct LineF : IPathF
    {
        public Vector2 StartPoint { get; set; }

        public Vector2 EndPoint { get; set; }

        public float Left => MathHelper.Min(StartPoint.X, StartPoint.X);
        public float Top => MathHelper.Min(StartPoint.Y, StartPoint.Y);
        public float Right => MathHelper.Max(StartPoint.X, StartPoint.X);
        public float Bottom => MathHelper.Max(StartPoint.Y, StartPoint.Y);
        public float Length => (EndPoint - StartPoint).Length();
        public RectangleF GetBoundingRectangle() {
            var left = Left;
            var top = Top;
            return new RectangleF(left, top,
                Right - left,
               Bottom - top);
        }

        public bool Contains(Vector2 point) {
            var r = GetBoundingRectangle();
            //simplified cross
            return Math.Abs((point.X - r.Left) * r.Height - (point.Y - r.Top) * r.Width) < float.Epsilon;
        }

        public bool Contains(Point point) {
            var r = GetBoundingRectangle();
            //simplified cross
            return Math.Abs((point.X - r.Left) * r.Height - (point.Y - r.Top) * r.Width) < 1.414;//TODO check for value to be right
        }

        public Vector2 PointOnOutline(float t) =>
            StartPoint + (EndPoint - StartPoint) * t;

        public Vector2 ToVector() {
            return EndPoint - StartPoint;
        }
    }
}