using System.Linq;
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
            foreach (var component in GetComponents<CharacterState>().ToArray())
            {
                if (!component.IsAlive)
                {
                    _entityFactory.CreateBloodExplosion(component.Entity.Position);
                    component.Entity.Destroy();

                    if(component.Entity.Name == Entities.Player)
                        _entityFactory.CreatePlayer(_spawnPoint);
                }

            }
        }
    }
}