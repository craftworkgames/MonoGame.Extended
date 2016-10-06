using Microsoft.Xna.Framework;
using MonoGame.Extended.Collision.Detection.Narrowphase;

namespace MonoGame.Extended.Collision.Response
{
    public interface ICollisionResponder2D
    {
        CollisionSimulation2D CollisionSimulation { get; }

        void RespondTo(ref NarrowphaseCollisionResult2D narrowphaseResult, GameTime gameTime);
    }
}
