using System;
using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Shapes;

namespace Demo.Platformer.Entities.Systems
{
    public class BasicCollisionSystem : UpdatableComponentSystem
    {
        private readonly RectangleF[] _collisionRectangles;

        public BasicCollisionSystem(RectangleF[] collisionRectangles)
        {
            _collisionRectangles = collisionRectangles;
        }

        public override void Update(GameTime gameTime)
        {
            var components = GetComponents<BasicCollisionComponent>();

            foreach (var c in components)
            {
                c.IsOnGround = false;

                foreach (var collisionRectangle in _collisionRectangles)
                {
                    var depth = c.BoundingRectangle.IntersectionDepth(collisionRectangle);

                    if (depth != Vector2.Zero)
                    {
                        var absDepthX = Math.Abs(depth.X);
                        var absDepthY = Math.Abs(depth.Y);

                        if (absDepthY < absDepthX)
                        {
                            c.Position += new Vector2(0, depth.Y);
                            c.Velocity = new Vector2(c.Velocity.X, c.Velocity.Y > 0 ? 0 : c.Velocity.Y);
                            c.IsOnGround = true;
                        }
                        else
                        {
                            c.Position += new Vector2(depth.X, 0);
                        }
                    }
                }
            }
        }
    }
}