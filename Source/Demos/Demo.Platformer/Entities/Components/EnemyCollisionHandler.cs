using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;

namespace Demo.Platformer.Entities.Components
{
    public class EnemyCollisionHandler : BasicCollisionHandler
    {
        public override void OnCollision(BasicCollisionBody bodyA, BasicCollisionBody bodyB, Vector2 depth)
        {
            var player = GetEntityByTag(bodyA.Entity, bodyB.Entity, Entities.Player);
            var enemy = GetEntityByTag(bodyA.Entity, bodyB.Entity, "BadGuy");

            if (player != null && enemy != null)
            {
                enemy.GetComponent<CharacterState>().HealthPoints = 0;
                player.GetComponent<CharacterState>().HealthPoints = 0;
            }

            base.OnCollision(bodyA, bodyB, depth);
        }

        private static Entity GetEntityByTag(Entity entityA, Entity entityB, string tagName)
        {
            if ((string) entityA.Tag == tagName)
                return entityA;

            return (string) entityB.Tag == tagName ? entityB : null;
        }
    }
}