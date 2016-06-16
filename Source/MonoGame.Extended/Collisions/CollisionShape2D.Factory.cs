using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions
{
    public partial class CollisionShape2D
    {
        public static CollisionShape2D Create(Vector2[] vertices)
        {
            //TODO: Request a collision shape from a pool objects.
            return new CollisionShape2D(null, vertices);
        }
    }
}
