using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Interpolation.Easing
{
    public class CurveEasing : EasingFunction
    {
        public CurveEasing(Curve curve) {
            Curve = curve;
        }
        public Curve Curve { get; set; }
        protected override double Function(double t) {
            return Curve.Evaluate((float)t);
        }
    }
}