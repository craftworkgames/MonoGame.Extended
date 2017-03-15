using System;

namespace MonoGame.Extended.Tweening
{
    public class TweenChain<T> : TweenAnimation<T>
    {
        private int _currentTweenIndex;

        public TweenChain(T target, Action onCompleteAction = null, bool disposeOnComplete = true)
            : base(target, onCompleteAction, disposeOnComplete)
        {
            _currentTweenIndex = 0;
        }

        protected override bool OnUpdate(float deltaTime)
        {
            var currentTween = Tweens[_currentTweenIndex];

            currentTween.Update(deltaTime);

            if (currentTween.IsComplete)
                _currentTweenIndex++;

            return _currentTweenIndex >= Tweens.Count;
        }
    }
}