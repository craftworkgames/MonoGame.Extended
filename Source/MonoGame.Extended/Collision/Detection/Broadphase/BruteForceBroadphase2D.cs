using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collision.Detection.Broadphase
{
    public class BruteForceBroadphase2D : IBroadphase2D
    {
        public CollisionSimulation2D CollisionSimulation { get; }

        public BruteForceBroadphase2D()
        {
        }

        public void Add(BroadphaseColliderProxy2D colliderProxy)
        {
        }

        public void Remove(BroadphaseColliderProxy2D colliderProxy)
        {
        }

        public void Update(BroadphaseColliderProxy2D colliderProxy)
        {
        }

        public BruteForceBroadphase2D(CollisionSimulation2D collisionSimulation)
        {
            CollisionSimulation = collisionSimulation;
        }

        public void Query(BroadphaseColliderProxy2D colliderProxy, GameTime gameTime, BroadphaseCollisionDelegate2D broadphaseCollisionDelegate)
        {   
            foreach (var otherColliderProxy in CollisionSimulation.ColliderProxies)
            {
                if (colliderProxy.Collider == otherColliderProxy.Collider)
                {
                    continue;
                }

                if (!colliderProxy.WorldBoundingVolume.Intersects(otherColliderProxy.WorldBoundingVolume))
                {
                    continue;
                }

                var broadphaseCollisionPair = new BroadphaseCollisionResult2D(colliderProxy.Collider, otherColliderProxy.Collider);
                broadphaseCollisionDelegate(ref broadphaseCollisionPair);
            }
        }
    }
}
