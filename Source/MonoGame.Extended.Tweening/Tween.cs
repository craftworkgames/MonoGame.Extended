using System;
using System.Collections.Generic;

namespace MonoGame.Extended.Tweening
{
    public class Tween
    {
        public Tween(object target, float duration, float delay, IEnumerable<TweenMember> members)
        {
            Target = target;
            Duration = duration;
            Delay = delay;

            _remainingDelay = delay;
            _members = new List<TweenMember>(members);
        }

        public object Target { get; }
        public float Duration { get; }
        public float Delay { get; }
        public bool IsPaused { get; set; }
        public bool IsRepeating => _remainingRepeats != 0;
        public bool IsRepeatingForever => _remainingRepeats < 0;
        public bool IsAutoReverse { get; private set; }
        public float TimeRemaining => Duration - _elapsedDuration;

        private float _completion;
        public float Completion
        {
            get
            {
                if (_completion < 0)
                    return 0;

                if (_completion > 1)
                    return 1;

                return _completion;
            }
        }

        private readonly List<TweenMember> _members;
        private Func<float, float> _easingFunction;
        private bool _isInitialized;
        private bool _isReversed;
        private bool _needsReset;
        private float _elapsedDuration;
        private Action _onBegin;
        private Action _onEnd;
        private int _remainingRepeats;
        private float _repeatDelay;
        private float _remainingDelay;

        public Tween Easing(Func<float, float> easingFunction) { _easingFunction = easingFunction; return this; }
        public Tween OnBegin(Action callback) { _onBegin = callback; return this; }
        public Tween OnEnd(Action callback) { _onEnd = callback; return this; }
        public Tween Pause() { IsPaused = true; return this; }
        public Tween Resume() { IsPaused = false; return this; }
        public Tween Repeat(int count, float repeatDelay = 0f) { _remainingRepeats = count; _repeatDelay = repeatDelay; return this; }
        public Tween RepeatForever(float repeatDelay = 0f) { _remainingRepeats = -1; _repeatDelay = repeatDelay; return this; }
        public Tween AutoReverse() { IsAutoReverse = true; return this; }

        public void Cancel() { throw new NotImplementedException(); }

        public void CancelAndComplete() { throw new NotImplementedException(); }

        public void Update(float elapsedSeconds)
        {
            if(IsPaused)
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

                foreach (var member in _members)
                    member.Initialize();

                _onBegin?.Invoke();
            }

            if (_needsReset)
            {
                _needsReset = false;
                _elapsedDuration = 0;
                _onBegin?.Invoke();
            }

            _elapsedDuration += elapsedSeconds;

            var n = _completion = _elapsedDuration / Duration;
            var raiseEnd = false;

            if (_isReversed)
                n = _completion = 1f - n;

            if (_elapsedDuration >= Duration)
            {
                if (_remainingRepeats != 0)
                {
                    if(_remainingRepeats > 0)
                        _remainingRepeats--;

                    _remainingDelay = _repeatDelay;
                    _needsReset = true;

                    if (IsAutoReverse)
                        _isReversed = !_isReversed;
                }
                else
                {
                    n = _completion = 1;
                }

                raiseEnd = true;
            }
            
            if (_easingFunction != null)
                n = _easingFunction(n);

            foreach (var member in _members)
                member.Update(n);

            if (raiseEnd)
                _onEnd?.Invoke();
        }
    }
}