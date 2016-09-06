using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collision.Broadphase
{
    public class BruteForceBroadphase2D : ICollisionBroadphase2D
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
            foreach (var secondColliderProxy in CollisionSimulation.ColliderProxies)
            {
                if (colliderProxy.Collider == secondColliderProxy.Collider)
                {
                    continue;
                }

                if (!colliderProxy.WorldBoundingVolume.Intersects(secondColliderProxy.WorldBoundingVolume))
                {
                    continue;
                }

                var broadphaseCollisionPair = new BroadphaseCollisionPair2D(colliderProxy.Collider, secondColliderProxy.Collider);
                broadphaseCollisionDelegate(ref broadphaseCollisionPair);
            }
        }
    }
}
