using MonoGame.Extended.Collision.Detection;
using MonoGame.Extended.Collision.Detection.Narrowphase;

namespace MonoGame.Extended.Collision
{
    public delegate void CollisionDelegate(ref NarrowphaseCollisionResult2D result, out bool cancel);
}
