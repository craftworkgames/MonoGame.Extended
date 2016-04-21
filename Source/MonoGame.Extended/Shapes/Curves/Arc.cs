using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Curves
{
    /// <summary>
    /// Describes a curve that goes through three points, laying on the circumference of a circle
    /// </summary>
    public class Arc : CurveF
    {
        private float _startAngle;
        private float _arcAngle;
        private Vector2 _middlePoint;
        private Vector2 _startPoint;
        private Vector2 _endPoint;

        public Arc(Vector2 start, Vector2 middle, Vector2 end)  {
            _middlePoint = middle;
            _endPoint = end;
            OnPointChange();
        }
        public Arc(Vector2 start, Vector2 middle) {
            _middlePoint = middle;
            _startPoint = start;
        }

        public override float Length { get; protected set; }

        public override Vector2 StartPoint
        {
            get { return _startPoint; }
            set
            {
                if (_startPoint == value) return;
                _startPoint = value;
                OnPointChange();
            }
        }

        public override Vector2 EndPoint
        {
            get { return _endPoint; }
            set
            {
                if (_endPoint == value) return;
                _endPoint = value;
                OnPointChange();
            }
        }
        public Vector2 MiddlePoint
        {
            get { return _middlePoint; }
            set
            {
                if (_middlePoint == value) return;
                _middlePoint = value;
                OnPointChange();
            }
        }

        public float Radius { get; private set; }
        public Vector2 Center { get; private set; }

        public override Vector2 GetPointOnCurve(float t) =>
            Center.SecondaryPoint(Radius, _startAngle + t * _arcAngle);

        private void OnPointChange() {
            Calculate(StartPoint, MiddlePoint, EndPoint);
            Length = Radius * _arcAngle;
        }

        private void Calculate(Vector2 a, Vector2 b, Vector2 c) {
            var ma = AngleHelper.Gradient(a, b);
            var mb = AngleHelper.Gradient(b, c);
            var x0 = (ma * mb * (a.Y - c.Y) + mb * (a.X + b.X) - ma * (b.X + c.X)) / (2 * (mb - ma));
            var y0 = (-2 * x0 + a.X + b.X) / (2 * ma) + (a.Y + b.Y) / 2.0f;
            Center = new Vector2(x0, y0);

            var start = a - Center;
            Radius = start.Length();
            _startAngle = start.ToAngle();
            var a1 = AngleHelper.BetweenZeroAndTau((b - Center).ToAngle() - _startAngle);
            var a2 = AngleHelper.BetweenZeroAndTau((c - Center).ToAngle() - _startAngle);
            _arcAngle = a2;
            if (a1 > a2) {
                _arcAngle -= MathHelper.TwoPi;
            }
        }
    }
}