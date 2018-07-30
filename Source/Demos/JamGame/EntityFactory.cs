using JamGame.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
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
            entity.Attach(new Body { Size = new Size2(16, 16) });
            entity.Attach(new Enemy());
        }

        public void SpawnZombie(float x, float y)
        {
            var entity = World.CreateEntity();
            entity.Attach(new Sprite(_tileset.GetTile(165)));
            entity.Attach(new Transform2(x, y));
            entity.Attach(new Body {Size = new Size2(16, 16)});
            entity.Attach(new Enemy());
        }

        public void SpawnPurpleThing(float x, float y)
        {
            var entity = World.CreateEntity();
            entity.Attach(new Sprite(_tileset.GetTile(213)));
            entity.Attach(new Transform2(x, y));
            entity.Attach(new Body { Size = new Size2(16, 16) });
            entity.Attach(new Enemy());
        }

        public Entity SpawnFireball(float x, float y)
        {
            var entity = World.CreateEntity();
            entity.Attach(new Sprite(_tileset.GetTile(240)));
            entity.Attach(new Transform2(x, y) { Scale = Vector2.One * 0.75f });
            entity.Attach(new Body { Size = new Size2(8, 8) });
            entity.Attach(new Projectile());
            return entity;
        }
    }
}