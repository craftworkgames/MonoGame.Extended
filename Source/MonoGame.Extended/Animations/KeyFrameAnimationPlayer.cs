using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Animations
{
    public class KeyFrameAnimationPlayer : IUpdate
    {
        public KeyFrameAnimationPlayer()
            : this(new KeyFrameAnimationDictionary())
        {
        }

        public KeyFrameAnimationPlayer(KeyFrameAnimationDictionary animations)
        {
            _animations = animations;
            _currentAnimation = _animations.Values.FirstOrDefault();
        }

        private readonly KeyFrameAnimationDictionary _animations;
        private KeyFrameAnimation _currentAnimation;

        public void Add(string name, KeyFrameAnimation animation)
        {
            if (!_animations.Values.Any())
                _currentAnimation = animation;

            _animations.Add(name, animation);
        }

        public bool Remove(string name)
        {
            return _animations.Remove(name);
        }

        public KeyFrameAnimation Get(string name)
        {
            return _animations[name];
        }

        public void Play(string name)
        {
            _currentAnimation = _animations[name];
        }

        public void Update(float deltaTime)
        {
            _currentAnimation?.Update(deltaTime);
        }

        public void Update(GameTime gameTime)
        {
            Update(gameTime.GetElapsedSeconds());
        }
    }
}