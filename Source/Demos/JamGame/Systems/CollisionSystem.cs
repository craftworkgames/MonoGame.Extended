using JamGame.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace JamGame.Systems
{
    public class CollisionSystem : EntityUpdateSystem
    {
        private ComponentMapper<Body> _bodyMapper;
        private ComponentMapper<Transform2> _transformMapper;

        public CollisionSystem()
            : base(Aspect.All(typeof(Body), typeof(Transform2)))
        {

        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _bodyMapper = mapperService.GetMapper<Body>();
            _transformMapper = mapperService.GetMapper<Transform2>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entityA in ActiveEntities)
            {
                var bodyA = _bodyMapper.Get(entityA);
                var transformA = _transformMapper.Get(entityA);
                var rectangleA = GetBoundingRectangle(transformA, bodyA);

                bodyA.IsHit = false;

                foreach (var entityB in ActiveEntities)
                {
                    var bodyB = _bodyMapper.Get(entityB);
                    var transformB = _transformMapper.Get(entityB);
                    var rectangleB = GetBoundingRectangle(transformB, bodyB);

                    if (entityA != entityB && rectangleA.Intersects(rectangleB))
                    {
                        bodyA.IsHit = true;
                        bodyB.IsHit = true;
                    }
                }
            }
        }

        private static RectangleF GetBoundingRectangle(Transform2 transform, Body body)
        {
            return new RectangleF(transform.Position.X - body.Size.Width / 2f, transform.Position.Y + body.Size.Height / 2f, body.Size.Width, body.Size.Height);
        }
    }
}