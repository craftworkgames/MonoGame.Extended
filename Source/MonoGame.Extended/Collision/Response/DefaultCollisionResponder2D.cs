using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collision.Narrowphase;

namespace MonoGame.Extended.Collision.Response
{
    public class DefaultCollisionResponder2D : ICollisionResponder2D
    {
        public CollisionSimulation2D CollisionSimulation { get; }

        public DefaultCollisionResponder2D(CollisionSimulation2D collisionSimulation)
        {
            if (collisionSimulation == null)
            {
                throw new ArgumentNullException(nameof(collisionSimulation));
            }

            CollisionSimulation = collisionSimulation;
        }

        public void RespondTo(GameTime gameTime, ref NarrowphaseCollisionPair2D collisionPair)
        {
        }

    }
}
