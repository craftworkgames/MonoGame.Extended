using System;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public class PropertyTween<T> : IAnimation
        where T : struct 
    {
        static PropertyTween()
        {
            var a = Expression.Parameter(typeof(T));
            var b = Expression.Parameter(typeof(T));
            var c = Expression.Parameter(typeof(float));
            Add = Expression.Lambda<Func<T, T, T>>(Expression.Add(a, b), a, b).Compile();
            Subtract = Expression.Lambda<Func<T, T, T>>(Expression.Subtract(a, b), a, b).Compile();
            Multiply = Expression.Lambda<Func<T, float, T>>(Expression.Multiply(a, c), a, c).Compile();
        } 

        public PropertyTween(Func<T> getValue, Action<T> setValue, T targetValue, float duration, EasingFunction easingFunction)
        {
            _getValue = getValue;
            _setValue = setValue;
            TargetValue = targetValue;
            Duration = duration;
            EasingFunction = easingFunction;
        }

        private readonly Func<T> _getValue;
        private readonly Action<T> _setValue;
        private float _currentTime;
        private float _currentMultiplier;
        private T? _initialValue;

        public T TargetValue { get; }
        public float Duration { get; }
        public EasingFunction EasingFunction { get; set; }
        public bool IsComplete { get; private set; }

        protected static Func<T, T, T> Add;
        protected static Func<T, T, T> Subtract;
        protected static Func<T, float, T> Multiply;

        public void Update(GameTime gameTime)
        {
            Update(gameTime.GetElapsedSeconds());
        }

        public void Update(float deltaTime)
        {
            if(IsComplete)
                return;

            if (!_initialValue.HasValue)
                _initialValue = _getValue();

            _currentTime += deltaTime;
            _currentMultiplier = EasingFunction(_currentTime/Duration);

            if (_currentTime >= Duration)
            {
                _currentTime = Duration;
                _currentMultiplier = 1.0f;
                IsComplete = true;
            }

            var difference = Subtract(TargetValue, _initialValue.Value);
            var multiply = Multiply(difference, _currentMultiplier);
            var newValue = Add(_initialValue.Value, multiply);
            _setValue(newValue);
        }
    }
}