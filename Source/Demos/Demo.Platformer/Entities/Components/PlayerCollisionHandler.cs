using System;
using Microsoft.Xna.Framework;

namespace Demo.Platformer.Entities.Components
{
    public class PlayerCollisionHandler : BasicCollisionHandler
    {
        public override void OnCollision(BasicCollisionBody bodyA, BasicCollisionBody bodyB, Vector2 depth)
        {
            var state = bodyA.Entity.GetComponent<PlayerState>();
            var absDepthX = Math.Abs(depth.X);
            var absDepthY = Math.Abs(depth.Y);

            if (absDepthY < absDepthX)
            {
                bodyA.Position += new Vector2(0, depth.Y); // move the player out of the ground or roof
                state.IsOnGround = bodyA.Velocity.Y > 0;

                if (state.IsOnGround)
                    bodyA.Velocity = new Vector2(bodyA.Velocity.X, 0); // set y velocity to zero only if this is a ground collision
            }
            else
            {
                bodyA.Position += new Vector2(depth.X, 0);  // move the player out of the wall
                bodyA.Velocity = new Vector2(bodyA.Velocity.X, bodyA.Velocity.Y < 0 ? 0 : bodyA.Velocity.Y); // drop the player down if they hit a wall
            }
        }
    }
}