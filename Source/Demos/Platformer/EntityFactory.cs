using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.TextureAtlases;
using Platformer.Collisions;
using Platformer.Components;
using World = MonoGame.Extended.Entities.World;

namespace Platformer
{
    public class EntityFactory
    {
        private readonly World _world;
        private readonly ContentManager _contentManager;

        public EntityFactory(World world, ContentManager contentManager)
        {
            _world = world;
            _contentManager = contentManager;
        }

        public Entity CreatePlayer(Vector2 position)
        {
            var dudeTexture = _contentManager.Load<Texture2D>("hero");
            var dudeAtlas = TextureAtlas.Create("dudeAtlas", dudeTexture, 16, 16);

            var entity = _world.CreateEntity();
            //var animationFactory = new SpriteSheetAnimationFactory(dudeAtlas);
            //animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0, 1, 2, 1 }));
            //animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 6, 7, 8, 9, 10, 11 }, frameDuration: 0.1f));
            //animationFactory.Add("jump", new SpriteSheetAnimationData(new[] { 10, 12 }, frameDuration: 1.0f, isLooping: false));
            //animationFactory.Add("fall", new SpriteSheetAnimationData(new[] { 13, 14 }, frameDuration: 1.0f, isLooping: false));
            //animationFactory.Add("swim", new SpriteSheetAnimationData(new[] { 18, 19, 20, 21, 22, 23 }));
            //animationFactory.Add("kick", new SpriteSheetAnimationData(new[] { 15 }, frameDuration: 0.3f, isLooping: false));
            //animationFactory.Add("punch", new SpriteSheetAnimationData(new[] { 26 }, frameDuration: 0.3f, isLooping: false));
            //animationFactory.Add("cool", new SpriteSheetAnimationData(new[] { 17 }, frameDuration: 0.3f, isLooping: false));
            //entity.Attach(new AnimatedSprite(animationFactory, "idle"));
            entity.Attach(new Transform2(position, 0, Vector2.One * 4));
            entity.Attach(new Body { Position = position, Size = new Vector2(32, 64), BodyType = BodyType.Dynamic });
            entity.Attach(new Player());
            return entity;
        }

        public Entity CreateBlue(Vector2 position)
        {
            var dudeTexture = _contentManager.Load<Texture2D>("blueguy");
            var dudeAtlas = TextureAtlas.Create("blueguyAtlas", dudeTexture, 16, 16);

            var entity = _world.CreateEntity();
            //var animationFactory = new SpriteSheetAnimationFactory(dudeAtlas);
            //animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0, 1, 2, 3, 2, 1 }));
            //animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 6, 7, 8, 9, 10, 11 }, frameDuration: 0.1f));
            //animationFactory.Add("jump", new SpriteSheetAnimationData(new[] { 10, 12 }, frameDuration: 1.0f, isLooping: false));
            //entity.Attach(new AnimatedSprite(animationFactory, "idle") { Effect = SpriteEffects.FlipHorizontally });
            entity.Attach(new Transform2(position, 0, Vector2.One * 4));
            entity.Attach(new Body { Position = position, Size = new Vector2(32, 64), BodyType = BodyType.Dynamic });
            entity.Attach(new Enemy());
            return entity;
        }

        public void CreateTile(int x, int y, int width, int height)
        {
            var entity = _world.CreateEntity();
            entity.Attach(new Body
            {
                Position = new Vector2(x * width + width * 0.5f, y * height + height * 0.5f),
                Size = new Vector2(width, height),
                BodyType = BodyType.Static
            });
        }
    }
}