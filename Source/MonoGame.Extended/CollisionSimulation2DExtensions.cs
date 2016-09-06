using MonoGame.Extended.Collision;
using MonoGame.Extended.Collision.Broadphase.BoundingVolumes;
using MonoGame.Extended.Collision.Narrowphase.Shapes;

namespace MonoGame.Extended
{
    public static class CollisionSimulation2DExtensions
    {
        public static Collider2D CreateBoxCollider(this CollisionSimulation2D collisionSimulation,
            ITransform2D transform, SizeF size,
            BoundingVolumeType2D boundingVolumeType = BoundingVolumeType2D.BoundingBox)
        {
            var box = new Box2D
            {
                Size = size
            };
            return collisionSimulation.CreateCollider(transform, box, boundingVolumeType);
        }
    }
}
