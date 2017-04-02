using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;

namespace Demo.Platformer.Entities.Systems
{
    [Aspect(AspectType.All, typeof(HealthComponent), typeof(TransformComponent))]
    [EntitySystem(GameLoopType.Update, Layer = 1)]
    public class DeathSystem : EntityProcessingSystem
    {
        protected override void Process(GameTime gameTime, Entity entity)
        {
            var health = entity.Get<HealthComponent>();
            if (health.IsAlive)
                return;

            var transform = entity.Get<TransformComponent>();
            var explosionEntity = EntityManager.CreateEntityFromTemplate("blood-explosion");
            var explosionTransform = explosionEntity.Get<TransformComponent>();
            explosionTransform.Position = transform.WorldPosition;

            //TODO: if entity has player component, then do something like game over screen...

            entity.Destroy();
        }
    }
}