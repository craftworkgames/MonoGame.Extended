using Microsoft.Xna.Framework;
using MonoGame.Extended.Animations;

namespace MonoGame.Extended.Tweening
{
    public class TweeningComponent : GameComponent
    {
        private readonly AnimationComponent _animationComponent;

        public TweeningComponent(Game game, AnimationComponent animationComponent)
            : base(game)
        {
            _animationComponent = animationComponent;
            FluentTweening.TweeningComponent = this;
        }

        public void AddTween(Animation tweenAnimation)
        {
            _animationComponent.Animations.Add(tweenAnimation);
        }

        public bool RemoveTween(Animation tweenAnimation)
        {
            return _animationComponent.Animations.Remove(tweenAnimation);
        }
    }
}
