using System;

namespace MonoGame.Extended.Animations.Easing
{
    public class StepEasing : EasingFunction
    {
        public StepEasing(int stepCount, bool roundup = false) {
            StepCount = stepCount;
            RoundUp = roundup;
        }

        public bool RoundUp { get; set; }
        public int StepCount { get; set; }

        public override double Ease(double t) {
            if (RoundUp) {
                return Math.Ceiling(t * StepCount) / StepCount;
            }
            return Math.Floor(t * StepCount) / StepCount;
        }
        public override string ToString() => $"Step easing[{StepCount}|" + (RoundUp ? "Start" : "End") + "]";
    }
}