using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Tiled;

namespace Demo.Platformer.Entities.Factories
{
    public sealed class PlayerEntityFactory : TiledEntityFactory
    {
        private TextureAtlas _characterTextureAtlas;

        public override void LoadContent(ContentManager contentManager)
        {
            var texture = contentManager.Load<Texture2D>("tiny-characters");
            _characterTextureAtlas = TextureAtlas.Create(texture, 32, 32, 112);
        }

        public override void BuildEntity(Entity entity, TiledMapObject mapObject)
        {
            var textureRegion = _characterTextureAtlas[0];
            var animationFactory = new SpriteSheetAnimationFactory(_characterTextureAtlas);

            animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 12, 13 }, 1.0f));
            animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 0, 1, 2, 3 }));
            animationFactory.Add("jump", new SpriteSheetAnimationData(new[] { 8, 9 }, isLooping: false));

            entity.GetComponent<CharacterState>().OnKilled += () => entity.CreateEntity(Entities.Player, (e) => BuildEntity(e, mapObject));
            entity.GetComponent<CollisionBody>().OnCollision += Collision.PlayerCollisionHandler;
            entity.GetComponent<Player>().MapObject = mapObject;
            entity.GetComponent<SpriteComponent>().AnimationFactory = animationFactory;

            base.BuildEntity(entity, mapObject);
        }
    }
}
