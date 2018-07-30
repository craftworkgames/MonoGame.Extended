using System.Diagnostics;
using JamGame.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace JamGame.Systems
{
    public class CollisionResponseSystem : EntityProcessingSystem
    {
        private ComponentMapper<Body> _bodyMapper;

        public CollisionResponseSystem() 
            : base(Aspect.All(typeof(Body)))
        {
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

                var hasProjectile = entity.Has<Projectile>();
                var hasEnemy = entity.Has<Enemy>();

                if (hasProjectile && hasEnemy)
                    Debug.Fail("Wtf");

                if (entity.Has<Projectile>())
                    entity.Destroy();

                if (entity.Has<Enemy>())
                    entity.Destroy();
            }
        }
    }
}