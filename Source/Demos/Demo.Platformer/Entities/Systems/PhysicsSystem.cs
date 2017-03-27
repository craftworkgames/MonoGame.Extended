using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using TransformComponent = Demo.Platformer.Entities.Components.TransformComponent;

namespace Demo.Platformer.Entities.Systems
{
    [System(
        Layer = 0,
        GameLoopType = GameLoopType.Update,
        AspectType = AspectType.AllOf,
        ComponentTypes = new[]
        {
            typeof(BasicCollisionBodyComponent), typeof(TransformComponent)
        })]
    public class PhysicsSystem : EntityProcessingSystem<TransformComponent, BasicCollisionBodyComponent>
    {
        private readonly Vector2 _gravity;

        public PhysicsSystem()
        {
            _gravity = new Vector2(0, 0);
        }

        protected override void Process(GameTime gameTime, Entity entity, TransformComponent transform, BasicCollisionBodyComponent body)
        {
            if (!body.IsStatic)
            {
                var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
                body.Velocity += _gravity * delta;
                transform.Position += body.Velocity * delta;
            }

            body.BoundingRectangle = new RectangleF(transform.Position - body.Size * body.Origin,
                body.Size);
        }
    }
}