using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public class TweenAnimation<T> : IAnimation
    {
        private readonly Action _onComplete;

        public TweenAnimation(T target, Action onComplete)
        {
            _onComplete = onComplete;
            Target = target;
            Tweens = new List<IAnimation>();
            //Chains = new List<IAnimation>();
        }

        public void Dispose()
        {
        }

        public T Target { get; }
        public IList<IAnimation> Tweens { get; }
        //public IList<IAnimation> Chains { get; } 

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
                        _onComplete?.Invoke();
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (IsComplete)
                return;

            foreach (var animation in Tweens)
                animation.Update(gameTime);

            IsComplete = Tweens.All(t => t.IsComplete);
        }
    }
}