using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collision.Detection.Broadphase;

namespace MonoGame.Extended.Collision.Detection.Narrowphase
{
    public class PassThroughNarrowphase2D : ICollisionNarrowphase2D
    {
        public CollisionSimulation2D CollisionSimulation { get; }

        public PassThroughNarrowphase2D(CollisionSimulation2D collisionSimulation)
        {
            if (collisionSimulation == null)
            {
                throw new ArgumentNullException(nameof(collisionSimulation));
            }

            CollisionSimulation = collisionSimulation;
        }

        public void Resolve(GameTime gameTime, ref BroadphaseCollisionPair2D collisionPair, NarrowphaseCollisionDelegate2D narrowphaseCollisionDelegate)
        {
            var narrowphaseCollisionPair = new NarrowphaseCollisionPair2D(collisionPair.FirstCollider, collisionPair.SecondCollider);
            narrowphaseCollisionDelegate(ref narrowphaseCollisionPair);
        }
    }
}
