using JamGame.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace JamGame.Systems
{
    public class CollisionResponseSystem : EntityProcessingSystem
    {
        private readonly EntityFactory _entityFactory;
        private ComponentMapper<Body> _bodyMapper;

        public CollisionResponseSystem(EntityFactory entityFactory) 
            : base(Aspect.All(typeof(Body), typeof(Transform2)))
        {
            _entityFactory = entityFactory;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _bodyMapper = mapperService.GetMapper<Body>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            var body = _bodyMapper.Get(entityId);

            if (body.IsHit)
            {
                var entity = GetEntity(entityId);

                if (entity.Has<Projectile>())
                {
                    _entityFactory.SpawnExplosion(entity.Get<Transform2>().Position, body.Velocity / 2f);
                    entity.Destroy();
                }


                if (entity.Has<Enemy>())
                {
                    _entityFactory.SpawnExplosion(entity.Get<Transform2>().Position, body.Velocity / 2f);
                    entity.Destroy();
                }
            }
        }
    }
}