using System;
using System.Linq;

namespace MonoGame.Extended.Animations.Tweens
{
    public class TweenGroup<T> : TweenAnimation<T>
    {
        public TweenGroup(T target, Action onCompleteAction = null, bool disposeOnComplete = true)
            : base(target, onCompleteAction, disposeOnComplete)
        {
        }

        protected override bool OnUpdate(float deltaTime)
        {
            foreach (var animation in Tweens)
                animation.Update(deltaTime);

            return Tweens.All(t => t.IsComplete);
        }
    }
}