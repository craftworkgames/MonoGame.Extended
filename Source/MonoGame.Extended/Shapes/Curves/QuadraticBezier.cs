using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Curves
{
    public class QuadraticBezier : CurveF
    {
        public QuadraticBezier(Vector2 start, Vector2 control, Vector2 end) {
            StartPoint = start;
            ControlPoint = control;
            EndPoint = end;
        }
        public Vector2 ControlPoint { get; set; }

        protected override float Left { get; }
        protected override float Top { get; }
        protected override float Right { get; }
        protected override float Bottom { get; }

        protected override Vector2 GetPointOnCurve(float t) {
            var i = 1 - t;
            return i * i * StartPoint
                   + 2 * i * t * ControlPoint
                   + t * t * EndPoint;
        }
    }
}