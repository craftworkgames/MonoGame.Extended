using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public abstract class TweenAnimation<T> : IAnimation
    {
        protected TweenAnimation(T target)
        {
            Target = target;
            Tweens = new List<IAnimation>();
        } 

        public T Target { get; }
        public IList<IAnimation> Tweens { get; }

        private bool _isComplete;
        public bool IsComplete
        {
            get {  return _isComplete; }
            protected set
            {
                if (_isComplete != value)
                {
                    _isComplete = value;

                    if (_isComplete)
                        OnCompleteAction?.Invoke();
                }
            }
        }

        public Action OnCompleteAction { get; set; }

        public abstract void Update(GameTime gameTime);
    }
}