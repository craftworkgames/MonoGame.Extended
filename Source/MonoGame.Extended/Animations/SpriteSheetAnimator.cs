using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Animations
{
    public class SpriteSheetAnimation
    {
        public SpriteSheetAnimation(string name, int[] frameIndices)
        {
            Name = name;
            FrameIndices = frameIndices;
        }

        public string Name { get; private set; }
        public int[] FrameIndices { get; private set; }
    }

    public class SpriteSheetAnimator
    {
        public SpriteSheetAnimator(Sprite targetSprite, TextureAtlas textureAtlas)
        {
            _sprite = targetSprite;
            _textureAtlas = textureAtlas;
            _animations = new Dictionary<string, SpriteSheetAnimation>();

            IsPlaying = true;
            FramesPerSecond = 15;
        }

        private readonly Sprite _sprite;
        private readonly TextureAtlas _textureAtlas;
        private readonly Dictionary<string, SpriteSheetAnimation> _animations;
        private SpriteSheetAnimation _currentAnimation;

        private int _frameIndex;
        private float _nextFrameDelay;

        public bool IsPlaying { get; private set; }
        public bool IsLooping { get; private set; }
        public int FramesPerSecond { get; private set; }

        public void AddAnimation(string name, int[] frameIndices)
        {
            var animation = new SpriteSheetAnimation(name, frameIndices);
            _animations.Add(name, animation);
        }

        public void SetAnimation(string name)
        {
            _currentAnimation = _animations[name];
        }

        public void Update(GameTime gameTime)
        {
            if (IsPlaying && FramesPerSecond > 0)
            {
                var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_nextFrameDelay <= 0)
                {
                    _nextFrameDelay = 1.0f / FramesPerSecond;
                    _frameIndex++;

                    if (_frameIndex >= _textureAtlas.RegionCount)
                    {
                        if (IsLooping)
                            _frameIndex = 0;
                        else
                            IsPlaying = false;
                    }

                    _sprite.TextureRegion = _textureAtlas[_frameIndex];
                }

                _nextFrameDelay -= deltaSeconds;
            }
        }
    }
}
