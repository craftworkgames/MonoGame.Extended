using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Components;

namespace Demo.Platformer.Entities.Components
{
    public class BasicCollisionHandler : EntityComponent
    {
        public virtual void OnCollision(BasicCollisionBody bodyA, BasicCollisionBody bodyB, Vector2 depth)
        {
            var characterState = bodyA.Entity.GetComponent<CharacterState>();
            var absDepthX = Math.Abs(depth.X);
            var absDepthY = Math.Abs(depth.Y);

            if (absDepthY < absDepthX)
            {
                bodyA.Position += new Vector2(0, depth.Y); // move the player out of the ground or roof
                var isOnGround = bodyA.Velocity.Y > 0;

                if (isOnGround)
                {
                    bodyA.Velocity = new Vector2(bodyA.Velocity.X, 0); // set y velocity to zero only if this is a ground collision
                    characterState.IsJumping = false;
                }
            }
            else
            {
                bodyA.Position += new Vector2(depth.X, 0);  // move the player out of the wall
                bodyA.Velocity = new Vector2(bodyA.Velocity.X, bodyA.Velocity.Y < 0 ? 0 : bodyA.Velocity.Y); // drop the player down if they hit a wall
            }
        }
    }
}