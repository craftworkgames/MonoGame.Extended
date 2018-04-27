using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tweening
{
    public class Tween
    {
        internal Tween(object target, float duration, float delay, TweenMember member)
        {
            Target = target;
            Member = member;
            Duration = duration;
            Delay = delay;
            IsAlive = true;

            _remainingDelay = delay;
        }

        public object Target { get; }
        public TweenMember Member { get; }
        public float Duration { get; }
        public float Delay { get; }
        public bool IsPaused { get; set; }
        public bool IsRepeating => _remainingRepeats != 0;
        public bool IsRepeatingForever => _remainingRepeats < 0;
        public bool IsAutoReverse { get; private set; }
        public bool IsAlive { get; private set; }
        public bool IsComplete { get; private set; }
        public float TimeRemaining => Duration - _elapsedDuration;
        public float Completion => MathHelper.Clamp(_completion, 0, 1);

        private Func<float, float> _easingFunction;
        private bool _isInitialized;
        private float _completion;
        private float _elapsedDuration;
        private float _remainingDelay;
        private float _repeatDelay;
        private int _remainingRepeats;
        private Action<Tween> _onBegin;
        private Action<Tween> _onEnd;

        public Tween Easing(Func<float, float> easingFunction) { _easingFunction = easingFunction; return this; }
        public Tween OnBegin(Action<Tween> action) { _onBegin = action; return this; }
        public Tween OnEnd(Action<Tween> action) { _onEnd = action; return this; }
        public Tween Pause() { IsPaused = true; return this; }
        public Tween Resume() { IsPaused = false; return this; }
        public Tween Repeat(int count, float repeatDelay = 0f) { _remainingRepeats = count; _repeatDelay = repeatDelay; return this; }
        public Tween RepeatForever(float repeatDelay = 0f) { _remainingRepeats = -1; _repeatDelay = repeatDelay; return this; }
        public Tween AutoReverse() { IsAutoReverse = true; return this; }

        public void Cancel()
        {
            _remainingRepeats = 0;
            IsAlive = false;
        }

        public void CancelAndComplete()
        {
            if (IsAlive)
            {
                _completion = 1;

                Member.Interpolate(1);
                IsComplete = true;
                _onEnd?.Invoke(this);
            }

            Cancel();
        }

        public void Update(float elapsedSeconds)
        {
            if(IsPaused || !IsAlive)
                return;

            if (_remainingDelay > 0)
            {
                _remainingDelay -= elapsedSeconds;

                if (_remainingDelay > 0)
                    return;
            }

            if (!_isInitialized)
            {
                _isInitialized = true;
                Member.Initialize();
                _onBegin?.Invoke(this);
            }

            if (IsComplete)
            {
                IsComplete = false;
                _elapsedDuration = 0;
                _onBegin?.Invoke(this);

                if (IsAutoReverse)
                    Member.Swap();
            }

            _elapsedDuration += elapsedSeconds;

            var n = _completion = _elapsedDuration / Duration;

            if (_easingFunction != null)
                n = _easingFunction(n);

            if (_elapsedDuration >= Duration)
            {
                if (_remainingRepeats != 0)
                {
                    if(_remainingRepeats > 0)
                        _remainingRepeats--;

                    _remainingDelay = _repeatDelay;
                }

                n = _completion = 1;
                IsComplete = true;
            }

            Member.Interpolate(n);

            if (IsComplete)
            {
                IsAlive = _remainingRepeats != 0;
                _onEnd?.Invoke(this);
            }
        }
    }
}