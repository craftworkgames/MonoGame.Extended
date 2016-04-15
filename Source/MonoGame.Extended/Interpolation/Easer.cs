using System;
using System.Collections.Generic;
using MonoGame.Extended.Interpolation.Easing;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Interpolation
{
    public class Easer<T>
    {
        private static Interpolator<T> _interpolator;

        static Easer() {
            _interpolator = InterpolatorStore.GetRegistered<T>();
        }

        public Easer(T endval) {
            EndValue = endval;
        }
        public Easer(Range<T> range) {
            ValueRange = range;
        }

        private T _startValue;
        public Range<T> ValueRange { get; set; }
        public T EndValue { get; set; }
        public EasingFunction Easing { get; set; } = EasingFunction.None;

        public T Ease(double t) {
            if (_startValue.Equals(EndValue)) return EndValue;
            return Interpolate(_startValue, EndValue, Easing.Ease(t));
        }

        private T Interpolate(T start, T end, double t) {
            return _interpolator == null
                ? (T)DynamicInterpolator.Singleton.Interpolate(start, end, t)
                : _interpolator.Interpolate(start, end, t);
        }

        public void SetStartvalue(T startvalue) {
            _startValue = startvalue;
            if (ValueRange != null) EndValue = Interpolate(ValueRange.Min, ValueRange.Max, FastRand.NextSingle());
        }
    }
}