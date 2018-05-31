using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Platformer.Collisions;
using Platformer.Components;

namespace Platformer
{
    public class EntityFactory
    {
        private readonly EntityManager _entityManager;
        private readonly ContentManager _contentManager;

        public EntityFactory(EntityManager entityManager, ContentManager contentManager)
        {
            _entityManager = entityManager;
            _contentManager = contentManager;
        }

        public Entity CreatePlayer(Vector2 position)
        {
            var dudeTexture = _contentManager.Load<Texture2D>("hero");
            var dudeAtlas = TextureAtlas.Create("dudeAtlas", dudeTexture, 16, 16);

            var entity = _entityManager.CreateEntity();
            entity.Attach(new Sprite(dudeAtlas[0]));
            entity.Attach(new Transform2(position, 0, Vector2.One * 4));
            entity.Attach(new Body { Position = new Vector2(400, 240), Size = new Vector2(32, 64), BodyType = BodyType.Dynamic });
            entity.Attach<PlayerState>();
            return entity;
        }

        public void CreateTile(int x, int y, int width, int height)
        {
            var entity = _entityManager.CreateEntity();
            entity.Attach(new Body
            {
                Position = new Vector2(x * width + width * 0.5f, y * height + height * 0.5f),
                Size = new Vector2(width, height),
                BodyType = BodyType.Static
            });
        }
    }
}