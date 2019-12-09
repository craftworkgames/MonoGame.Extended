using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Sprites
{
    public class AnimatedSprite : Sprite
    {
        private readonly SpriteSheet _spriteSheet;
        private SpriteSheetAnimation _currentAnimation;

        public AnimatedSprite(SpriteSheet spriteSheet, string playAnimation = null)
            : base(spriteSheet.TextureAtlas[0])
        {
            _spriteSheet = spriteSheet;

            if (playAnimation != null)
                Play(playAnimation);
        }

        public SpriteSheetAnimation Play(string name, Action onCompleted = null)
        {
            if (_currentAnimation == null || _currentAnimation.IsComplete || _currentAnimation.Name != name)
            {
                var cycle = _spriteSheet.Cycles[name];
                var keyFrames = cycle.Frames.Select(f => _spriteSheet.TextureAtlas[f.Index]).ToArray();
                _currentAnimation = new SpriteSheetAnimation(name, keyFrames, cycle.FrameDuration, cycle.IsLooping, cycle.IsReversed, cycle.IsPingPong);

                if(_currentAnimation != null)
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