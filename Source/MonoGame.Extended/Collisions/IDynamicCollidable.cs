using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Collisions
{
    public interface IActor
    {
        void OnCollision(CollisionInfo collisionInfo);
    }

    public interface IActorTarget : IMovable
    {
        void OnCollision(CollisionInfo collisionInfo);
    }
}