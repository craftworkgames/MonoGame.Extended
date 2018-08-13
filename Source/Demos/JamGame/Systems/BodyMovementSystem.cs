using JamGame.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace JamGame.Systems
{
    public class BodyMovementSystem : EntityUpdateSystem
    {
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Body> _bodyMapper;

        public BodyMovementSystem() 
            : base(Aspect.All(typeof(Transform2), typeof(Body)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _bodyMapper = mapperService.GetMapper<Body>();
        }

        public override void Update(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();

            foreach (var entity in ActiveEntities)
            {
                var transform = _transformMapper.Get(entity);
                var body = _bodyMapper.Get(entity);

                transform.Position += body.Velocity * elapsedSeconds;
            }
        }
    }
}