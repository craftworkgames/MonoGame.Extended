using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Collisions
{
    public interface IActorTarget : IMovable
    {
        Vector2 Velocity { get; set; }
        RectangleF BoundingBox { get; }
        void OnCollision(CollisionInfo collisionInfo);
    }
}