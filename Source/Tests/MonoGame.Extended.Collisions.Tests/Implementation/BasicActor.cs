using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions;

namespace MonoGame.Extended.Gui.Tests
{
    public class BasicActor : IActorTarget
    {
        public Vector2 Position { get; set; }
        public RectangleF BoundingBox { get; set; }
        public Vector2 Velocity { get; set; }
        public void OnCollision(CollisionInfo collisionInfo)
        {
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
