using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collision.Detection.Broadphase
{
    public interface IBroadphase2D
    {
        CollisionSimulation2D CollisionSimulation { get; }

        void Add(BroadphaseColliderProxy2D colliderProxy);
        void Remove(BroadphaseColliderProxy2D colliderProxy);
        void Update(BroadphaseColliderProxy2D colliderProxy);
        void Query(BroadphaseColliderProxy2D colliderProxy, GameTime gameTime, BroadphaseCollisionDelegate2D broadphaseCollisionDelegate);
    }
}
