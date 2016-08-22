using System;
using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Shapes;

namespace Demo.Platformer.Entities.Systems
{
    public class BasicCollisionSystem : UpdatableComponentSystem
    {
        public BasicCollisionSystem()
        {
            _groundRectangle = new RectangleF(0, 305, 800, 200);
        }

        private readonly RectangleF _groundRectangle;

        public override void Update(GameTime gameTime)
        {
            var components = GetComponents<BasicCollisionComponent>();

            foreach (var component in components)
            {
                var depth = component.BoundingRectangle.IntersectionDepth(_groundRectangle);

                component.IsOnGround = false;

                if (depth != Vector2.Zero)
                {
                    var absDepthX = Math.Abs(depth.X);
                    var absDepthY = Math.Abs(depth.Y);

                    if (absDepthY < absDepthX)
                    {
                        component.Position += new Vector2(0, depth.Y);
                        component.Velocity = new Vector2(component.Velocity.X, 0);
                        component.IsOnGround = true;
                    }
                    else
                    {
                        component.Position += new Vector2(depth.X, 0);
                    }
                }
            }
        }
    }
}