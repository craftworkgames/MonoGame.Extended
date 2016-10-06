using MonoGame.Extended.Collision.Detection.Broadphase;

namespace MonoGame.Extended.Collision
{
    public delegate void BroadphaseCollisionDelegate(ref BroadphaseCollisionResult2D result, out bool cancel);
}
