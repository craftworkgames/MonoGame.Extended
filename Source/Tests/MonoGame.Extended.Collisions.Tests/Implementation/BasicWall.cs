using System;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions.Tests
{
    public class BasicWall<TShape> : ICollisionActor<TShape>
        where TShape : struct, IShapeF
    {
        public Vector2 Position { get; set; }
        public TShape Bounds { get; set; }
        public Vector2 Velocity { get; set; }

        public BasicWall()
        {
            if (typeof(TShape) == typeof(RectangleF))
                Bounds = (TShape)(object)new RectangleF(0f, 0f, 1f, 1f);
            if (typeof(TShape) == typeof(CircleF))
                Bounds = (TShape)(object)new CircleF(new Point2(0f, 0f), 1f);

        }
        public void OnCollision(CollisionEventArgs collisionInfo)
        {
        }
    }
}
