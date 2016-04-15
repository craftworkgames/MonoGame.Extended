using System;

namespace MonoGame.Extended.Interpolation.Easing
{
    public class CircularEasing : EasingFunction
    {
        protected override double Function(double t) {
            return 1 - Math.Sqrt(1 - Math.Pow(t, 2));
        }
    }
}