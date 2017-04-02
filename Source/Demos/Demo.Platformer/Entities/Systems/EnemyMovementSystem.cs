using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;

namespace Demo.Platformer.Entities.Systems
{
    [Aspect(AspectType.All, typeof(TransformComponent), typeof(EnemyAiComponent), typeof(SpriteComponent))]
    [EntitySystem(GameLoopType.Update, Layer = 0)]
    public class EnemyMovementSystem : EntityProcessingSystem
    {
        protected override void Process(GameTime gameTime, Entity entity)
        {
            var enemy = entity.Get<EnemyAiComponent>();
            var transform = entity.Get<TransformComponent>();
            var sprite = entity.Get<SpriteComponent>();

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