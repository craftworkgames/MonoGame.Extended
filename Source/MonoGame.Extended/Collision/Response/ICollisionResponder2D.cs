﻿using Microsoft.Xna.Framework;
using MonoGame.Extended.Collision.Narrowphase;

namespace MonoGame.Extended.Collision.Response
{
    public interface ICollisionResponder2D
    {
        CollisionSimulation2D CollisionSimulation { get; }

        void RespondTo(GameTime gameTime, ref NarrowphaseCollisionPair2D collisionPair);
    }
}
