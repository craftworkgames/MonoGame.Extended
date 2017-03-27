using System.Reflection;
using Demo.EntityComponentSystem.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Entities;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.EntityComponentSystem
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private Camera2D _camera;

        private EntityComponentSystemManager _ecs;
        private Entity _entity;
        private Entity _animatedEntity;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            _ecs = new EntityComponentSystemManager(this);
            Services.AddService(_ecs);

            _ecs.Scan(Assembly.GetExecutingAssembly());

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);
            Services.AddService(_camera);

            var spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(spriteBatch);

            var logoTexture = Content.Load<Texture2D>("logo-square-128");

            _entity = _ecs.CreateEntity();

            var transform = _entity.Attach<TransformComponent>();
            transform.Position = new Vector2(200, 400);

            var sprite = _entity.Attach<SpriteComponent>();
            sprite.Texture = logoTexture;
            sprite.Origin = new Vector2(logoTexture.Width / 2f, logoTexture.Height / 2f);

            var motwTexture = Content.Load<Texture2D>("motw");
            var motwAtlas = TextureAtlas.Create("motw-atlas", motwTexture, 52, 72);
            var motwAnimationFactory = new SpriteSheetAnimationFactory(motwAtlas);
            motwAnimationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
            motwAnimationFactory.Add("walkSouth", new SpriteSheetAnimationData(new[] { 0, 1, 2, 1 }, isLooping: false));
            motwAnimationFactory.Add("walkWest", new SpriteSheetAnimationData(new[] { 12, 13, 14, 13 }, isLooping: false));
            motwAnimationFactory.Add("walkEast", new SpriteSheetAnimationData(new[] { 24, 25, 26, 25 }, isLooping: false));
            motwAnimationFactory.Add("walkNorth", new SpriteSheetAnimationData(new[] { 36, 37, 38, 37 }, isLooping: false));

            _animatedEntity = _ecs.CreateEntity();

            _animatedEntity.Attach<SpriteComponent>();
            _animatedEntity.Attach<TransformComponent>();
            var animatedSpriteComponent = _animatedEntity.Attach<AnimatedSpriteComponent>();
            animatedSpriteComponent.AnimationFactory = motwAnimationFactory;
            animatedSpriteComponent.Play("walkSouth").IsLooping = true;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            var transform = _entity.Get<TransformComponent>();
            transform.Rotation += deltaTime;

            _ecs.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _ecs.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}