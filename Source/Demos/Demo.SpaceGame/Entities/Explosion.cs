using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Sprites;

namespace Demo.SpaceGame.Entities
{
    public class Explosion : Entity
    {
        private readonly SpriteSheetAnimator _animator;
        private Sprite _sprite;

        public Explosion(SpriteSheetAnimationGroup animations, Vector2 position, float radius)
        {
            _animator = new SpriteSheetAnimator(animations) { IsLooping = false };
            _sprite = new Sprite(animations.GetFrame(0)) { Position = position, Scale = Vector2.One * radius * 0.2f };
            _animator.PlayAnimation("explode", Destroy);
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