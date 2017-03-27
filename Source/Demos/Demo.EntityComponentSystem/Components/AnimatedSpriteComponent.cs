using System;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Entities;

namespace Demo.EntityComponentSystem.Components
{
    [Component]
    [ComponentPool(InitialSize = 20)]
    public class AnimatedSpriteComponent : Component
    {
        private SpriteSheetAnimationFactory _animationFactory;

        public SpriteSheetAnimationFactory AnimationFactory
        {
            get { return _animationFactory; }
            set
            {
                if (_animationFactory == value)
                    return;

                _animationFactory = value;
                if (CurrentAnimation != null)
                    CurrentAnimation.OnCompleted = null;
                CurrentAnimation = null;
            }
        }

        public SpriteSheetAnimation CurrentAnimation { get; private set; }

        public override void Reset()
        {
            AnimationFactory = null;
            CurrentAnimation = null;
        }

        public SpriteSheetAnimation Play(string name, Action onCompleted = null)
        {
            if (CurrentAnimation != null && !CurrentAnimation.IsComplete && CurrentAnimation.Name == name)
                return CurrentAnimation;

            CurrentAnimation = AnimationFactory.Create(name);
            CurrentAnimation.OnCompleted = onCompleted;

            return CurrentAnimation;
        }
    }
}
