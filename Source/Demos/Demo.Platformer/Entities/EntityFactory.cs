using System;
using Demo.Platformer.Entities.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace Demo.Platformer.Entities
{
    public static class Entities
    {
        public const string Player = "Player";
    }

    public class EntityFactory
    {
        private readonly EntityComponentSystem _entityComponentSystem;
        private TextureAtlas _characterTextureAtlas;

        public EntityFactory(EntityComponentSystem entityComponentSystem, ContentManager contentManager)
        {
            _entityComponentSystem = entityComponentSystem;

            LoadContent(contentManager);
        }

        private void LoadContent(ContentManager content)
        {
            var texture = content.Load<Texture2D>("tiny-characters");
            _characterTextureAtlas = TextureAtlas.Create(texture, 32, 32, 112);
        }

        public Entity CreatePlayer(Vector2 position)
        {
            var entity = _entityComponentSystem.CreateEntity(Entities.Player, position);
            var textureRegion = _characterTextureAtlas[0];
            var animationFactory = new SpriteSheetAnimationFactory(_characterTextureAtlas);

            animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 12, 13 }, 1.0f));
            animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 0, 1, 2, 3 }));
            animationFactory.Add("jump", new SpriteSheetAnimationData(new[] { 8, 9 }, isLooping: false));

            entity.AttachComponent(new AnimatedSprite(animationFactory));
            entity.AttachComponent(new BasicCollisionBody(textureRegion.Size, Vector2.One * 0.5f));
            entity.AttachComponent(new PlayerCollisionHandler());
            entity.AttachComponent(new CharacterState());
            entity.Tag = Entities.Player;

            return entity;
        }

        public Entity CreateSolid(Vector2 position, SizeF size)
        {
            var entity = _entityComponentSystem.CreateEntity(position);
            entity.AttachComponent(new BasicCollisionBody(size, Vector2.Zero) { IsStatic = true });
            return entity;
        }

        public Entity CreateDeadly(Vector2 position, SizeF size)
        {
            var entity = _entityComponentSystem.CreateEntity(position);
            entity.AttachComponent(new BasicCollisionBody(size, Vector2.Zero) { IsStatic = true, Tag = "Deadly" });
            return entity;
        }

        public Entity CreateBloodExplosion(Vector2 position, float totalSeconds = 1.0f)
        {
            var random = new FastRandom();
            var textureRegion = _characterTextureAtlas[0];
            var entity = _entityComponentSystem.CreateEntity(position);
            var profile = Profile.Spray(new Vector2(0, -1), MathHelper.Pi);
            var term = TimeSpan.FromSeconds(totalSeconds);
            var particleEmitter = new ParticleEmitter(textureRegion, 32, term, profile)
            {
                Parameters = new ParticleReleaseParameters
                {
                    Speed = new Range<float>(140, 200),
                    Quantity = new Range<int>(32, 64),
                    Rotation = new Range<float>(-MathHelper.TwoPi, MathHelper.TwoPi)
                },
                Modifiers = new IModifier[]
                {
                    new LinearGravityModifier { Direction = Vector2.UnitY, Strength = 350 },
                    new OpacityFastFadeModifier(),
                    new RotationModifier { RotationRate = random.NextSingle(-MathHelper.TwoPi, MathHelper.TwoPi) }
                }
            };
            entity.AttachComponent(particleEmitter);
            entity.Destroy(delaySeconds: totalSeconds);
            return entity;
        }

        public Entity CreateBadGuy(Vector2 position, SizeF size)
        {
            var entity = _entityComponentSystem.CreateEntity(position);
            var textureRegion = _characterTextureAtlas[90];
            var animationFactory = new SpriteSheetAnimationFactory(_characterTextureAtlas);

            animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 100 }, 1.0f));
            animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 96, 97, 98, 99 }, isPingPong: true));

            entity.AttachComponent(new AnimatedSprite(animationFactory, "walk"));
            entity.AttachComponent(new BasicCollisionBody(textureRegion.Size, Vector2.One * 0.5f) {Tag = "Deadly"});
            entity.AttachComponent(new EnemyCollisionHandler());
            entity.AttachComponent(new CharacterState());
            entity.AttachComponent(new EnemyAi());
            entity.Tag = "BadGuy";

            return entity;
        }
    }
}
