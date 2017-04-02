using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using TransformComponent = Demo.Platformer.Entities.Components.TransformComponent;

namespace Demo.Platformer.Entities.Systems
{
    [Aspect(AspectType.All, typeof(CollisionBodyComponent), typeof(TransformComponent))]
    [EntitySystem(GameLoopType.Update, Layer = 0)]
    public class PhysicsSystem : EntityProcessingSystem
    {
        private readonly Vector2 _gravity;

        public PhysicsSystem()
        {
            _gravity = new Vector2(0, 0);
        }

        protected override void Process(GameTime gameTime, Entity entity)
        {
            var transform = entity.Get<TransformComponent>();
            var body = entity.Get<CollisionBodyComponent>();

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