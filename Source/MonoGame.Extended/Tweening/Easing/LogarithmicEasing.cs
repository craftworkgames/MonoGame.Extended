using System;

namespace MonoGame.Extended.Tweening.Easing
{
    public class LogarithmicEasing : EasingFunction
    {
        public int Base { get; set; } = 2;
        protected override double Function(double t) {
            return Math.Log((Base - 1) * t + 1, Base);
        }
    }
}