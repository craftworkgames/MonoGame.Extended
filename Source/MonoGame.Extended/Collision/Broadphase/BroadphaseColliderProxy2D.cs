using System.Runtime.InteropServices;
using MonoGame.Extended.Collision.Broadphase.BoundingVolumes;

namespace MonoGame.Extended.Collision.Broadphase
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BroadphaseColliderProxy2D
    {
        public readonly BoundingVolume2D WorldBoundingVolume;
        public readonly Collider2D Collider;

        public BroadphaseColliderProxy2D(Collider2D collider)
        {
            Collider = collider;
            WorldBoundingVolume = BoundingVolume2D.Create(collider.BoundingVolume.Type);
        }
    }
}
