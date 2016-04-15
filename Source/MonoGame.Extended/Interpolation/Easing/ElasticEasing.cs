using System;

namespace MonoGame.Extended.Interpolation.Easing
{
    public class ElasticEasing : EasingFunction
    {
        protected override double Function(double t) {
            const double angle = Math.PI * 20 / 3;
            return Math.Pow(2, 10 * t--) * Math.Sin((t - .075) * angle);
        }
    }
}