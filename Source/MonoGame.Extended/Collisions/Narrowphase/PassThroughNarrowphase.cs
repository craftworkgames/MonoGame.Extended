using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collisions.Broadphase;

namespace MonoGame.Extended.Collisions.Narrowphase
{
    public class PassThroughNarrowphase : INarrowphaseCollisionDetector
    {
        private NarrowphaseCollisionDelegate _narrowphaseCollisionDelegate;

        public PassThroughNarrowphase()
        {
        }

        public void Initialize(NarrowphaseCollisionDelegate narrowphaseCollisionDelegate)
        {
            _narrowphaseCollisionDelegate = narrowphaseCollisionDelegate;
        }

        public void Update(GameTime gameTime, IReadOnlyList<BroadphaseCollisionPair> collisionPairs)
        {
            foreach (var contact in collisionPairs)
            {
                var narrowphaseCollisionPair = new NarrowphaseCollisionPair(contact.FirstFixture, contact.SecondFixture);
                _narrowphaseCollisionDelegate(ref narrowphaseCollisionPair);
            }
        }
    }
}
