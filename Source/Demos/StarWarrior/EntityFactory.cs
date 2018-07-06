using System;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using StarWarrior.Components;

namespace StarWarrior
{
    public class EntityFactory
    {
        public EntityFactory()
        {
        }

        // TODO: Remove this property injection.
        public World World { get; set; }

        public Entity CreateMissile()
        {
            var entity = World.CreateEntity();
            entity.Attach(new Transform2());
            entity.Attach(new SpatialFormComponent { SpatialFormFile = "Missile" });
            entity.Attach(new PhysicsComponent());
            entity.Attach(new ExpiresComponent { LifeTime = TimeSpan.FromMilliseconds(2000) });
            return entity;
        }

        public Entity CreateShipExplosion()
        {
            var entity = World.CreateEntity();
            entity.Attach(new Transform2());
            entity.Attach(new SpatialFormComponent { SpatialFormFile = "ShipExplosion" });
            entity.Attach(new ExpiresComponent { LifeTime = TimeSpan.FromMilliseconds(1000) });
            return entity;
        }

        public Entity CreateBulletExplosion()
        {
            var entity = World.CreateEntity();
            entity.Attach(new Transform2());
            entity.Attach(new SpatialFormComponent { SpatialFormFile = "BulletExplosion" });
            entity.Attach(new ExpiresComponent { LifeTime = TimeSpan.FromMilliseconds(1000) });
            return entity;
        }

        public Entity CreateEnemyShip()
        {
            var entity = World.CreateEntity();
            entity.Attach(new Transform2());
            entity.Attach(new SpatialFormComponent { SpatialFormFile = "EnemyShip" });
            entity.Attach(new HealthComponent { Points = 10, MaximumPoints = 10 });
            entity.Attach(new WeaponComponent { ShootDelay = TimeSpan.FromSeconds(2) });
            entity.Attach(new PhysicsComponent());
            entity.Attach(new EnemyComponent());
            return entity;
        }
    }
}