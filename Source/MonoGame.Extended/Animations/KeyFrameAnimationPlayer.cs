using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;

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
            CurrentAnimation = _animations.Values.FirstOrDefault();
        }

        private readonly KeyFrameAnimationDictionary _animations;

        public Sprite TargetSprite { get; set; }
        public KeyFrameAnimation CurrentAnimation { get; private set; }

        public void Add(string name, KeyFrameAnimation animation)
        {
            if (!_animations.Values.Any())
                CurrentAnimation = animation;

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
            CurrentAnimation = _animations[name];
        }

        public void Update(float deltaTime)
        {
            if (CurrentAnimation != null)
            {
                CurrentAnimation.Update(deltaTime);

                if (TargetSprite != null)
                    TargetSprite.TextureRegion = CurrentAnimation.CurrentFrame;
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