using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions.Tests
{
    public class BasicActor : ICollisionActor
    {
        public Vector2 Position { get; set; }
        public IShapeF Bounds { get; set; }
        public Vector2 Velocity { get; set; }

        public BasicActor()
        {
            Bounds = new RectangleF(0f, 0f, 1f, 1f);
        }
        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            Bounds.Position -= collisionInfo.PenetrationVector;
            Position -= collisionInfo.PenetrationVector;

            if (collisionInfo.Other is BasicActor)
            {
                CollisionCount++;
            }
            else
            {
                Console.WriteLine(collisionInfo.Other.GetType().Name);
            }
        }

        public int CollisionCount { get; set; }
    }
}
