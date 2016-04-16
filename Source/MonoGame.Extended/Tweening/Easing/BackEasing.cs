using System;

namespace MonoGame.Extended.Tweening.Easing
{
    public class BackEasing : EasingFunction
    {
        public double Amplitude = 1;
        protected override double Function(double t) {
            return Math.Pow(t, 3) - t * Amplitude * Math.Sin(t * Math.PI);
        }
    }
}