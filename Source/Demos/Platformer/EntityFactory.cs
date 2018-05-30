using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;
using Platformer.Components;

namespace Platformer
{
    public class EntityFactory
    {
        private readonly EntityManager _entityManager;

        public EntityFactory(EntityManager entityManager)
        {
            _entityManager = entityManager;
        }

        public Entity CreatePlayer(Vector2 position)
        {
            var entity = _entityManager.CreateEntity();
            entity.Attach<Sprite>(s => s.Color = Color.WhiteSmoke);
            entity.Attach<Transform2>(t => t.Position = position);
            entity.Attach<VelocityComponent>();
            entity.Attach<PlayerComponent>();
            return entity;
        }

        public Entity CreateTile(int x, int y)
        {
            var entity = _entityManager.CreateEntity();
            entity.Attach<Sprite>(s => s.Color = Color.Blue);
            entity.Attach<Transform2>(t => t.Position = new Vector2(32 * x, 32 * y));
            return entity;
        }
    }
}