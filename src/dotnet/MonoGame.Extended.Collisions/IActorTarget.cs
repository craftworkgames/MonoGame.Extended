using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    public interface ICollidable
    {
        RectangleF BoundingBox { get; }
    }

    public interface IActorTarget : IMovable, ICollidable
    {
        Vector2 Velocity { get; set; }
        void OnCollision(CollisionInfo collisionInfo);
    }
}