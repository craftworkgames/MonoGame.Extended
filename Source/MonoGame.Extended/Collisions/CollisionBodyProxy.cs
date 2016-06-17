using System.Runtime.InteropServices;

namespace MonoGame.Extended.Collisions
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct CollisionBodyProxy
    {
        internal CollisionBody Body;
        internal CollisionBodyFlags Flags;

        internal CollisionBodyProxy(CollisionBody collisionBody)
        {
            Body = collisionBody;
            Flags = collisionBody.Flags;
        }
    }
}
