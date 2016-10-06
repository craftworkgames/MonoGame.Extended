using Microsoft.Xna.Framework;
using MonoGame.Extended.Collision.Detection.Broadphase;

namespace MonoGame.Extended.Collision.Detection.Narrowphase
{
    public interface INarrowphase2D
    {
        CollisionSimulation2D CollisionSimulation { get; }

        void Query(ref BroadphaseCollisionResult2D broadphaseResult, GameTime gameTime, out NarrowphaseCollisionResult2D? narrowphaseResult);
    }
}
