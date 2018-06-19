using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions.Tests
{
    public class BasicWall : ICollisionActor
    {
        public Vector2 Position { get; set; }
        public IShapeF Bounds { get; set; }
        public Vector2 Velocity { get; set; }

        public BasicWall()
        {
            Bounds = new RectangleF(0f, 0f, 1f, 1f);
        }
        public void OnCollision(CollisionEventArgs collisionInfo)
        {
        }
    }
}