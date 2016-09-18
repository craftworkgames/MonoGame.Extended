using Microsoft.Xna.Framework;
using MonoGame.Extended.Collision.Detection.Broadphase;

namespace MonoGame.Extended.Collision.Detection.Narrowphase
{
    public interface INarrowphase2D
    {
        CollisionSimulation2D CollisionSimulation { get; }

        void Resolve(ref BroadphaseCollisionPair2D collisionPair, GameTime gameTime, NarrowphaseCollisionDelegate2D narrowphaseCollisionDelegate);
    }
}
