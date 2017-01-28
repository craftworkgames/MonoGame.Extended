using Demo.Platformer.Entities.Components;
using Demo.Platformer.Entities.Factories;
using Demo.Platformer.Entities.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Entities.Systems;

namespace Demo.Platformer.Entities
{
    public static class EntityComponentSystemExtensions
    {
        public static void RegisterComponents(this EntityComponentSystem ecs)
        {
            ecs.RegisterComponent<CharacterState>();
            ecs.RegisterComponent<CollisionBody>();
            ecs.RegisterComponent<Deadly>();
            ecs.RegisterComponent<Enemy>();
            ecs.RegisterComponent<Player>();
            ecs.RegisterComponent<Transform>();
            ecs.RegisterComponent<SpriteComponent>();
        }

        public static void RegisterEntities(this EntityComponentSystem ecs, TiledEntityFactoryCollection factories)
        {
            Entities.RegisterEntities(ecs, factories);
        }

        public static void RegisterSystems(this EntityComponentSystem ecs, SpriteBatch spriteBatch, Camera2D camera)
        {
            ecs.RegisterSystem(new PlayerControllerSystem());
            ecs.RegisterSystem(new EnemyMovementSystem());
            ecs.RegisterSystem(new CharacterStateSystem());
            ecs.RegisterSystem(new CollisionSystem(gravity: new Vector2(0, 1150)));
            ecs.RegisterSystem(new ParticleEmitterSystem());
            ecs.RegisterSystem(new SpriteAnimatorSystem());
            ecs.RegisterSystem(new SpriteBatchSystem(spriteBatch, camera) { SamplerState = SamplerState.PointClamp });
        }
    }
}
