using System;

namespace MonoGame.Extended.Animations.Easing
{
    public abstract class EasingFunction
    {
        public abstract double Ease(double t);
        public double Ease(double time, double start, double end) => time < start ? 0 : time > end ? 1 :
            Ease(Math.Min(1, Math.Max(0, time - start) / (end - start)));

        private sealed class NoEasing : EasingFunction
        {
            public override double Ease(double t) => t;
            public override string ToString() => "No easing";
        }
        /// <summary>
        /// transition effect has the same speed from start to end.
        /// </summary>
        public static readonly EasingFunction Linear = new NoEasing();
        /// <summary>
        /// Smoothly starts the transition.
        /// </summary>
        public static EasingFunction EaseIn => new CubicBezierEasing(0.42, 0, 1, 1);
        /// <summary>
        /// Smoothly ends the transition.
        /// </summary>
        public static EasingFunction EaseOut => new CubicBezierEasing(0, 0, 0.58, 1);
        //Smoothly starts and ends the transition.
        public static EasingFunction EaseInOut => new CubicBezierEasing(0.42, 0, 0.58, 1);
    }
}