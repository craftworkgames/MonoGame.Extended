using System;

namespace MonoGame.Extended.Tweening.Easing
{
    public class ExponentialEasing : EasingFunction
    {
        protected override double Function(double t) {
            return Math.Pow(2, 10 * (t - 1));
        }
    }
}