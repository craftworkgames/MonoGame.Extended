using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Systems;

namespace Demo.Platformer.Entities.Systems
{
    public class PlayerStateSystem : ComponentSystem
    {
        private readonly EntityFactory _entityFactory;

        public PlayerStateSystem(EntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public override void Update(GameTime gameTime)
        {
            var playerEntity = GetEntity(Entities.Player);

            if (playerEntity != null)
            {
                var playerState = playerEntity.GetComponent<PlayerState>();

                if (!playerState.IsAlive)
                {
                    _entityFactory.CreateBloodExplosion(playerEntity.Position);
                    playerEntity.Destroy();
                }
            }
        }
    }
}