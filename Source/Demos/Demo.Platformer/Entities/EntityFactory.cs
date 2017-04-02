using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Entities;
using MonoGame.Extended.TextureAtlases;
//using AnimationComponent = Demo.Platformer.Entities.Components.AnimationComponent;
using SpriteComponent = Demo.Platformer.Entities.Components.SpriteComponent;
using TransformComponent = Demo.Platformer.Entities.Components.TransformComponent;

namespace Demo.Platformer.Entities
{
    public static class Entities
    {
        public const string Player = "Player";
    }

    public class EntityFactory
    {
        private readonly EntityComponentSystemManager _ecs;
        private readonly EntityManager _entityManager;
        private TextureAtlas _characterTextureAtlas;

        public EntityFactory(EntityComponentSystemManager ecs, ContentManager contentManager)
        {
            _ecs = ecs;
            _entityManager = ecs.EntityManager;

            LoadContent(contentManager);
        }

        private void LoadContent(ContentManager content)
        {
            var texture = content.Load<Texture2D>("tiny-characters");
            _characterTextureAtlas = TextureAtlas.Create("tiny-characters-atlas", texture, 32, 32, 112);
        }

        public Entity CreatePlayer(Vector2 position)
        {
            var entity = _entityManager.CreateEntity();

            var transform = entity.Attach<TransformComponent>();
            transform.Position = position;

            var textureRegion = _characterTextureAtlas[0];
            var sprite = entity.Attach<SpriteComponent>();
            sprite.Origin = new Vector2(textureRegion.Width / 2f, textureRegion.Height / 2f);

            //var animatedSprite = entity.Attach<AnimationComponent>();
            //var animationFactory = animatedSprite.AnimationFactory = new SpriteSheetAnimationFactory(_characterTextureAtlas); ;

            //animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 12, 13 }, 1.0f));
            //animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 0, 1, 2, 3 }));
            //animationFactory.Add("jump", new SpriteSheetAnimationData(new[] { 8, 9 }, isLooping: false));

            //animatedSprite.Play("idle");

            var collision = entity.Attach<CollisionBodyComponent>();
            collision.Size = textureRegion.Size;
            collision.Origin = Vector2.One * 0.5f;

            var player = entity.Attach<PlayerComponent>();
            player.WalkSpeed = 220f;
            player.JumpSpeed = 420f;

            var health = entity.Attach<HealthComponent>();
            health.Points = 20;

            return entity;
        }

        public Entity CreateSolid(Vector2 position, Size2 size)
        {
            var entity = _entityManager.CreateEntity();
            var transform = entity.Attach<TransformComponent>();
            transform.Position = position;
            var collision = entity.Attach<CollisionBodyComponent>();
            collision.Size = size;
            collision.IsStatic = true;
            return entity;
        }

        public Entity CreateDeadly(Vector2 position, Size2 size)
        {
            var entity = _entityManager.CreateEntity();
            var transform = entity.Attach<TransformComponent>();
            transform.Position = position;

            var collision = entity.Attach<CollisionBodyComponent>();
            collision.Size = size;
            collision.IsStatic = true;

            return entity;
        }

        //public Entity CreateBloodExplosion(Vector2 position, float totalSeconds = 1.0f)
        //{
        //    var random = new FastRandom();
        //    var textureRegion = _characterTextureAtlas[0];
        //    var entity = _ecs.CreateEntity(position);
        //    var profile = Profile.Spray(new Vector2(0, -1), MathHelper.Pi);
        //    var term = TimeSpan.FromSeconds(totalSeconds);
        //    var particleEmitter = new ParticleEmitter(textureRegion, 32, term, profile)
        //    {
        //        Parameters = new ParticleReleaseParameters
        //        {
        //            Speed = new Range<float>(140, 200),
        //            Quantity = new Range<int>(32, 64),
        //            Rotation = new Range<float>(-MathHelper.TwoPi, MathHelper.TwoPi)
        //        },
        //        Modifiers = new IModifier[]
        //        {
        //            new LinearGravityModifier { Direction = Vector2.UnitY, Strength = 350 },
        //            new OpacityFastFadeModifier(),
        //            new RotationModifier { RotationRate = random.NextSingle(-MathHelper.TwoPi, MathHelper.TwoPi) }
        //        }
        //    };
        //    entity.AttachComponent(new TransformableComponent<ParticleEmitter>(particleEmitter));
        //    entity.Destroy(delaySeconds: totalSeconds);
        //    return entity;
        //}
    }
}
