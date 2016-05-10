using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public class TweenAnimation<T> : IAnimation
    {
        private readonly Action _onCompleteAction;
        private readonly bool _disposeOnComplete;

        public TweenAnimation(T target, Action onCompleteAction = null, bool disposeOnComplete = true)
        {
            _onCompleteAction = onCompleteAction;
            _disposeOnComplete = disposeOnComplete;
            Target = target;
            Tweens = new List<IAnimation>();
        }

        public void Dispose()
        {
            IsDisposed = true;
        }

        public bool IsDisposed { get; private set; }

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
                    {
                        _onCompleteAction?.Invoke();

                        if (_disposeOnComplete)
                            Dispose();
                    }
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