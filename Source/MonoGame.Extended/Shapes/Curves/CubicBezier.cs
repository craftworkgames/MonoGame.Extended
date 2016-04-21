using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Curves
{
    public class CubicBezier : CurveF, IBezier
    {
        public enum CubicBezierType
        {
            Serpentine,
            Cusp,
            Loop,
            /// <summary>
            /// Controlpoints are equal
            /// </summary>
            Quadratic,
            /// <summary>
            /// Last three points are equal
            /// </summary>
            Line,
            /// <summary>
            /// All points are equal
            /// </summary>
            Point,
        }

        public CubicBezier(Vector2 start, Vector2 controlpoint1, Vector2 controlpoint2, Vector2 end) {
            ControlPoint1 = controlpoint1;
            ControlPoint2 = controlpoint2;
            StartPoint = start;
            EndPoint = end;

        }

        public Vector2 ControlPoint1 { get; set; }

        public Vector2 ControlPoint2 { get; set; }
        
        public override float Length { get; protected set; }

        public override Vector2 GetPointOnCurve(float t) {
            return BezierCalculation(t);
        }

        public int Order => 4;

        private Vector2 BezierCalculation(float t) {
            var i = 1 - t;
            return i*i*i*StartPoint +
                   3*i*i*t*ControlPoint1 +
                   3*i*t*t*ControlPoint2 +
                   t*t*t*EndPoint;
        }
    }
}