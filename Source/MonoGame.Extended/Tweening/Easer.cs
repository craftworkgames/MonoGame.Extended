using MonoGame.Extended.Tweening.Easing;
using MonoGame.Extended.Tweening.Interpolators;

namespace MonoGame.Extended.Tweening
{
    public class Easer<T>
    {
        // One will be created for each Type instance of Easer
        private static readonly Interpolator<T> INTERPOLATOR;

        static Easer() {
            INTERPOLATOR = InterpolatorStore.GetRegistered<T>();
        }

        public T Endvalue
        {
            get { return _endvalue; }
            set
            {
                _endvalue = value;
                if (_startvalue == null) return;
                _difference = INTERPOLATOR == null
                    ? DynamicInterpolator.Singleton.Substract(Endvalue, Startvalue)
                    : INTERPOLATOR.Substract(Endvalue, Startvalue);
            }
        }

        public T Startvalue
        {
            get { return _startvalue; }
            set
            {
                _startvalue = value;
                if (_endvalue == null) return;
                _difference = INTERPOLATOR == null
                    ? DynamicInterpolator.Singleton.Substract(Endvalue, Startvalue)
                    : INTERPOLATOR.Substract(Endvalue, Startvalue);
            }
        }

        public void SetValues(T startvalue, T endvalue) {
            _startvalue = startvalue;
            _endvalue = endvalue;
            _difference = INTERPOLATOR == null
                ? DynamicInterpolator.Singleton.Substract(Endvalue, Startvalue)
                : INTERPOLATOR.Substract(Endvalue, Startvalue);
        }

        private T _difference;
        private T _startvalue;
        private T _endvalue;
        // public Range<T> ValueRange { get; set; }


        public EasingFunction Easing { get; set; } = EasingFunction.None;

        public T Ease(double t) {
            return INTERPOLATOR == null
                ? (T)DynamicInterpolator.Singleton.Add(_startvalue, (T)DynamicInterpolator.Singleton.Mult(_difference, Easing.Ease(t)))
                : INTERPOLATOR.Add(_startvalue, INTERPOLATOR.Mult(_difference, Easing.Ease(t)));
        }

        public T Interpolate(double t) {
            return INTERPOLATOR == null
                            ? (T)DynamicInterpolator.Singleton.Add(_startvalue, (T)DynamicInterpolator.Singleton.Mult(_difference, t))
                            : INTERPOLATOR.Add(_startvalue, INTERPOLATOR.Mult(_difference, t));
        }
        public T Interpolate(double t, T startValue, T endValue) {
            return INTERPOLATOR == null
                            ? (T)DynamicInterpolator.Singleton.Interpolate(startValue, endValue, t)
                            : INTERPOLATOR.Interpolate(startValue, endValue, t);
        }

    }
}