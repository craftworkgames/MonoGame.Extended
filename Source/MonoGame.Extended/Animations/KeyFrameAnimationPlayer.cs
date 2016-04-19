using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;

namespace MonoGame.Extended.Animations
{
    public class KeyFrameAnimationPlayer : IUpdate
    {
        public KeyFrameAnimationPlayer()
        {
            _animations = new Dictionary<string, KeyFrameAnimation>();
        }

        public KeyFrameAnimationPlayer(KeyFrameAnimationCollection animations)
        {
            _animations = animations.ToDictionary(a => a.Name);
            CurrentAnimation = _animations.Values.FirstOrDefault();
        }

        private readonly Dictionary<string, KeyFrameAnimation> _animations;
        private Action _onComplete;

        public Sprite TargetSprite { get; set; }
        public KeyFrameAnimation CurrentAnimation { get; private set; }

        public void Add(KeyFrameAnimation animation)
        {
            if (animation.Name == null)
                throw new InvalidOperationException("Animations must be named.");

            if (!_animations.Values.Any())
                CurrentAnimation = animation;
            
            _animations.Add(animation.Name, animation);
        }

        public bool Remove(string name)
        {
            return _animations.Remove(name);
        }

        public KeyFrameAnimation Get(string name)
        {
            return _animations[name];
        }

        public KeyFrameAnimation this[string name] => Get(name);

        public void Play(string name, Action onComplete = null)
        {
            if (CurrentAnimation.IsComplete || CurrentAnimation.Name != name)
                CurrentAnimation.Rewind();

            CurrentAnimation = _animations[name];
            _onComplete = onComplete;
        }

        public void Update(float deltaTime)
        {
            if (CurrentAnimation != null && !CurrentAnimation.IsComplete)
            {
                CurrentAnimation.Update(deltaTime);

                if (TargetSprite != null)
                    TargetSprite.TextureRegion = CurrentAnimation.CurrentFrame;

                if (CurrentAnimation.IsComplete)
                    _onComplete?.Invoke();
            }
        }

        public void Update(GameTime gameTime)
        {
            Update(gameTime.GetElapsedSeconds());
        }

        public Sprite CreateSprite(Vector2 position)
        {
            return TargetSprite = new Sprite(CurrentAnimation.CurrentFrame)
            {
                Position = position
            };
        }

        public Sprite CreateSprite()
        {
            return CreateSprite(Vector2.Zero);
        }
    }
}