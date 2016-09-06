using Microsoft.Xna.Framework;
using MonoGame.Extended.Collision.Broadphase;

namespace MonoGame.Extended.Collision.Narrowphase
{
    public interface ICollisionNarrowphase2D
    {
        CollisionSimulation2D CollisionSimulation { get; }

        void Resolve(GameTime gameTime, ref BroadphaseCollisionPair2D collisionPair, NarrowphaseCollisionDelegate2D narrowphaseCollisionDelegate);
    }
}
