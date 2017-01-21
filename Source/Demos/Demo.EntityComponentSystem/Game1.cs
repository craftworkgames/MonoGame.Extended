using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace Demo.EntityComponentSystem
{
    class LogoTag
    {
        
    }

    class LogoSystem : EntitySystem
    {
        private Texture2D _texture;

        public override void LoadContent(ContentManager contentManager)
        {
            _texture = contentManager.Load<Texture2D>("logo-square-128");
        }

        public override void EntityCreated(Entity entity)
        {
            if (entity.HasComponent<LogoTag>())
            {
                SpriteComponent sprite = entity.GetComponent<SpriteComponent>();
                sprite.TextureRegion = new Sprite(_texture).TextureRegion;
                sprite.Position = new Vector2(400, 240);
                sprite.Origin = (sprite.TextureRegion.Size / 2);
            }
        }

        public override void Update(Entity entity, GameTime gameTime)
        {
            if (entity.HasComponent<LogoTag>())
                entity.GetComponent<TransformComponent>().Rotation += gameTime.GetElapsedSeconds();
        }
    }

    class MotwTag
    {

    }

    class MotwSystem : EntitySystem
    {
        private SpriteSheetAnimationFactory _animation;

        public override void LoadContent(ContentManager contentManager)
        {
            var texture = contentManager.Load<Texture2D>("motw");
            var textureAtlas = TextureAtlas.Create(texture, 52, 72);

            _animation = new SpriteSheetAnimationFactory(textureAtlas);
            _animation.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
            _animation.Add("walkSouth", new SpriteSheetAnimationData(new[] { 0, 1, 2, 1 }, isLooping: true));
            _animation.Add("walkWest", new SpriteSheetAnimationData(new[] { 12, 13, 14, 13 }, isLooping: true));
            _animation.Add("walkEast", new SpriteSheetAnimationData(new[] { 24, 25, 26, 25 }, isLooping: true));
            _animation.Add("walkNorth", new SpriteSheetAnimationData(new[] { 36, 37, 38, 37 }, isLooping: true));
        }

        public override void EntityCreated(Entity entity)
        {
            if (entity.HasComponent<MotwTag>())
            {
                var sprite = entity.GetComponent<SpriteComponent>();
                sprite.AnimationFactory = _animation;
                sprite.Play("walkSouth");
                sprite.Position = new Vector2(50, 50);
                sprite.Origin = sprite.TextureRegion.Size / 2;
            }
        }
    }

    public class Game1 : Game
    {
        private MonoGame.Extended.Entities.EntityComponentSystem _entityComponentSystem;

        public Game1()
        {
            new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            var camera = new Camera2D(new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480));
            var spriteBatch = new SpriteBatch(GraphicsDevice);

            _entityComponentSystem = new MonoGame.Extended.Entities.EntityComponentSystem(this);
            Components.Add(_entityComponentSystem);

            _entityComponentSystem.RegisterComponent<LogoTag>(() => new LogoTag());
            _entityComponentSystem.RegisterComponent<MotwTag>(() => new MotwTag());
            _entityComponentSystem.RegisterComponent<TransformComponent>(() => new TransformComponent());
            _entityComponentSystem.RegisterComponent<SpriteComponent>(() => new SpriteComponent());

            _entityComponentSystem.RegisterEntity("logo", new Type[] { typeof(LogoTag), typeof(SpriteComponent), typeof(TransformComponent) });
            _entityComponentSystem.RegisterEntity("motw", new Type[] { typeof(MotwTag), typeof(SpriteComponent) });

            _entityComponentSystem.RegisterSystem(new SpriteBatchSystem(spriteBatch, camera));
            _entityComponentSystem.RegisterSystem(new AnimatedSpriteSystem());
            _entityComponentSystem.RegisterSystem(new ParticleEmitterSystem());
            _entityComponentSystem.RegisterSystem(new LogoSystem());
            _entityComponentSystem.RegisterSystem(new MotwSystem());

            _entityComponentSystem.Initialize();

            _entityComponentSystem.CreateEntity("logo");
            _entityComponentSystem.CreateEntity("motw");

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }
    }
}