using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Transformations
{
    public abstract class Easing
    {
        public abstract double Ease(double t);
        public double Ease(double time, double start, double end) =>
            Ease(Math.Min(1, Math.Max(0, time - start) / (end - start)));

        private sealed class NoEasing : Easing
        {
            public override double Ease(double t) => t;
            public override string ToString() => "No easing";
        }
        /// <summary>
        /// transition effect has the same speed from start to end.
        /// </summary>
        public static readonly Easing Linear = new NoEasing();
        /// <summary>
        /// Smoothly starts the transition.
        /// </summary>
        public static Easing EaseIn => new CubicBezierEasing(0.42, 0, 1, 1);
        /// <summary>
        /// Smoothly ends the transition.
        /// </summary>
        public static Easing EaseOut => new CubicBezierEasing(0, 0, 0.58, 1);
        //Smoothly starts and ends the transition.
        public static Easing EaseInOut => new CubicBezierEasing(0.42, 0, 0.58, 1);
    }
    public class CurveEasing : Easing
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