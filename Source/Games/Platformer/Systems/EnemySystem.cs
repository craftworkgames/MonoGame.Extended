using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Platformer.Collisions;
using Platformer.Components;

namespace Platformer.Systems
{
    public class EnemySystem : EntityProcessingSystem
    {
        public EnemySystem() 
            : base(Aspect.All(typeof(Body), typeof(Enemy)))
        {
        }

        private ComponentMapper<Body> _bodyMapper;
        private ComponentMapper<Enemy> _enemyMapper;

        public override void Initialize(IComponentMapperService mapperService)
        {
            _enemyMapper = mapperService.GetMapper<Enemy>();
            _bodyMapper = mapperService.GetMapper<Body>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();
            var body = _bodyMapper.Get(entityId);
            var enemy = _enemyMapper.Get(entityId);

            enemy.TimeLeft -= elapsedSeconds;

            if (enemy.TimeLeft <= 0)
            {
                enemy.Speed = -enemy.Speed;
                enemy.TimeLeft = 1.0f;
            }

            body.Position = body.Position.Translate(enemy.Speed * elapsedSeconds, 0);
        }
    }
}