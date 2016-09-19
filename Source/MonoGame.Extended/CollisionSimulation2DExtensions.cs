using MonoGame.Extended.Collision;
using MonoGame.Extended.Collision.BoundingVolumes;
using MonoGame.Extended.Collision.Shapes;

namespace MonoGame.Extended
{
    public static class CollisionSimulation2DExtensions
    {
        public static Collider2D CreateBoxCollider(this CollisionSimulation2D collisionSimulation,
            object owner, SizeF size,
            BoundingVolumeType2D boundingVolumeType = BoundingVolumeType2D.BoundingBox)
        {
            var transform = owner as Transform2D ?? new Transform2D();

            var box = new Box2D(transform)
            {
                Size = size
            };
            return collisionSimulation.CreateCollider(owner, transform, box, boundingVolumeType);
        }
    }
}
