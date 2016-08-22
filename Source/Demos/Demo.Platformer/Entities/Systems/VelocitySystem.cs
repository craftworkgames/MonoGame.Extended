using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities.Systems;

namespace Demo.Platformer.Entities.Systems
{
    public class VelocitySystem : UpdatableComponentSystem
    {
        private readonly Vector2 _gravity;

        public VelocitySystem(Vector2 gravity)
        {
            _gravity = gravity;
        }

        public override void Update(GameTime gameTime)
        {
            var components = GetComponents<BasicCollisionComponent>();
            var deltaTime = gameTime.GetElapsedSeconds();

            foreach (var velocityComponent in components)
            {
                velocityComponent.Velocity += _gravity * deltaTime;
                velocityComponent.Position += velocityComponent.Velocity * deltaTime;
            }
        }
    }
}
