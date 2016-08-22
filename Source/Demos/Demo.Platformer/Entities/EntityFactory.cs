using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace Demo.Platformer.Entities
{
    public class EntityFactory
    {
        private readonly ContentManager _contentManager;
        private readonly EntityComponentSystem _entityComponentSystem;
        private TextureAtlas _characterTextureAtlas;

        public EntityFactory(EntityComponentSystem entityComponentSystem, ContentManager contentManager)
        {
            _entityComponentSystem = entityComponentSystem;
            _contentManager = contentManager;

            LoadContent(contentManager);
        }

        private void LoadContent(ContentManager content)
        {
            var texture = content.Load<Texture2D>("tiny-characters");
            _characterTextureAtlas = TextureAtlas.Create(texture, 32, 32, 15);

            var animationFactory = new SpriteSheetAnimationFactory(_characterTextureAtlas);
            animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0, 1, 2, 3 }, isReversed: true));
        }

        public Entity CreatePlayer(Vector2 position)
        {
            var entity = _entityComponentSystem.CreateEntity("Player", position);
            var textureRegion = _characterTextureAtlas[0];

            entity.AttachComponent(new Sprite(textureRegion));
            entity.AttachComponent(new BasicCollisionComponent(textureRegion.Size));
            entity.AttachComponent(new PlayerMovementComponent());

            return entity;
        }
    }
}