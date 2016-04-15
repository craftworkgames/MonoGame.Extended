using System;

namespace MonoGame.Extended.Interpolation.Easing
{
    public struct EaseStepping
    {
        public EaseStepping(int stepcount = 1, bool roundup = false) {
            StepCount = stepcount;
            RoundUp = roundup;
        }
        public int StepCount { get; set; }
        public bool RoundUp { get; set; }

    }
    public class StepEasing : EasingFunction
    {
        public StepEasing(int stepCount, bool roundup = false) {
            StepCount = stepCount;
            RoundUp = roundup;
        }

        public bool RoundUp { get; set; }
        public int StepCount { get; set; }

        protected override double Function(double t) {
            if (RoundUp) {
                return Math.Ceiling(t * StepCount) / StepCount;
            }
            return Math.Floor(t * StepCount) / StepCount;
        }
        public override string ToString() => $"Step easing[{StepCount}|" + (RoundUp ? "Start" : "End") + "]";
    }
}