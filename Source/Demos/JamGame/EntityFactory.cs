using System.Linq;
using JamGame.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;

namespace JamGame
{
    public class EntityFactory
    {
        private readonly Tileset _tileset;

        public EntityFactory(Tileset tileset)
        {
            _tileset = tileset;
        }

        public World World { get; set; }

        public void SpawnPlayer(float x, float y)
        {
            var entity = World.CreateEntity();
            entity.Attach(new Sprite(_tileset.GetTile(232)));
            entity.Attach(new Transform2(x, y));
            entity.Attach(new Player());
            entity.Attach(new Body { Size = new Size2(16, 16) });
        }

        public void SpawnSkeleton(float x, float y)
        {
            var entity = World.CreateEntity();
            entity.Attach(new Sprite(_tileset.GetTile(145)));
            entity.Attach(new Transform2(x, y));
            entity.Attach(new Body { Size = new Size2(16, 16), Velocity = new Vector2(-8, 0) });
            entity.Attach(new Enemy());
        }

        public void SpawnZombie(float x, float y)
        {
            var entity = World.CreateEntity();
            entity.Attach(new Sprite(_tileset.GetTile(165)));
            entity.Attach(new Transform2(x, y));
            entity.Attach(new Body {Size = new Size2(16, 16), Velocity = new Vector2(-4, 0) });
            entity.Attach(new Enemy());
        }

        public void SpawnPurpleThing(float x, float y)
        {
            var entity = World.CreateEntity();
            entity.Attach(new Sprite(_tileset.GetTile(213)));
            entity.Attach(new Transform2(x, y));
            entity.Attach(new Body { Size = new Size2(16, 16), Velocity = new Vector2(-2, 0) });
            entity.Attach(new Enemy());
        }

        public Entity SpawnFireball(float x, float y)
        {
            var entity = World.CreateEntity();
            entity.Attach(new Sprite(_tileset.GetTile(240)));
            entity.Attach(new Transform2(x, y) { Scale = Vector2.One * 0.75f });
            entity.Attach(new Body { Size = new Size2(16, 16) });
            entity.Attach(new Projectile());
            return entity;
        }

        public void SpawnExplosion(Vector2 position, Vector2 velocity)
        {
            var frames = new[] {256, 257, 258, 259, 260, 261, 262, 263, 264};
            var animationFactory = new SpriteSheetAnimationFactory(frames.Select(f => _tileset.GetTile(f)));
            animationFactory.Add("explode", new SpriteSheetAnimationData(new []{0,1,2,3,4,5,6,7,8}, isLooping: false, frameDuration: 1f / 32f));
            var entity = World.CreateEntity();
            var animatedSprite = new AnimatedSprite(animationFactory);
            entity.Attach(animatedSprite);
            entity.Attach(new Transform2(position));
            entity.Attach(new Body {Velocity = velocity});
            animatedSprite.Play("explode", () => entity.Destroy());
        }
    }
}