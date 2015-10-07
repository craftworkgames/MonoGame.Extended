using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Sprites
{
    public class SpriteSheetAnimator
    {
        public SpriteSheetAnimator(Sprite sprite, TextureAtlas textureAtlas)
        {
            _sprite = sprite;
            _textureAtlas = textureAtlas;
            _animations = new Dictionary<string, SpriteSheetAnimation>();
            _frameIndex = 0;

            IsPlaying = true;
            IsLooping = true;
        }

        private readonly Sprite _sprite;
        private readonly TextureAtlas _textureAtlas;
        private readonly Dictionary<string, SpriteSheetAnimation> _animations;
        private SpriteSheetAnimation _currentAnimation;
        private float _nextFrameDelay;
        private int _frameIndex;
        private Action _onCompleteAction;

        public bool IsPlaying { get; private set; }
        public bool IsLooping { get; set; }

        public void AddAnimation(string name, int framesPerSecond, params int[] frameIndices)
        {
            var animation = new SpriteSheetAnimation(name, framesPerSecond, frameIndices);
            _animations.Add(name, animation);
        }

        public void AddAnimation(string name, int framesPerSecond, int firstFrameIndex, int lastFrameIndex)
        {
            var frameIndices = new int[lastFrameIndex - firstFrameIndex + 1];

            for (var i = 0; i < frameIndices.Length; i++)
                frameIndices[i] = firstFrameIndex + i;

            AddAnimation(name, framesPerSecond, frameIndices);
        }

        public void RemoveAnimation(string name)
        {
            _animations.Remove(name);
        }

        public void PlayAnimation(string name, Action onCompleteAction = null)
        {
            if(_currentAnimation != null && _currentAnimation.Name == name)
                return;
            
            _currentAnimation = _animations[name];
            _frameIndex = 0;
            _onCompleteAction = onCompleteAction;
        }

        public void Update(GameTime gameTime)
        {
            if(!IsPlaying || _currentAnimation == null)
                return;

            var deltaSeconds = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (_nextFrameDelay <= 0)
            {
                _nextFrameDelay = 1.0f / _currentAnimation.FramesPerSecond;
                _frameIndex++;

                if (_frameIndex >= _currentAnimation.FrameIndicies.Length)
                {
                    if (IsLooping)
                        _frameIndex = 0;
                    else
                        IsPlaying = false;

                    var onCompleteAction = _onCompleteAction;

                    if (onCompleteAction != null)
                        onCompleteAction();
                }

                var atlasIndex = _currentAnimation.FrameIndicies[_frameIndex];
                _sprite.TextureRegion = _textureAtlas[atlasIndex];
            }

            _nextFrameDelay -= deltaSeconds;
        }

        private class SpriteSheetAnimation
        {
            public SpriteSheetAnimation(string name, int framesPerSecond, int[] frameIndicies)
            {
                Name = name;
                FramesPerSecond = framesPerSecond;
                FrameIndicies = frameIndicies;
            }

            public string Name { get; private set; }
            public int FramesPerSecond { get; private set; }
            public int[] FrameIndicies { get; private set; }
        }
    }
}