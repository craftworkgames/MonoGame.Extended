using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Platformer.Collisions;
using World = Platformer.Collisions.World;

namespace Platformer.Systems
{
    public class WorldSystem : EntityProcessingSystem
    {
        private readonly World _world;
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Body> _bodyMapper;

        public WorldSystem()
            : base(Aspect.All(typeof(Body), typeof(Transform2)))
        {
            _world = new World(new Vector2(0, 60));
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _bodyMapper = mapperService.GetMapper<Body>();
        }

        protected override void OnEntityAdded(int entityId)
        {
            var body = _bodyMapper.Get(entityId);
            _world.AddBody(body);
        }

        protected override void OnEntityRemoved(int entityId)
        {
            var body = _bodyMapper.Get(entityId);
            _world.RemoveBody(body);
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _world.Update(gameTime.GetElapsedSeconds());
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            var transform = _transformMapper.Get(entityId);
            var body = _bodyMapper.Get(entityId);
            transform.Position = body.Position;
        }
    }
}