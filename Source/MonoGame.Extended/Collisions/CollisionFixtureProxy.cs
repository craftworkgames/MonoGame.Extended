using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoGame.Extended.Shapes.BoundingVolumes;

namespace MonoGame.Extended.Collisions
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CollisionFixtureProxy
    {
        public AxisAlignedBoundingBox2D BoundingVolume;
        public readonly CollisionFixture Fixture;
        public readonly int BodyHashCode;

        internal CollisionFixtureProxy(CollisionFixture fixture)
        { 
            Fixture = fixture;
            BoundingVolume = default(AxisAlignedBoundingBox2D);
            BodyHashCode = RuntimeHelpers.GetHashCode(fixture.Body);
        }
    }
}
