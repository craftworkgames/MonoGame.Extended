using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace SpaceGame.Entities
{
    public class Explosion : Entity
    {
        private readonly SpriteSheetAnimator _animator;

        public Explosion(SpriteSheetAnimator animator, Vector2 position)
        {
            _animator = animator;
            _animator.Sprite.Position = position;
            _animator.Sprite.Scale = Vector2.One*0.2f;
            _animator.IsLooping = false;
            _animator.PlayAnimation("explode", Destroy);
        }

        public override void Update(GameTime gameTime)
        {
            _animator.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_animator);
        }
    }
}