using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Sprites
{
    public class SpriteSheetAnimator : IUpdate
    {
        public SpriteSheetAnimator(Sprite sprite, TextureAtlas textureAtlas)
            : this(sprite, textureAtlas.Select(t => t))
        {
        }

        public SpriteSheetAnimator(TextureAtlas textureAtlas)
            : this(null, textureAtlas)
        {
        }

        public SpriteSheetAnimator(IEnumerable<TextureRegion2D> regions)
            : this(null, regions)
        {
        }

        public SpriteSheetAnimator(Sprite sprite, IEnumerable<TextureRegion2D> regions)
        {
            Sprite = sprite;
            _frames = new List<TextureRegion2D>(regions);
            _animations = new Dictionary<string, SpriteSheetAnimation>();
            _frameIndex = 0;

            IsPlaying = true;
            IsLooping = true;
        }

        private readonly List<TextureRegion2D> _frames;
        private readonly Dictionary<string, SpriteSheetAnimation> _animations;
        private SpriteSheetAnimation _currentAnimation;
        private float _nextFrameDelay;
        private int _frameIndex;
        private Action _onCompleteAction;

        public Sprite Sprite { get; set; }
        public bool IsPlaying { get; private set; }
        public bool IsLooping { get; set; }

        public int AddFrame(TextureRegion2D textureRegion)
        {
            var index = _frames.Count;
            _frames.Add(textureRegion);
            return index;
        }

        public bool RemoveFrame(TextureRegion2D textureRegion)
        {
            return _frames.Remove(textureRegion);
        }

        public void RemoveFrameAt(int frameIndex)
        {
            _frames.RemoveAt(frameIndex);
        }

        public void AddAnimation(string name, int framesPerSecond, params int[] frameIndices)
        {
            _animations.Add(name, new SpriteSheetAnimation(name, framesPerSecond, frameIndices));
        }

        public void AddAnimation(string name, int framesPerSecond, int firstFrameIndex, int lastFrameIndex)
        {
            var frameIndices = new int[lastFrameIndex - firstFrameIndex + 1];

            for (var i = 0; i < frameIndices.Length; i++)
                frameIndices[i] = firstFrameIndex + i;

            AddAnimation(name, framesPerSecond, frameIndices);
        }

        public bool RemoveAnimation(string name)
        {
            if (!_animations.ContainsKey(name))
                return false;

            _animations.Remove(name);
            return true;
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

                if(Sprite != null)
                    Sprite.TextureRegion = _frames[atlasIndex];
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