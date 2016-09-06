using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Systems;

namespace Demo.Platformer.Entities.Systems
{
    public class CharacterStateSystem : ComponentSystem
    {
        private readonly EntityFactory _entityFactory;
        private readonly Vector2 _spawnPoint;

        public CharacterStateSystem(EntityFactory entityFactory, Vector2 spawnPoint)
        {
            _entityFactory = entityFactory;
            _spawnPoint = spawnPoint;
        }

        public override void Update(GameTime gameTime)
        {
            var playerEntity = GetEntity(Entities.Player);

            if (playerEntity != null)
            {
                var playerState = playerEntity.GetComponent<CharacterState>();

                if (!playerState.IsAlive)
                {
                    _entityFactory.CreateBloodExplosion(playerEntity.Position);
                    playerEntity.Destroy();
                    _entityFactory.CreatePlayer(_spawnPoint);
                }
            }
        }
    }
}