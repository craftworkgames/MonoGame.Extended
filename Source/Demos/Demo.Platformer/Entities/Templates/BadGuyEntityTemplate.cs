using System;
using System.Diagnostics.CodeAnalysis;
using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Entities;

namespace Demo.Platformer.Entities.Templates
{
    [EntityTemplate(Name)]
    public class BadGuyTemplate : EntityTemplate
    {
        public const string Name = "BadGuyTemplate";

        protected override void Initialize()
        {

        }

        [SuppressMessage("ReSharper", "UnusedVariable")]
        protected override void Build(Entity entity)
        {
            var transform = entity.Attach<TransformComponent>();
            var collision = entity.Attach<CollisionBodyComponent>();

            collision.Origin = Vector2.One * 0.5f;

            //var textureRegion = _characterTextureAtlas[90];
            //var sprite = entity.Attach<SpriteComponent>();
            //sprite.Origin = new Vector2(textureRegion.Width / 2f, textureRegion.Height / 2f);

            //var animatedSprite = entity.Attach<AnimationComponent>();
            //animatedSprite.
            //var animationFactory = animatedSprite.AnimationFactory = new SpriteSheetAnimationFactory(_characterTextureAtlas); ;

            //animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 100 }, 1.0f));
            //animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 96, 97, 98, 99 }, isPingPong: true));

            //animatedSprite.Play("walk");

            var health = entity.Attach<HealthComponent>();
            health.Points = health.MaximumPoints = 20;

            entity.Attach<EnemyAiComponent>();

            //entity.Group = "SHIPS";

            //var transform = entity.Attach<TransformComponent>();

            //var spatial = entity.Attach<SpatialFormComponent>();
            //spatial.SpatialFormFile = "EnemyShip";

            //var health = entity.Attach<HealthComponent>();
            //health.Points = health.MaximumPoints = 10;

            //var weapon = entity.Attach<WeaponComponent>();
            //weapon.ShootDelay = TimeSpan.FromSeconds(2);

            //var enemy = entity.Attach<EnemyComponent>();

            //var physics = entity.Attach<PhysicsComponent>();
        }
    }
}
