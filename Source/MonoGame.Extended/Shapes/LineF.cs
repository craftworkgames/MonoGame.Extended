using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Shapes
{
    public interface IPathF : IShapeBase
    {
        Vector2 StartPoint { get; set; }
        Vector2 EndPoint { get; set; }
    }
    public struct LineF : IPathF
    {
        public Vector2 StartPoint { get; set; }

        private void Stuff(out float left, out float top, out float width, out float height) {
            left = MathHelper.Min(StartPoint.X, EndPoint.X);
            width = MathHelper.Max(StartPoint.X, EndPoint.X) - left;
            top = MathHelper.Min(StartPoint.Y, EndPoint.Y);
            height = MathHelper.Max(StartPoint.Y, EndPoint.Y) - top;
        }

        public Vector2 EndPoint { get; set; }

        public RectangleF GetBoundingRectangle() {
            var left = MathHelper.Min(StartPoint.X, StartPoint.X);
            var top = MathHelper.Min(StartPoint.Y, StartPoint.Y);
            return new RectangleF(left, top,
                MathHelper.Max(StartPoint.X, StartPoint.X) - left,
                MathHelper.Max(StartPoint.Y, StartPoint.Y) - top);
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