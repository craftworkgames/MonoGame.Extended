using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using Demo.Platformer.Entities.Components;

namespace Demo.Platformer.Entities
{
    public delegate void CollisionHandler(Entity entity, Entity otherEntity, Vector2 depth);

    public static class Collision
    {
        public static void BasicCollisionHandler(Entity entity, Entity otherEntity, Vector2 depth)
        {
            var body = entity.GetComponent<CollisionBody>();
            var otherBody = otherEntity.GetComponent<CollisionBody>();

            var characterState = entity.GetComponent<CharacterState>();
            var absDepthX = Math.Abs(depth.X);
            var absDepthY = Math.Abs(depth.Y);

            if (absDepthY < absDepthX)
            {
                body.Position += new Vector2(0, depth.Y); // move the player out of the ground or roof
                var isOnGround = body.Velocity.Y > 0;

                if (isOnGround)
                {
                    body.Velocity = new Vector2(body.Velocity.X, 0); // set y velocity to zero only if this is a ground collision
                    characterState.IsJumping = false;
                }
            }
            else
            {
                body.Position += new Vector2(depth.X, 0);  // move the player out of the wall
                body.Velocity = new Vector2(body.Velocity.X, body.Velocity.Y < 0 ? 0 : body.Velocity.Y); // drop the player down if they hit a wall
            }
        }

        public static void PlayerCollisionHandler(Entity entity, Entity otherEntity, Vector2 depth)
        {
            if (otherEntity.HasComponent<Deadly>())
                entity.GetComponent<CharacterState>().HealthPoints = 0;
        }

        public static void EnemyCollisionHandler(Entity entity, Entity otherEntity, Vector2 depth)
        {
            if (otherEntity.HasComponent<Player>())
            {
                entity.GetComponent<CharacterState>().HealthPoints = 0;
                otherEntity.GetComponent<CharacterState>().HealthPoints = 0;
            }
        }
    }
}