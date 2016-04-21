using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Shapes.Curves
{
    public class QuadraticBezier : CurveF, IBezier
    {
        public QuadraticBezier(Vector2 start, Vector2 control, Vector2 end) {
            ControlPoint = control;
            StartPoint = start;
            EndPoint = end;
        }

        public Vector2 ControlPoint { get; set; }
        public override float Length { get; protected set; }

        public override Vector2 GetPointOnCurve(float t) {
            var i = 1 - t;
            return i * i * StartPoint
                   + 2 * i * t * ControlPoint
                   + t * t * EndPoint;
        }
        
        public int Order => 3;
    }
}