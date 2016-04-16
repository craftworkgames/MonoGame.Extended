using System;
using System.Collections.Generic;
using MonoGame.Extended.Interpolation.Easing;
using MonoGame.Extended.Particles;

namespace MonoGame.Extended.Interpolation
{
    public class Easer<T>
    {
        // One will be created for each Type instance of Easer
        private static readonly Interpolator<T> INTERPOLATOR;

        static Easer() {
            INTERPOLATOR = InterpolatorStore.GetRegistered<T>();
        }

        public Easer(T endval) {
            EndValue = endval;
        }
        public Easer(Range<T> range) {
            ValueRange = range;
        }

        private T _startValue;
        private T _difference;
        public Range<T> ValueRange { get; set; }

        public T EndValue
        {
            get
            {
                return INTERPOLATOR == null
                    ? DynamicInterpolator.Singleton.Add(_startValue, _difference)
                    : INTERPOLATOR.Add(_startValue, _difference);
            }
            set
            {
                _difference = INTERPOLATOR == null
                  ? DynamicInterpolator.Singleton.Substract(value, _startValue)
                  : INTERPOLATOR.Substract(value, _startValue);
            }
        }

        public EasingFunction Easing { get; set; } = EasingFunction.None;

        public T Ease(double t, EasingFunction easing = null) {
            t = (easing ?? Easing).Ease(t);
            return INTERPOLATOR == null
                ? (T)DynamicInterpolator.Singleton.Add(_startValue, (T)DynamicInterpolator.Singleton.Mult(_difference,  t))
                : INTERPOLATOR.Add(_startValue, INTERPOLATOR.Mult(_difference, Easing.Ease(t)));
        }



        public void SetStartvalue(T startvalue) {
            _startValue = startvalue;
            if (ValueRange != null) {
                double t = FastRand.NextSingle();
                EndValue = INTERPOLATOR == null
                    ? (T)DynamicInterpolator.Singleton.Interpolate(ValueRange.Min, ValueRange.Max, t)
                    : INTERPOLATOR.Interpolate(ValueRange.Min, ValueRange.Max, t);
            }
        }
    }
}