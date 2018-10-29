using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    public interface ICollidable
    {
        RectangleF BoundingBox { get; }
    }

    public interface IActorTarget : IMovable, ICollidable
    {
        string Handle { get; }
        Vector2 Velocity { get; }
        int Tile { get; set; }
        void OnCollision(CollisionInfo collisionInfo);
    }
}