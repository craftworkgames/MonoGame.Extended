using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Sandbox.Components;

namespace Sandbox.Systems
{
    public class RainfallSystem : EntityUpdateSystem
    {
        private readonly EntityWorld _world;
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Raindrop> _raindropMapper;

        public RainfallSystem(EntityWorld world)
            : base(Aspect.All(typeof(Transform2), typeof(Raindrop)))
        {
            _world = world;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _raindropMapper = mapperService.GetMapper<Raindrop>();
        }

        public override void Update(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();

            foreach (var entity in ActiveEntities)
            {
                var transform = _transformMapper.Get(entity);
                var raindrop = _raindropMapper.Get(entity);

                raindrop.Velocity += new Vector2(0, 10) * elapsedSeconds;
                transform.Position += raindrop.Velocity * elapsedSeconds;

                if (transform.Position.Y >= 480)
                    _world.DestroyEntity(entity);
            }
        }
    }
}