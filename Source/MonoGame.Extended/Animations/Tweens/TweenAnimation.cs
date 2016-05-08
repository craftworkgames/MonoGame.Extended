using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations.Tweens
{
    public interface ITweenAnimation<out T>
    {
        T Target { get; }
        IList<IAnimation> Tweens { get; }
    }

    public class TweenAnimation<T> : IAnimation, ITweenAnimation<T>
    {
        public TweenAnimation(T target)
        {
            Target = target;
            Tweens = new List<IAnimation>();
        } 

        public T Target { get; }
        public IList<IAnimation> Tweens { get; }
        public bool IsComplete { get; private set; }

        public void Update(GameTime gameTime)
        {
            if(IsComplete)
                return;

            foreach (var animation in Tweens)
                animation.Update(gameTime);

            IsComplete = Tweens.All(t => t.IsComplete);
        }
    }

    public class TweenSequence<T> : IAnimation, ITweenAnimation<T>
    {
        public TweenSequence(T target)
        {
            Target = target;
            Tweens = new List<IAnimation>();
            _currentTweenIndex = 0;
        }

        public T Target { get; }
        public IList<IAnimation> Tweens { get; }
        public bool IsComplete { get; private set; }

        private int _currentTweenIndex;

        public void Update(GameTime gameTime)
        {
            if(IsComplete)
                return;

            var currentTween = Tweens[_currentTweenIndex];

            currentTween.Update(gameTime);

            if (currentTween.IsComplete)
                _currentTweenIndex++;

            if (_currentTweenIndex >= Tweens.Count)
                IsComplete = true;
        }
    }
}