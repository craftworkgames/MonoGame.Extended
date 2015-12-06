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
            _frames = new List<TextureRegion2D>(regions);
            _animations = new Dictionary<string, SpriteSheetAnimation>();
            _frameIndex = 0;

            Sprite = sprite;
            IsPlaying = true;
            IsLooping = true;

            if (Sprite != null && _frames.Any())
                Sprite.TextureRegion = _frames.First();
        }

        public SpriteSheetAnimator()
            : this(null, Enumerable.Empty<TextureRegion2D>())
        {
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

        public IEnumerable<TextureRegion2D> Frames
        {
            get { return _frames; }
        }

        public IEnumerable<string> Animations
        {
            get { return _animations.Keys.OrderBy(i => i); }
        }

        public int AddFrame(TextureRegion2D textureRegion)
        {
            var index = _frames.Count;
            _frames.Add(textureRegion);

            if (Sprite != null && Sprite.TextureRegion == null)
                Sprite.TextureRegion = textureRegion;

            return index;
        }

        public bool RemoveFrame(TextureRegion2D textureRegion)
        {
            return _frames.Remove(textureRegion);
        }

        public bool RemoveFrame(string name)
        {
            var frame = GetFrame(name);
            return RemoveFrame(frame);
        }

        public void RemoveFrameAt(int frameIndex)
        {
            _frames.RemoveAt(frameIndex);
        }

        public TextureRegion2D GetFrameAt(int index)
        {
            return _frames[index];
        }

        public TextureRegion2D GetFrame(string name)
        {
            return _frames.FirstOrDefault(f => f.Name == name);
        }

        public SpriteSheetAnimation AddAnimation(string name, int framesPerSecond, int[] frameIndices)
        {
            if (_animations.ContainsKey(name))
                throw new InvalidOperationException(string.Format("Animator already contrains an animation called {0}", name));

            var animation = new SpriteSheetAnimation(name, framesPerSecond, frameIndices);
            _animations.Add(name, animation);
            return animation;
        }

        public SpriteSheetAnimation AddAnimation(string name, int framesPerSecond, int firstFrameIndex, int lastFrameIndex)
        {
            var frameIndices = new int[lastFrameIndex - firstFrameIndex + 1];

            for (var i = 0; i < frameIndices.Length; i++)
                frameIndices[i] = firstFrameIndex + i;

            return AddAnimation(name, framesPerSecond, frameIndices);
        }

        public bool RemoveAnimation(string name)
        {
            SpriteSheetAnimation animation;

            if (!_animations.TryGetValue(name, out animation))
                return false;

            if (_currentAnimation == animation)
                _currentAnimation = null;

            _animations.Remove(name);
            return true;
        }

        public void PlayAnimation(SpriteSheetAnimation animation, Action onCompleteAction = null)
        {
            if (!_animations.ContainsValue(animation))
                throw new InvalidOperationException("Animation does not belong to this animator");

            PlayAnimation(animation.Name);
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
            if(!IsPlaying || _currentAnimation == null || _currentAnimation.FrameIndicies.Length == 0)
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
    }
}