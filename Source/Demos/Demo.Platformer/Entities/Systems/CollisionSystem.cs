using System;
using System.Collections.Generic;
using System.Linq;
using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;

namespace Demo.Platformer.Entities.Systems
{
    [Aspect(AspectType.All, typeof(CollisionBodyComponent), typeof(TransformComponent))]
    [EntitySystem(GameLoopType.Update, Layer = 0)]
    public class CollisionSystem : EntitySystem
    {
        private readonly List<CollisionBodyComponent> _staticBodies = new List<CollisionBodyComponent>();
        private readonly List<CollisionBodyComponent> _movingBodies = new List<CollisionBodyComponent>();

        public CollisionSystem()
        {
        }

        public override void Initialize()
        {
            EntityManager.EntityAdded += OnEntityAdded;
            EntityManager.EntityRemoved += OnEntityRemoved;
        }

        private void OnEntityAdded(Entity entity)
        {
            var collision = entity.Get<CollisionBodyComponent>();
            if (collision == null)
                return;

            if (collision.IsStatic)
                _staticBodies.Add(collision);
            else
                _movingBodies.Add(collision);
        }

        private void OnEntityRemoved(Entity entity)
        {
            var collision = entity.Get<CollisionBodyComponent>();
            if (collision == null)
                return;

            if (collision.IsStatic)
                _staticBodies.Remove(collision);
            else
                _movingBodies.Remove(collision);
        }

        protected override void Process(GameTime gameTime)
        {
            foreach (var bodyA in _movingBodies)
            {
                foreach (var bodyB in _staticBodies.Concat(_movingBodies))
                {
                    if (bodyA == bodyB)
                        continue;

                    var depth = IntersectionDepth(bodyA.BoundingRectangle, bodyB.BoundingRectangle);

                    //if (depth != Vector2.Zero)
                    //{
                    //    bodyA.OnCollision(bodyA, bodyB, depth);
                    //}
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