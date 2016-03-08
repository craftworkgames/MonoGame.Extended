using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Animations
{
    public class SpriteSheetAnimator : IUpdate
    {
        //public SpriteSheetAnimator()
        //    : this(null, Enumerable.Empty<TextureRegion2D>())
        //{
        //}

        private readonly SpriteSheetAnimationGroup _animationGroup;
        private SpriteSheetAnimation _currentAnimation;
        private int _frameIndex;
        private float _nextFrameDelay;
        private Action _onCompleteAction;
        public bool IsLooping { get; set; }
        public bool IsPlaying { get; private set; }

        public Sprite Sprite { get; set; }
        //public SpriteSheetAnimator(Sprite sprite, TextureAtlas textureAtlas)
        //    : this(sprite, textureAtlas.Select(t => t))
        //{
        //}

        //public SpriteSheetAnimator(TextureAtlas textureAtlas)
        //    : this(null, textureAtlas)
        //{
        //}

        //public SpriteSheetAnimator(IEnumerable<TextureRegion2D> regions)
        //    : this(null, regions)
        //{
        //}

        public SpriteSheetAnimator(SpriteSheetAnimationGroup animationGroup)
            : this(animationGroup, new Sprite(animationGroup.Frames.First()))
        {
        }

        public SpriteSheetAnimator(SpriteSheetAnimationGroup animationGroup, Sprite sprite)
        {
            _animationGroup = animationGroup;
            //_frames = new List<TextureRegion2D>(regions);
            //_animations = new Dictionary<string, SpriteSheetAnimation>();
            _frameIndex = 0;

            Sprite = sprite;
            IsPlaying = true;
            IsLooping = true;

            if (Sprite != null && _animationGroup.Frames.Any())
            {
                Sprite.TextureRegion = _animationGroup.Frames.First();
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!IsPlaying || _currentAnimation == null || _currentAnimation.FrameIndicies.Length == 0)
            {
                return;
            }

            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_nextFrameDelay <= 0)
            {
                _nextFrameDelay = 1.0f / _currentAnimation.FramesPerSecond;
                _frameIndex++;

                if (_frameIndex >= _currentAnimation.FrameIndicies.Length)
                {
                    _frameIndex = 0;

                    if (!IsLooping)
                    {
                        IsPlaying = false;
                    }

                    var onCompleteAction = _onCompleteAction;

                    if (onCompleteAction != null)
                    {
                        onCompleteAction();
                    }
                }

                var atlasIndex = _currentAnimation.FrameIndicies[_frameIndex];

                if (Sprite != null)
                {
                    Sprite.TextureRegion = _animationGroup.GetFrame(atlasIndex);
                }
            }

            _nextFrameDelay -= deltaSeconds;
        }

        //public IEnumerable<TextureRegion2D> Frames
        //{
        //    get { return _frames; }
        //}

        //public IEnumerable<string> Animations
        //{
        //    get { return _animations.Keys.OrderBy(i => i); }
        //}

        //public int AddFrame(TextureRegion2D textureRegion)
        //{
        //    var index = _frames.Count;
        //    _frames.Add(textureRegion);

        //    if (Sprite != null && Sprite.TextureRegion == null)
        //        Sprite.TextureRegion = textureRegion;

        //    return index;
        //}

        //public bool RemoveFrame(TextureRegion2D textureRegion)
        //{
        //    return _frames.Remove(textureRegion);
        //}

        //public bool RemoveFrame(string name)
        //{
        //    var frame = GetFrame(name);
        //    return RemoveFrame(frame);
        //}

        //public void RemoveFrameAt(int frameIndex)
        //{
        //    _frames.RemoveAt(frameIndex);
        //}

        //public TextureRegion2D GetFrameAt(int index)
        //{
        //    return _frames[index];
        //}

        //public TextureRegion2D GetFrame(string name)
        //{
        //    return _frames.FirstOrDefault(f => f.Name == name);
        //}

        //public SpriteSheetAnimation AddAnimation(string name, int framesPerSecond, int[] frameIndices)
        //{
        //    if (_animations.ContainsKey(name))
        //        throw new InvalidOperationException(string.Format("Animator already contrains an animation called {0}", name));

        //    var animation = new SpriteSheetAnimation(name, framesPerSecond, frameIndices);
        //    _animations.Add(name, animation);
        //    return animation;
        //}

        //public SpriteSheetAnimation AddAnimation(string name, int framesPerSecond, int firstFrameIndex, int lastFrameIndex)
        //{
        //    var frameIndices = new int[lastFrameIndex - firstFrameIndex + 1];

        //    for (var i = 0; i < frameIndices.Length; i++)
        //        frameIndices[i] = firstFrameIndex + i;

        //    return AddAnimation(name, framesPerSecond, frameIndices);
        //}

        //public bool RemoveAnimation(string name)
        //{
        //    SpriteSheetAnimation animation;

        //    if (!_animations.TryGetValue(name, out animation))
        //        return false;

        //    if (_currentAnimation == animation)
        //        _currentAnimation = null;

        //    _animations.Remove(name);
        //    return true;
        //}

        public void PlayAnimation(SpriteSheetAnimation animation, Action onCompleteAction = null)
        {
            if (!_animationGroup.Contains(animation))
            {
                throw new InvalidOperationException("Animation does not belong to this animator");
            }

            PlayAnimation(animation.Name);
        }

        public void PlayAnimation(string name, Action onCompleteAction = null)
        {
            if (_currentAnimation != null && _currentAnimation.Name == name && IsPlaying)
            {
                return;
            }

            _currentAnimation = _animationGroup.GetAnimation(name);
            _frameIndex = 0;
            _onCompleteAction = onCompleteAction;
            IsPlaying = true;
        }
    }
}