using System;

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

        public static Easing None = new NoEasing();
    }
}