using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions.Tests
{
    public class BasicActor<TShape> : ICollisionActor<TShape>
        where TShape : struct, IShapeF
    {
        public Vector2 Position { get; set; }
        public TShape Bounds { get; set; }
        public Vector2 Velocity { get; set; }

        public BasicActor()
        {
            if (typeof(TShape) == typeof(RectangleF))
                Bounds = (TShape)(object)new RectangleF(0f, 0f, 1f, 1f);
            if (typeof(TShape) == typeof(CircleF))
                Bounds = (TShape)(object)new CircleF(new Point2(0f, 0f), 1f);
        }
        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            var b = Bounds;
            b.Position -= collisionInfo.PenetrationVector;
            Bounds = b;
            Position = Bounds.Position;
            if (collisionInfo.Other is BasicActor<RectangleF>
                || collisionInfo.Other is BasicActor<CircleF>)
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
