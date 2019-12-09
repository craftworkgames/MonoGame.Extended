using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Sprites;

namespace SpaceGame.Entities
{
    //public class Explosion : Entity
    //{
    //    private readonly AnimatedSprite _sprite;
    //    private readonly Transform2 _transform;

    //    public Explosion(SpriteSheetAnimationFactory animations, Vector2 position, float radius)
    //    {
    //        _sprite = new AnimatedSprite(animations);
    //        _sprite.Play("explode", Destroy);
    //        _transform = new Transform2 {Position = position, Scale = Vector2.One * radius * 0.2f};
    //    }

    //    public override void Update(GameTime gameTime)
    //    {
    //        _sprite.Update(gameTime);
    //    }

    //    public override void Draw(SpriteBatch spriteBatch)
    //    {
    //        spriteBatch.Draw(_sprite, _transform);
    //    }
    //}
}