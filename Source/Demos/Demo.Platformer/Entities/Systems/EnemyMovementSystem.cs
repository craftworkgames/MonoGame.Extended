using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Components;

namespace Demo.Platformer.Entities.Systems
{
    public sealed class EnemyMovementSystem : EntitySystem
    {
        protected override void Update(Entity entity, GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var component = entity.GetComponent<Enemy>();
            if (component != null)
            {
                entity.GetComponent<CollisionBody>().Position += component.Direction * deltaTime;
                component.WalkTimeRemaining -= deltaTime;

                if (component.WalkTimeRemaining <= 0f)
                {
                    var sprite = entity.GetComponent<SpriteComponent>();
                    sprite.Effect = FlipSpriteEffect(sprite.Effect);

                    component.Direction = -component.Direction;
                    component.WalkTimeRemaining = component.WalkTime;
                }
            }
        }

        private SpriteEffects FlipSpriteEffect(SpriteEffects effect)
        {
            return effect == SpriteEffects.None
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;
        }
    }
}