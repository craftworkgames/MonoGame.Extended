using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using Platformer.Collisions;

namespace Platformer.Systems
{
    [Aspect(AspectType.All, typeof(Body))]
    [EntitySystem(GameLoopType.Update, Layer = 0)]
    public class WorldSystem : EntityProcessingSystem
    {
        private readonly World _world;

        public WorldSystem()
        {
            _world = new World(new Vector2(0, 60)) {OnCollision = OnCollision};
        }

        public override void OnEntityAdded(Entity entity)
        {
            var body = entity.Get<Body>();
            _world.Bodies.Add(body);
        }

        public override void OnEntityRemoved(Entity entity)
        {
            var body = entity.Get<Body>();
            _world.Bodies.Remove(body);
        }

        protected override void Process(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();
            _world.Update(elapsedSeconds);

            base.Process(gameTime);
        }

        protected override void Process(GameTime gameTime, Entity entity)
        {
        }

        private static void OnCollision(Manifold manifold)
        {
            var body = manifold.BodyB.BodyType == BodyType.Dynamic ? manifold.BodyB : manifold.BodyA;

            body.Position -= manifold.Normal * manifold.Penetration;

            if (manifold.Normal.Y < 0 || manifold.Normal.Y > 0)
                body.Velocity.Y = 0;

            //if (manifold.Normal.X < 0 || manifold.Normal.X > 0)
            //    body.Velocity = new Vector2(0, body.Velocity.Y);
        }
    }
}