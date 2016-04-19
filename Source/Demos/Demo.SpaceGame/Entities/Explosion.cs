using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Sprites;

namespace Demo.SpaceGame.Entities
{
    public class Explosion : Entity
    {
        private readonly KeyFrameAnimationPlayer _animator;
        private readonly Sprite _sprite;

        public Explosion(KeyFrameAnimationCollection animations, Vector2 position, float radius)
        {
            _animator = new KeyFrameAnimationPlayer(animations);
            _sprite = _animator.CreateSprite();
            _sprite.Position = position;
            _sprite.Scale = Vector2.One*radius*0.2f;
            _animator.Play("explode", Destroy);
        }

        public override void Update(GameTime gameTime)
        {
            _animator.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite);
        }
    }
}