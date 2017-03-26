using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;

namespace Demo.Platformer.Entities.Systems
{
    [System(
        Layer = 0,
        GameLoopType = GameLoopType.Update,
        AspectType = AspectType.RequiresAllOf,
        ComponentTypes = new[]
        {
            typeof(TransformComponent), typeof(EnemyAiComponent)
        })]
    public class EnemyMovementSystem : MonoGame.Extended.Entities.System
    {
        protected override void Process(GameTime gameTime, Entity entity)
        {
            var deltaTime = gameTime.GetElapsedSeconds();

            var enemy = entity.GetComponent<EnemyAiComponent>();
            var transform = entity.GetComponent<TransformComponent>();
            var sprite = entity.GetComponent<SpriteComponent>();
            transform.Position += enemy.Direction * deltaTime;
            enemy.WalkTimeRemaining -= deltaTime;

            if (enemy.WalkTimeRemaining > 0)
                return;

            sprite.Effects = sprite.Effects == SpriteEffects.None ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            enemy.Direction = -enemy.Direction;
            enemy.WalkTimeRemaining = enemy.WalkTime;
        }
    }
}