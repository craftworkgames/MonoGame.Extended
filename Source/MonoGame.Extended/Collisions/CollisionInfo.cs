using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MonoGame.Extended.Collisions
{
    public class CollisionInfo
    {
        public ICollidable Other { get; internal set; }
        public Vector2 PenetrationVector { get; internal set; }
    }
}