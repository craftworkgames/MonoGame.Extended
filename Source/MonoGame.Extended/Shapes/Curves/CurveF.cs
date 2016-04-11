using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Shapes.Curves
{
    public abstract class CurveF : IPathF
    {
        public Vector2 StartPoint { get; set; }
        public Vector2 EndPoint { get; set; }
        protected abstract float Left { get; }
        protected abstract float Top { get; }
        protected abstract float Right { get; }
        protected abstract float Bottom { get; }
        public RectangleF GetBoundingRectangle() => new RectangleF(Left, Top, Right - Left, Bottom - Top);

        public abstract bool Contains(Vector2 point);

        public Vector2 RandomPointInside() => PointOnOutline(FastRand.NextSingle(0, 1));
        public Vector2 PointOnOutline(float t) {
            if (t < float.Epsilon) return StartPoint;
            if (t - 1 < float.Epsilon) return EndPoint;
            return GetPointOnCurve(t);
        }

        /// <summary>
        /// Returns the point at t on the curve, 1 being startpoint and 0 being endpoint
        /// </summary>
        /// <param name="t">a value between 0 and 1</param>
        /// <returns></returns>
        protected abstract Vector2 GetPointOnCurve(float t);

    }
}