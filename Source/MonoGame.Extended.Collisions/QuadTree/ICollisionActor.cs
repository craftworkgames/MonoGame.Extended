using System;

namespace MonoGame.Extended.Collisions
{
    /// <summary>
    /// An actor that can be collided with.
    /// </summary>
    public interface ICollisionActor<TShape> : ICollisionActor
        where TShape : struct, IShapeF
    {
        TShape Bounds { get; }


    }
    
    public interface ICollisionActor
    {
        void OnCollision(CollisionEventArgs collisionInfo);
    }

    public static class CollisionActorExtensions
    {
        public static RectangleF GetEnclosingRectangle(this ICollisionActor actor)
        {
            if (actor is ICollisionActor<RectangleF> rectActor)
            {
                return rectActor.Bounds;
            }
            else if (actor is ICollisionActor<CircleF> circleActor)
            {
                return circleActor.Bounds.ToRectangleF();
            }
            else
                throw new NotImplementedException();
        }

        public static bool Intersects<TShape>(this ICollisionActor actor, TShape shape)
            where TShape : struct, IShapeF
        {
            if (actor is ICollisionActor<RectangleF> rectActor)
            {
                return rectActor.Bounds.Intersects(shape);
            }
            else if (actor is ICollisionActor<CircleF> circleActor)
            {
                return circleActor.Bounds.Intersects(shape);
            }
            else
                throw new NotImplementedException();
        }




    }
}
