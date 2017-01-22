using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.EntityComponentSystem
{
    class Rotator
    {
        public float Speed { get; set; } = 1f;
    }

    class RotatorSystem : EntitySystem
    {
        protected override void Update(Entity entity, GameTime gameTime)
        {
            var transform = entity.GetComponent<Transform>();
            var rotator = entity.GetComponent<Rotator>();

            if (transform != null && rotator != null)
                transform.Rotation += gameTime.GetElapsedSeconds() * rotator.Speed;
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

            _entityComponentSystem = new MonoGame.Extended.Entities.EntityComponentSystem();

            _entityComponentSystem.RegisterComponent<Rotator>(() => new Rotator());
            _entityComponentSystem.RegisterComponent<Transform>(() => new Transform());
            _entityComponentSystem.RegisterComponent<SpriteComponent>(() => new SpriteComponent());

            _entityComponentSystem.RegisterEntity("logo", new Type[] { typeof(Rotator), typeof(SpriteComponent), typeof(Transform) });
            _entityComponentSystem.RegisterEntity("motw", new Type[] { typeof(SpriteComponent) });

            _entityComponentSystem.RegisterSystem(new SpriteBatchSystem(spriteBatch, camera));
            _entityComponentSystem.RegisterSystem(new SpriteAnimatorSystem());
            _entityComponentSystem.RegisterSystem(new ParticleEmitterSystem());
            _entityComponentSystem.RegisterSystem(new RotatorSystem());

            _entityComponentSystem.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _entityComponentSystem.CreateEntity("logo", (entity) =>
            {
                var texture = Content.Load<Texture2D>("logo-square-128");
                var sprite = entity.GetComponent<SpriteComponent>();
                sprite.TextureRegion = new Sprite(texture).TextureRegion;
                sprite.Position = new Vector2(400, 240);
                sprite.Origin = (sprite.TextureRegion.Size / 2);
            });

            _entityComponentSystem.CreateEntity("motw", (entity) =>
            {
                var texture = Content.Load<Texture2D>("motw");
                var textureAtlas = TextureAtlas.Create(texture, 52, 72);

                var animation = new SpriteSheetAnimationFactory(textureAtlas);
                animation.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
                animation.Add("walkSouth", new SpriteSheetAnimationData(new[] { 0, 1, 2, 1 }, isLooping: true));
                animation.Add("walkWest", new SpriteSheetAnimationData(new[] { 12, 13, 14, 13 }, isLooping: true));
                animation.Add("walkEast", new SpriteSheetAnimationData(new[] { 24, 25, 26, 25 }, isLooping: true));
                animation.Add("walkNorth", new SpriteSheetAnimationData(new[] { 36, 37, 38, 37 }, isLooping: true));

                var sprite = entity.GetComponent<SpriteComponent>();
                sprite.AnimationFactory = animation;
                sprite.Play("walkSouth");
                sprite.Position = new Vector2(50, 50);
                sprite.Origin = sprite.TextureRegion.Size / 2;
            });

            _entityComponentSystem.LoadContent(Content);
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _entityComponentSystem.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _entityComponentSystem.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}