using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using Platformer.Components;

namespace Platformer.Systems
{
    [Aspect(AspectType.All, typeof(VelocityComponent), typeof(Transform2))]
    [EntitySystem(GameLoopType.Update, Layer = 0)]
    public class MovementSystem : EntityProcessingSystem
    {
        protected override void Process(GameTime gameTime, Entity entity)
        {
            var movement = entity.Get<VelocityComponent>();
            var transform = entity.Get<Transform2>();

            var elapsedSeconds = gameTime.GetElapsedSeconds();

            transform.Position += movement.Velocity * elapsedSeconds;
            movement.Velocity += new Vector2(0, 1600) * elapsedSeconds;

            const int floor = 480 - 64;

            if (transform.Position.Y >= floor)
            {
                movement.Velocity = new Vector2(movement.Velocity.X, 0);
                transform.Position = new Vector2(transform.Position.X, floor);
            }
        }
    }
}