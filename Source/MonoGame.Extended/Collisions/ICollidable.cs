using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Collisions
{
    public interface ICollidable
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        RectangleF GetAxisAlignedBoundingBox();
        void OnCollision(CollisionInfo collisionInfo);
    }
}