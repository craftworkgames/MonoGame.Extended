using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions.Broadphase;

namespace MonoGame.Extended.Collisions.Narrowphase
{
    public interface ICollisionNarrowphase
    {
        void Initialize(NarrowphaseCollisionDelegate narrowphaseCollisionDelegate);
        void Update(GameTime gameTime, IReadOnlyList<BroadphaseCollisionPair> collisionPairs);
    }
}
