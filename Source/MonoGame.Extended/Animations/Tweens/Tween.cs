using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public abstract class Tween<T> : IUpdate
    {
        protected Tween(T initialValue, Action<T> setValue, T targetValue, float duration, EasingFunction easingFunction)
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

        protected abstract T CalculateNewValue(T initialValue, float multiplier);

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

            var newValue = CalculateNewValue(InitialValue, _currentMultiplier);
            _setValue(newValue);
        }
    }
}