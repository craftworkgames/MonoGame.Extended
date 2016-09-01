using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Components;

namespace Demo.Platformer.Entities.Components
{
    public abstract class BasicCollisionHandler : EntityComponent
    {
        public abstract void OnCollision(BasicCollisionBody bodyA, BasicCollisionBody bodyB, Vector2 depth);
    }
}