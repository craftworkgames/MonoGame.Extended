using System;
using System.Linq.Expressions;

namespace MonoGame.Extended.Animations.Tweens
{
    public class PropertyTween<T> : Animation
        where T : struct
    {
        protected static Func<T, T, T> Add;
        protected static Func<T, T, T> Subtract;
        protected static Func<T, float, T> Multiply;

        private readonly Func<T> _getValue;
        private readonly Action<T> _setValue;
        private float _currentMultiplier;
        private T? _initialValue;

        static PropertyTween()
        {
            var a = Expression.Parameter(typeof(T));
            var b = Expression.Parameter(typeof(T));
            var c = Expression.Parameter(typeof(float));
            Add = Expression.Lambda<Func<T, T, T>>(Expression.Add(a, b), a, b).Compile();
            Subtract = Expression.Lambda<Func<T, T, T>>(Expression.Subtract(a, b), a, b).Compile();
            Multiply = Expression.Lambda<Func<T, float, T>>(Expression.Multiply(a, c), a, c).Compile();
        }

        public PropertyTween(Func<T> getValue, Action<T> setValue, T targetValue, float duration,
            EasingFunction easingFunction)
            : base(null, true)
        {
            _getValue = getValue;
            _setValue = setValue;
            TargetValue = targetValue;
            Duration = duration;
            EasingFunction = easingFunction;
        }

        public T TargetValue { get; }
        public float Duration { get; }
        public EasingFunction EasingFunction { get; set; }

        protected override bool OnUpdate(float deltaTime)
        {
            if (!_initialValue.HasValue)
                _initialValue = _getValue();

            _currentMultiplier = EasingFunction(CurrentTime/Duration);

            if (CurrentTime >= Duration)
            {
                CurrentTime = Duration;
                _currentMultiplier = 1.0f;
                return true;
            }

            var difference = Subtract(TargetValue, _initialValue.Value);
            var multiply = Multiply(difference, _currentMultiplier);
            var newValue = Add(_initialValue.Value, multiply);
            _setValue(newValue);
            return false;
        }
    }
}