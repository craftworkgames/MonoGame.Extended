using System;
using System.Collections.Generic;
using MonoGame.Extended.Animations;

namespace MonoGame.Extended.Tweening
{
    public abstract class TweenAnimation<T> : Animation
    {
        protected TweenAnimation(T target, Action onCompleteAction = null, bool disposeOnComplete = true)
            : base(onCompleteAction, disposeOnComplete)
        {
            Target = target;
            Tweens = new List<Animation>();
        }

        public T Target { get; }
        public IList<Animation> Tweens { get; }
    }
}