using System;

namespace MonoGame.Extended.Tweening.Easing
{
    public abstract class EasingFunction
    {
        public EasingInOut EasingInOut { get; set; } = EasingInOut.In;
        public int? StepCount { get; set; } = null;
        public bool RoundStepUp { get; set; }

        protected abstract double Function(double t);
        public double Ease(double time, double start, double end) =>
            time < start ? 0 :
            time > end ? 1 :
            Ease(Math.Min(1, Math.Max(0, time - start) / (end - start)));

        public double Ease(double t) {
            if (StepCount.HasValue) {
                if (RoundStepUp) {
                    t = Math.Ceiling(t * StepCount.Value) / StepCount.Value;
                }
                t = Math.Floor(t * StepCount.Value) / StepCount.Value;
            }
            switch (EasingInOut) {
                case EasingInOut.In:
                    return Function(t);
                case EasingInOut.Out:
                    return 1 - Function(1 - t);
                case EasingInOut.InOut:
                    if (t < 0.5) return 0.5 * Function(t * 2);
                    return 1 - 0.5 * Function(2 - t * 2);
                case EasingInOut.OutIn:
                    if (t < 0.5) return 0.5 - 0.5 * Function(1 - t * 2);
                    return 0.5 + 0.5 * Function(2 * t - 1);
                default:
                    return t;
            }
        }

        /// <summary>
        /// transition effect has the same speed from start to end.
        /// </summary>
        public static EasingFunction None => new LinearEasing();
    }
}