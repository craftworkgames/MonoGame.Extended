using System;
using System.Collections.Generic;
using System.Diagnostics;
using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;

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
    public class CollisionSystem : EntityProcessingSystem<TransformComponent, BasicCollisionBodyComponent>
    {
        internal HashSet<CollisionPair> CollisionPairs = new HashSet<CollisionPair>();

        protected override void Begin(GameTime gameTime)
        {
            CollisionPairs.Clear();
        }

        protected override void Process(GameTime gameTime, Entity entity, TransformComponent transform, BasicCollisionBodyComponent body)
        {
            //foreach (var entityB in EntityManager.Entities)
            //{
            //    if (entity == entityB)
            //        return;
            //    var collisionComponentB = entityB.Get<BasicCollisionBodyComponent>();
            //    if (collisionComponentB == null)
            //        return;

            //    var rectangleA = body.BoundingRectangle;
            //    var rectangleB = collisionComponentB.BoundingRectangle;
            //    var depth = IntersectionDepth(rectangleA, rectangleB);
            //    if (depth == Vector2.Zero)
            //        return;

            //   var collisionPair = new CollisionPair(body, collisionComponentB, depth);

            //    if (CollisionPairs.Contains(collisionPair))
            //        continue;

            //    CollisionPairs.Add(collisionPair);
            //}
        }

        protected override void End(GameTime gameTime)
        {
            foreach (var collisionPair in CollisionPairs)
                ResolveCollision(collisionPair.FirstBody, collisionPair.SecondBody, collisionPair.Depth);
        }

        private void ResolveCollision(BasicCollisionBodyComponent bodyA, BasicCollisionBodyComponent bodyB, Vector2 depth)
        {
            var player = bodyA.Entity.Get<PlayerComponent>();

            var transform = bodyA.Entity.Get<TransformComponent>();
            var absDepthX = Math.Abs(depth.X);
            var absDepthY = Math.Abs(depth.Y);

            if (absDepthY < absDepthX)
            {
                transform.Position += new Vector2(0, depth.Y); // move the player out of the ground or roof
                var isOnGround = bodyA.Velocity.Y > 0;

                if (isOnGround)
                {
                    bodyA.Velocity = new Vector2(bodyA.Velocity.X, 0); // set y velocity to zero only if this is a ground collision
                    if (player != null)
                        player.IsJumping = false;
                }
            }
            else
            {
                transform.Position += new Vector2(depth.X, 0);  // move the player out of the wall
                bodyA.Velocity = new Vector2(bodyA.Velocity.X, bodyA.Velocity.Y < 0 ? 0 : bodyA.Velocity.Y); // drop the player down if they hit a wall
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

        // ReSharper disable once UseNameofExpression
        [DebuggerDisplay("{DebugDisplayString,nq}")]
        public struct CollisionPair : IEquatable<CollisionPair>
        {
            public readonly BasicCollisionBodyComponent FirstBody;
            public readonly BasicCollisionBodyComponent SecondBody;
            public readonly Vector2 Depth;

            public CollisionPair(BasicCollisionBodyComponent firstBody, BasicCollisionBodyComponent secondBody, Vector2 depth)
            {
                FirstBody = firstBody;
                SecondBody = secondBody;
                Depth = depth;
            }

            public static bool operator ==(CollisionPair first, CollisionPair second)
            {
                return first.FirstBody == second.FirstBody && first.SecondBody == second.SecondBody;
            }

            public static bool operator !=(CollisionPair first, CollisionPair second)
            {
                return !(first == second);
            }

            public bool Equals(CollisionPair other)
            {
                return FirstBody == other.FirstBody && SecondBody == other.SecondBody;
            }

            public override bool Equals(object other)
            {
                throw new NotSupportedException();
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (FirstBody.GetHashCode() * 397) ^ SecondBody.GetHashCode();
                }
            }

            internal string DebugDisplayString => $"FirstBody = {FirstBody}, SecondBody = {SecondBody}";

            public override string ToString()
            {
                return $"{{FirstBody = {FirstBody}, SecondBody = {SecondBody}}}";
            }
        }
    }
}