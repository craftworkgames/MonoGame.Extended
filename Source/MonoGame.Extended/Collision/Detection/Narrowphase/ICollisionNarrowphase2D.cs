using Microsoft.Xna.Framework;
using MonoGame.Extended.Collision.Detection.Broadphase;

namespace MonoGame.Extended.Collision.Detection.Narrowphase
{
    public interface ICollisionNarrowphase2D
    {
        CollisionSimulation2D CollisionSimulation { get; }

        void Resolve(GameTime gameTime, ref BroadphaseCollisionPair2D collisionPair, NarrowphaseCollisionDelegate2D narrowphaseCollisionDelegate);
    }
}
