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
        AspectType = AspectType.AllOf,
        ComponentTypes = new[]
        {
            typeof(TransformComponent), typeof(EnemyAiComponent), typeof(SpriteComponent)
        })]
    public class EnemyMovementSystem : EntityProcessingSystem<EnemyAiComponent, TransformComponent, SpriteComponent>
    {
        protected override void Process(GameTime gameTime, Entity entity, EnemyAiComponent enemy, TransformComponent transform, SpriteComponent sprite)
        {
            var deltaTime = gameTime.GetElapsedSeconds();

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