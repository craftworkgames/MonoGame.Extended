using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Collisions.Broadphase
{
    public interface IBroadphaseCollisionDetector
    {
        void Initialize(BroadphaseCollisionDelegate broadphaseCollisionDelegate);
        void Update(GameTime gameTime, IReadOnlyList<CollisionFixtureProxy> fixtureProxies);
    }
}
