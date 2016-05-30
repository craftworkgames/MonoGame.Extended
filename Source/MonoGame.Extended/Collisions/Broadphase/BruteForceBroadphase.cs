using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions.Broadphase
{
    public class BruteForceBroadphase : IBroadphaseCollisionDetector
    {
        private BroadphaseCollisionDelegate _contactCollisionDelegate;

        public BruteForceBroadphase()
        {
        }

        public void Initialize(BroadphaseCollisionDelegate broadphaseCollisionDelegate)
        {
            _contactCollisionDelegate = broadphaseCollisionDelegate;
        }

        public void Update(GameTime gameTime, IReadOnlyList<CollisionFixtureProxy> fixtureProxies)
        {
            for (var i = 0; i < fixtureProxies.Count - 1; ++i)
            {
                var firstProxy = fixtureProxies[i];
                for (var j = 1; j < fixtureProxies.Count && i != j; ++j)
                {
                    var secondProxy = fixtureProxies[j];

                    if (firstProxy.BodyHashCode == secondProxy.BodyHashCode)
                    {
                        continue;
                    }

                    if (!firstProxy.BoundingVolume.Intersects(ref secondProxy.BoundingVolume))
                    {
                        continue;
                    }

                    var broadphaseCollisionPair = new BroadphaseCollisionPair(firstProxy.Fixture, secondProxy.Fixture);
                    _contactCollisionDelegate(ref broadphaseCollisionPair);
                }
            }
        }
    }
}
