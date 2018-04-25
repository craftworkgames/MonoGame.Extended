using System;
using System.Collections.Generic;

namespace Tweening.NewTweening
{
    public class Tween
    {
        public Tween(object target, float duration, float delay, IEnumerable<TweenMember> members)
        {
            Target = target;
            Duration = duration;
            Delay = delay;
            _members = new List<TweenMember>(members);
        }

        public object Target { get; }
        public float Duration { get; }
        public float Delay { get; }
        public bool IsPaused { get; set; }
        public bool IsRepeating => _repeatCount != 0;
        public bool IsRepeatingForever => _repeatCount < 0;
        public bool IsAutoReverse { get; private set; }
        public float TimeRemaining => Duration - _time;

        public float Completion
        {
            get
            {
                var completion = _time / Duration;

                if (completion < 0)
                    return 0;

                if (completion > 1)
                    return 1;

                return completion;
            }
        }

        private readonly List<TweenMember> _members;
        private Func<float, float> _easingFunction;
        private bool _isInitialized;
        private float _time;
        private Action _onStart;
        private Action _onEnd;
        private int _repeatCount;
        private float _repeatDelay;

        public Tween Easing(Func<float, float> easingFunction) { _easingFunction = easingFunction; return this; }
        public Tween OnStart(Action callback) { _onStart = callback; return this; }
        public Tween OnEnd(Action callback) { _onEnd = callback; return this; }
        public Tween Pause() { IsPaused = true; return this; }
        public Tween Resume() { IsPaused = false; return this; }
        public Tween Repeat(int count) { _repeatCount = count; return this; }
        public Tween RepeatForever() { _repeatCount = -1; return this; }
        public Tween RepeatDelay(float seconds) { _repeatDelay = seconds; return this; }
        public Tween AutoReverse() { IsAutoReverse = true; return this; }

        public void Cancel() { throw new NotImplementedException(); }

        public void CancelAndComplete() { throw new NotImplementedException(); }

        public void Update(float elapsedSeconds)
        {
            if(IsPaused)
                return;

            if (!_isInitialized)
            {
                _isInitialized = true;

                foreach (var member in _members)
                    member.Initialize();

                _onStart?.Invoke();
            }

            _time += elapsedSeconds;
            var completion = _time / Duration;

            if (_easingFunction != null)
                completion = _easingFunction(completion);

            foreach (var member in _members)
                member.Update(completion);
        }
    }
}