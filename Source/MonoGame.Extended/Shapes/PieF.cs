using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes
{
    public class PieF : IShapeF
    {
        public PieF(Vector2 center, float radius, float startangle, float endangle) {
            Center = center;
            Radius = radius;
            StartAngle = startangle;
            EndAngle = endangle;
            CaclulateBounds();
        }

        public float Radius
        {
            get { return _radius; }
            set
            {
                if (value == _radius) return;
                _radius = value;
                _dirty = true;
            }
        }

        public Vector2 Center
        {
            get { return _center; }
            set
            {
                if (value == _center) return;
                _center = value;
                _dirty = true;
            }
        }

        public float StartAngle
        {
            get { return _startAngle; }
            set
            {
                _startAngle = AngleHelper.BetweenZeroAndTau(value);
                _dirty = true;
            }
        }

        public float EndAngle
        {
            get { return StartAngle + _curveAngle; }
            set
            {
                _curveAngle = AngleHelper.BetweenZeroAndTau(value) - StartAngle;
                _dirty = true;
            }
        }

        private float _curveAngle;
        private float _startAngle;
        public float AdjustedCircumference => MathHelper.Pi * _curveAngle;


        public float Left { get; private set; }
        public float Top { get; private set; }
        public float Right { get; private set; }
        public float Bottom { get; private set; }

        private void CaclulateBounds() {
            if (!_dirty) return;
            var points = new[]{
                Center.SecondaryPoint(StartAngle, Radius),
                Center.SecondaryPoint(EndAngle, Radius),
                Center
            };
            Left = AngleHelper.IsBetween(MathHelper.Pi, StartAngle, EndAngle)
                ? Center.X - Radius
                : points.Min(p => p.X);
            Right = AngleHelper.IsBetween(0, StartAngle, EndAngle)
                ? Center.X + Radius
                : points.Max(p => p.X);
            Top = AngleHelper.IsBetween(MathHelper.PiOver2, StartAngle, EndAngle)
                ? Center.Y - Radius
                : points.Min(p => p.Y);
            Bottom = AngleHelper.IsBetween(MathHelper.PiOver2 * 3, StartAngle, EndAngle)
                ? Center.Y + Radius
                : points.Max(p => p.Y);
            _dirty = false;
        }

        private bool _dirty = true;
        private Vector2 _center;
        private float _radius;

        public RectangleF GetBoundingRectangle() {
            CaclulateBounds();
            return new RectangleF(Left, Top, Right - Left, Bottom - Top);
        }
        
        public bool Contains(Vector2 point) {
            var diff = point - Center;
            return (diff.LengthSquared() <= Radius * Radius) &&
                   AngleHelper.IsBetween(AngleHelper.BetweenZeroAndTau(diff.ToAngle()), StartAngle, EndAngle);
        }

        public IList<Vector2> GetVertices() {
            var result = new List<Vector2> { Center, Center.SecondaryPoint(StartAngle, Radius) };
            var theta = StartAngle;
            var step = MathHelper.TwoPi / Radius;
            while ((theta += step) < EndAngle) {
                result.Add(Center.SecondaryPoint(theta, Radius));
            }
            result.Add(Center.SecondaryPoint(EndAngle, Radius));
            result.Add(Center);
            return result;
        }
    }
}