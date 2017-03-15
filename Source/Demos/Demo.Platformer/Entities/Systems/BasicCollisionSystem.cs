using System;
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
                bodyA.Entity.Position += bodyA.Velocity * deltaTime;

                foreach (var bodyB in _staticBodies.Concat(_movingBodies))
                {
                    if (bodyA == bodyB)
                        continue;

                    var depth = IntersectionDepth(bodyA.BoundingRectangle, bodyB.BoundingRectangle);

                    if (depth != Vector2.Zero)
                    {
                        var collisionHandlers = bodyA.Entity.GetComponents<BasicCollisionHandler>();

                        foreach (var collisionHandler in collisionHandlers)
                            collisionHandler.OnCollision(bodyA, bodyB, depth);
                    }
                }
            }


        }

        public Vector2 IntersectionDepth(RectangleF first, RectangleF second)
        {
            // Calculate half sizes.
            var thisHalfWidth = first.Width / 2.0f;
            var thisHalfHeight = first.Height / 2.0f;
            var otherHalfWidth = second.Width / 2.0f;
            var otherHalfHeight = second.Height / 2.0f;

            // Calculate centers.
            var centerA = new Vector2(first.Left + thisHalfWidth, first.Top + thisHalfHeight);
            var centerB = new Vector2(second.Left + otherHalfWidth, second.Top + otherHalfHeight);

            // Calculate current and minimum-non-intersecting distances between centers.
            var distanceX = centerA.X - centerB.X;
            var distanceY = centerA.Y - centerB.Y;
            var minDistanceX = thisHalfWidth + otherHalfWidth;
            var minDistanceY = thisHalfHeight + otherHalfHeight;

            // If we are not intersecting at all, return (0, 0).
            if ((Math.Abs(distanceX) >= minDistanceX) || (Math.Abs(distanceY) >= minDistanceY))
                return Vector2.Zero;

            // Calculate and return intersection depths.
            var depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            var depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }
    }
}