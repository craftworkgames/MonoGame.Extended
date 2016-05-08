using System;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public class PropertyTween<T> : IAnimation
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

        public PropertyTween(T initialValue, Action<T> setValue, T targetValue, float duration, EasingFunction easingFunction)
        {
            _setValue = setValue;
            InitialValue = initialValue;
            TargetValue = targetValue;
            Duration = duration;
            EasingFunction = easingFunction;
        }

        private readonly Action<T> _setValue;
        private float _currentTime;
        private float _currentMultiplier;

        public T InitialValue { get; }
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

            _currentTime += deltaTime;
            _currentMultiplier = EasingFunction(_currentTime/Duration);

            if (_currentTime >= Duration)
            {
                _currentTime = Duration;
                _currentMultiplier = 1.0f;
                IsComplete = true;
            }

            var newValue = Add(InitialValue, Multiply(Subtract(TargetValue, InitialValue), _currentMultiplier));
            _setValue(newValue);
        }
    }
}