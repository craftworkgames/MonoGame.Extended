using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes.BoundingVolumes;
using MonoGame.Extended.Shapes.Explicit;

namespace MonoGame.Extended.Collisions
{
    public class CollisionShape2D : ExplicitShape2D
    {
        private AxisAlignedBoundingBox2D _boundingVolume;

        public CollisionShape2D(Vector2[] vertices)
            : base(vertices)
        {
        }
    }
}
