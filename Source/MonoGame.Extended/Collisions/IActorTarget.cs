using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

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