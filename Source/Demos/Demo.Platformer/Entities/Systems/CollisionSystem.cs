using System;
using System.Collections.Generic;
using System.Linq;
using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Components;

namespace Demo.Platformer.Entities.Systems
{
    public sealed class CollisionSystem : EntitySystem
    {
        private Dictionary<Entity, CollisionBody> _movingBodies;
        private Dictionary<Entity, CollisionBody> _staticBodies;

        public CollisionSystem(Vector2 gravity = new Vector2())
        {
            Gravity = gravity;
            _movingBodies = new Dictionary<Entity, CollisionBody>();
            _staticBodies = new Dictionary<Entity, CollisionBody>();
        }

        public Vector2 Gravity { get; set; }

        protected override void Update(Entity entity, GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var body = entity.GetComponent<CollisionBody>();

            if (body != null && !body.IsStatic)
            {
                body.Velocity += Gravity * deltaTime;
                body.Position += body.Velocity * deltaTime;

                entity.GetComponent<Transform>().Position = body.Position;

                foreach (var iter in _staticBodies.Concat(_movingBodies).Reverse())
                {
                    CollisionBody otherBody = iter.Value;

                    if (body == otherBody)
                        continue;

                    Vector2 depth = body.BoundingRectangle.IntersectionDepth(otherBody.BoundingRectangle);
                    if (depth != Vector2.Zero)
                        body.OnCollision?.Invoke(entity, iter.Key, depth);
                }
            }
        }

        protected override void ComponentAdded(Entity entity, Type type, object component)
        {
            if (entity.HasComponent(type) && type == typeof(CollisionBody))
            {
                var body = (CollisionBody)component;

                Action<bool> staticChanged = (value) =>
                {
                    if (value)
                    {
                        _movingBodies.Remove(entity);
                        _staticBodies.Add(entity, body);
                    }
                    else
                    {
                        _staticBodies.Remove(entity);
                        _movingBodies.Add(entity, body);
                    }
                };

                staticChanged(body.IsStatic);
                body.StaticChanged += staticChanged;
                body.OnCollision += Collision.BasicCollisionHandler;

                if (!entity.HasComponent<Transform>())
                    entity.AddComponent<Transform>();
            }
        }

        protected override void ComponentRemoved(Entity entity, Type type, object component)
        {
            if (entity.HasComponent(type) && type == typeof(CollisionBody))
            {
                var body = (CollisionBody)component;
                (body.IsStatic ? _staticBodies : _movingBodies).Remove(entity);
            }
        }
    }
}