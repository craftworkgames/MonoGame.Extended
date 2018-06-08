using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Legacy;
using Platformer.Collisions;

namespace Platformer.Systems
{
    [Aspect(AspectType.All, typeof(Body), typeof(Transform2))]
    [EntitySystem(GameLoopType.Update, Layer = 0)]
    public class WorldSystem : EntityProcessingSystem
    {
        private readonly World _world;

        public WorldSystem()
        {
            _world = new World(new Vector2(0, 60));// {OnCollision = OnCollision};
        }

        public override void OnEntityAdded(Entity entity)
        {
            var body = entity.Get<Body>();
            _world.AddBody(body);
        }

        public override void OnEntityRemoved(Entity entity)
        {
            var body = entity.Get<Body>();
            _world.RemoveBody(body);
        }

        protected override void Process(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();
            _world.Update(elapsedSeconds);

            base.Process(gameTime);
        }

        protected override void Process(GameTime gameTime, Entity entity)
        {
            var transform = entity.Get<Transform2>();
            var body = entity.Get<Body>();

            transform.Position = body.Position;
        }
    }
}