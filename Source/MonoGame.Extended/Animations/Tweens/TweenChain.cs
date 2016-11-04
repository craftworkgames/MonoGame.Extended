using System;

namespace MonoGame.Extended.Animations.Tweens
{
    public class TweenChain<T> : TweenAnimation<T>
    {
        public TweenChain(T target, Action onCompleteAction = null, bool disposeOnComplete = true) 
            : base(target, onCompleteAction, disposeOnComplete)
        {
            _currentTweenIndex = 0;
        }

        private int _currentTweenIndex;

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