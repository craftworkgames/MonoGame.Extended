using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Shapes;

namespace Demo.Platformer.Entities.Systems
{
    public class BasicCollisionSystem : ComponentSystem
    {
        private readonly Vector2 _gravity;
        private readonly RectangleF[] _collisionRectangles;

        public BasicCollisionSystem(Vector2 gravity, RectangleF[] collisionRectangles)
        {
            _gravity = gravity;
            _collisionRectangles = collisionRectangles;
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var components = GetComponents<BasicCollisionBody>();

            foreach (var c in components)
            {
                c.Velocity += _gravity * deltaTime;
                c.Position += c.Velocity * deltaTime;

                c.IsOnGround = false; // TODO: Not sure what to do about this yet

                foreach (var collisionRectangle in _collisionRectangles)
                {
                    var depth = c.BoundingRectangle.IntersectionDepth(collisionRectangle);

                    if (depth != Vector2.Zero)
                    {
                        var collisionHandlers = c.Entity.GetComponents<BasicCollisionHandler>();

                        foreach (var collisionHandler in collisionHandlers)
                            collisionHandler.OnCollision(c, null, depth);
                    }
                }
            }
        }
    }
}