using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Entities;
using StarWarrior.Components;
using StarWarrior.Systems;

namespace StarWarrior
{
    public class GameMain : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly FramesPerSecondCounter _fpsCounter = new FramesPerSecondCounter();
        private readonly Random _random = new Random();

        private EntityFactory _entityFactory;
        private SpriteBatch _spriteBatch;
        private BitmapFont _font;
        private World _world;

        public GameMain()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = false;
            IsFixedTimeStep = true;

            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 600,
                PreferredBackBufferFormat = SurfaceFormat.Color,
                PreferMultiSampling = false,
                PreferredDepthStencilFormat = DepthFormat.None,
                SynchronizeWithVerticalRetrace = true,
            };
        }

        protected override void LoadContent()
        {
            _entityFactory = new EntityFactory();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<BitmapFont>("montserrat-32");

            _world = new WorldBuilder()
                .AddSystem(new CollisionSystem())
                .AddSystem(new EnemyShipMovementSystem(GraphicsDevice))
                .AddSystem(new EnemyShooterSystem(_entityFactory))
                .AddSystem(new EnemySpawnSystem(GraphicsDevice, _entityFactory))
                .AddSystem(new ExpirationSystem())
                .AddSystem(new HealthBarRenderSystem(_spriteBatch, _font))
                .AddSystem(new HudRenderSystem(GraphicsDevice, _spriteBatch, _font))
                .AddSystem(new MovementSystem())
                .AddSystem(new PlayerShipControlSystem(_entityFactory))
                .AddSystem(new RenderSystem(_spriteBatch, Content))
                .Build();

            _entityFactory.World = _world;

            InitializePlayerShip();
            InitializeEnemyShips();

        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            var gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();

            _fpsCounter.Update(gameTime);
            _world.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _fpsCounter.Draw(gameTime);
            var fps = $"FPS: {_fpsCounter.FramesPerSecond}";

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _world.Draw(gameTime);

            _spriteBatch.DrawString(_font, fps, new Vector2(16, 16), Color.White);

//#if DEBUG
//            var entityCount = $"Active Entities Count: {_entityManager.ActiveEntitiesCount}";
//            //var removedEntityCount = $"Removed Entities TotalCount: {_ecs.TotalEntitiesRemovedCount}";
//            var totalEntityCount = $"Allocated Entities Count: {_entityManager.TotalEntitiesCount}";

//            _spriteBatch.DrawString(_font, entityCount, new Vector2(16, 62), Color.White);
//            _spriteBatch.DrawString(_font, totalEntityCount, new Vector2(16, 92), Color.White);
//            //_spriteBatch.DrawString(_font, removedEntityCount, new Vector2(32, 122), Color.Yellow);
//#endif

            _spriteBatch.End();
        }

        private void InitializeEnemyShips()
        {
            var viewport = GraphicsDevice.Viewport;

            var random = new Random();
            for (var index = 0; 2 > index; ++index)
            {
                var entity = _entityFactory.CreateEnemyShip();

                var transform = entity.Get<Transform2>();
                var position = new Vector2
                {
                    X = random.Next(viewport.Width - 100) + 50,
                    Y = random.Next((int)(viewport.Height * 0.75 + 0.5)) + 50
                };
                transform.Position = position;

                var physics = entity.Get<PhysicsComponent>();
                physics.Speed = 0.05f;
                physics.Angle = random.Next() % 2 == 0 ? 0 : 180;
            }
        }

        private void InitializePlayerShip()
        {
            var viewport = GraphicsDevice.Viewport;

            var entity = _world.CreateEntity();
            entity.Attach(new Transform2(x: viewport.Width * 0.5f, y: viewport.Height - 50f));
            entity.Attach(new SpatialFormComponent {SpatialFormFile = "PlayerShip" });
            entity.Attach(new HealthComponent {Points = 30, MaximumPoints = 30});
            entity.Attach(new PlayerComponent());
        }
    }
}
