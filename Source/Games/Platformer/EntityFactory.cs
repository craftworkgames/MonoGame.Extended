using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;
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
            var spriteSheet = new SpriteSheet {TextureAtlas = dudeAtlas};

            AddAnimationCycle(spriteSheet, "idle", new[] {0, 1, 2, 1});
            AddAnimationCycle(spriteSheet, "walk", new[] {6, 7, 8, 9, 10, 11});
            AddAnimationCycle(spriteSheet, "jump", new[] {10, 12}, false);
            AddAnimationCycle(spriteSheet, "fall", new[] {13, 14}, false);
            AddAnimationCycle(spriteSheet, "swim", new[] {18, 19, 20, 21, 22, 23});
            AddAnimationCycle(spriteSheet, "kick", new[] {15}, false, 0.3f);
            AddAnimationCycle(spriteSheet, "punch", new[] {26}, false, 0.3f);
            AddAnimationCycle(spriteSheet, "cool", new[] {17}, false, 0.3f);
            entity.Attach(new AnimatedSprite(spriteSheet, "idle"));
            entity.Attach(new Transform2(position, 0, Vector2.One * 4));
            entity.Attach(new Body {Position = position, Size = new Vector2(32, 64), BodyType = BodyType.Dynamic});
            entity.Attach(new Player());
            return entity;
        }

        public Entity CreateBlue(Vector2 position)
        {
            var dudeTexture = _contentManager.Load<Texture2D>("blueguy");
            var dudeAtlas = TextureAtlas.Create("blueguyAtlas", dudeTexture, 16, 16);

            var entity = _world.CreateEntity();
            var spriteSheet = new SpriteSheet {TextureAtlas = dudeAtlas};
            AddAnimationCycle(spriteSheet, "idle", new[] {0, 1, 2, 3, 2, 1});
            AddAnimationCycle(spriteSheet, "walk", new[] {6, 7, 8, 9, 10, 11});
            AddAnimationCycle(spriteSheet, "jump", new[] {10, 12}, false, 1.0f);
            entity.Attach(new AnimatedSprite(spriteSheet, "idle") {Effect = SpriteEffects.FlipHorizontally});
            entity.Attach(new Transform2(position, 0, Vector2.One * 4));
            entity.Attach(new Body {Position = position, Size = new Vector2(32, 64), BodyType = BodyType.Dynamic});
            entity.Attach(new Enemy());
            return entity;
        }

        private void AddAnimationCycle(SpriteSheet spriteSheet, string name, int[] frames, bool isLooping = true, float frameDuration = 0.1f)
        {
            var cycle = new SpriteSheetAnimationCycle();
            foreach (var f in frames)
            {
                cycle.Frames.Add(new SpriteSheetAnimationFrame(f, frameDuration));
            }

            cycle.IsLooping = isLooping;

            spriteSheet.Cycles.Add(name, cycle);
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