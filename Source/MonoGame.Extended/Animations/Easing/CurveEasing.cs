using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Easing
{
    public class CurveEasing : EasingFunction
    {
        public CurveEasing(Curve curve) {
            Curve = curve;
        }
        public Curve Curve { get; set; }
        public override double Ease(double t) {
            return Curve.Evaluate((float)t);
        }
    }


    
}