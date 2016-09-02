using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Animations.SpriteSheets
{
    public class AnimatedSprite : Sprite
    {
        public AnimatedSprite(SpriteSheetAnimationFactory animationFactory, string playAnimation = null)
            : base(animationFactory.Frames[0])
        {
            _animationFactory = animationFactory;

            if(playAnimation != null)
                Play(playAnimation);
        }

        private readonly SpriteSheetAnimationFactory _animationFactory;
        private SpriteSheetAnimation _currentAnimation;

        public SpriteSheetAnimation Play(string name, Action onCompleted = null)
        {
            if (_currentAnimation == null || _currentAnimation.IsComplete || _currentAnimation.Name != name)
            {
                _currentAnimation = _animationFactory.Create(name);
                _currentAnimation.OnCompleted = onCompleted;
            }

            return _currentAnimation;
        }

        public void Update(float deltaTime)
        {
            if (_currentAnimation != null && !_currentAnimation.IsComplete)
            {
                _currentAnimation.Update(deltaTime);
                TextureRegion = _currentAnimation.CurrentFrame;
            }
        }

        public void Update(GameTime gameTime)
        {
            Update(gameTime.GetElapsedSeconds());
        }
    }
}