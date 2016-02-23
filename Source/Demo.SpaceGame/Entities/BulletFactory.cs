using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace Demo.SpaceGame.Entities
{
    public interface IBulletFactory
    {
        void Create(Vector2 position, Vector2 direction, float rotation, float speed);
    }

    public class BulletFactory : IBulletFactory
    {
        private readonly IEntityManager _entityManager;
        private readonly TextureRegion2D _bulletRegion;

        public BulletFactory(IEntityManager entityManager, TextureRegion2D bulletRegion)
        {
            _entityManager = entityManager;
            _bulletRegion = bulletRegion;
        }

        public void Create(Vector2 position, Vector2 direction, float rotation, float speed)
        {
            var velocity = direction * speed;
            var laser = new Laser(_bulletRegion, velocity)
            {
                Position = position,
                Rotation = rotation
            };
            _entityManager.AddEntity(laser);
        }
    }
}