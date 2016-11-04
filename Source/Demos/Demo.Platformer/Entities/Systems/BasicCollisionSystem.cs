using System.Collections.Generic;
using System.Linq;
using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Entities.Systems;

namespace Demo.Platformer.Entities.Systems
{
    public class BasicCollisionSystem : ComponentSystem
    {
        private readonly Vector2 _gravity;
        private readonly List<BasicCollisionBody> _staticBodies = new List<BasicCollisionBody>();
        private readonly List<BasicCollisionBody> _movingBodies = new List<BasicCollisionBody>();

        public BasicCollisionSystem(Vector2 gravity)
        {
            _gravity = gravity;
        }

        protected override void OnComponentAttached(EntityComponent component)
        {
            var body = component as BasicCollisionBody;

            if (body != null)
            {
                if (body.IsStatic)
                    _staticBodies.Add(body);
                else
                    _movingBodies.Add(body);
            }

            base.OnComponentAttached(component);
        }

        protected override void OnComponentDetached(EntityComponent component)
        {
            var body = component as BasicCollisionBody;

            if (body != null)
            {
                if (body.IsStatic)
                    _staticBodies.Remove(body);
                else
                    _movingBodies.Remove(body);
            }

            base.OnComponentDetached(component);
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();

            foreach (var bodyA in _movingBodies)
            {
                bodyA.Velocity += _gravity * deltaTime;
                bodyA.Position += bodyA.Velocity * deltaTime;

                foreach (var bodyB in _staticBodies.Concat(_movingBodies))
                {
                    if (bodyA != bodyB)
                    {
                        var depth = bodyA.BoundingRectangle.IntersectionDepth(bodyB.BoundingRectangle);

                        if (depth != Vector2.Zero)
                        {
                            var collisionHandlers = bodyA.Entity.GetComponents<BasicCollisionHandler>();

                            foreach (var collisionHandler in collisionHandlers)
                                collisionHandler.OnCollision(bodyA, bodyB, depth);
                        }
                    }
                }
            }


        }
    }
}