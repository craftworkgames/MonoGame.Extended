using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Animations.SpriteSheets;
using Demo.Platformer.Entities.Components;

namespace Demo.Platformer.Entities.Factories
{
    public sealed class BadGuyFactory : TiledEntityFactory
    {
        private TextureAtlas _characterTextureAtlas;

        public override void BuildEntity(Entity entity, TiledMapObject mapObject)
        {
            var textureRegion = _characterTextureAtlas[0];
            var animationFactory = new SpriteSheetAnimationFactory(_characterTextureAtlas);

            animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 100 }, 1.0f));
            animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 96, 97, 98, 99 }, isPingPong: true));

            var spriteComponent = entity.GetComponent<SpriteComponent>();
            spriteComponent.AnimationFactory = animationFactory;
            spriteComponent.Play("walk");

            entity.GetComponent<CollisionBody>().OnCollision += Collision.EnemyCollisionHandler;

            base.BuildEntity(entity, mapObject);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            var texture = contentManager.Load<Texture2D>("tiny-characters");
            _characterTextureAtlas = TextureAtlas.Create(texture, 32, 32, 112);
        }
    }
}
