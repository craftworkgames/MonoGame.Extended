using System;

namespace MonoGame.Extended.Interpolation.Easing
{
    public class HermiteEasing : EasingFunction
    {
        protected override double Function(double t) {
            var d = t * t;
            return (3 * d - d * t) * 0.5;
        }
    }
}