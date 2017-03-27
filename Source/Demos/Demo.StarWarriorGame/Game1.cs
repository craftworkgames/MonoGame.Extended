﻿using System;
using System.Reflection;
using Demo.StarWarriorGame.Components;
using Demo.StarWarriorGame.Templates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Entities;

namespace Demo.StarWarriorGame
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly FramesPerSecondCounter _fpsCounter = new FramesPerSecondCounter();
        private readonly Random _random = new Random();
        private readonly EntityComponentSystemManager _ecs;

        private SpriteBatch _spriteBatch;
        private BitmapFont _font;

        public Game1()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = false;
            IsFixedTimeStep = false;

            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferHeight = 720,
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferFormat = SurfaceFormat.Color,
                PreferMultiSampling = false,
                PreferredDepthStencilFormat = DepthFormat.None,
#if DEBUG
                SynchronizeWithVerticalRetrace = false,
#else
                SynchronizeWithVerticalRetrace = true,
#endif
        };

            _ecs = new EntityComponentSystemManager(this);

            // scan for components and systems in provided assemblies
            _ecs.Scan(Assembly.GetExecutingAssembly());

            Services.AddService(Content);
            Services.AddService(_ecs);
            Services.AddService(_random);
        }

        protected override void Initialize()
        {
            base.Initialize();

            InitializePlayerShip();
            InitializeEnemyShips();
        }

        protected override void LoadContent()
        {
            var graphicsDevice = GraphicsDevice;
           
            _spriteBatch = new SpriteBatch(graphicsDevice);
            Services.AddService(_spriteBatch);

            _font = Content.Load<BitmapFont>("montserrat-32");
            Services.AddService(_font);
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            var gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();

            _fpsCounter.Update(gameTime);
            _ecs.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _fpsCounter.Draw(gameTime);
            var fps = $"FPS: {_fpsCounter.FramesPerSecond}";

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _ecs.Draw(gameTime);

            _spriteBatch.DrawString(_font, fps, new Vector2(32, 32), Color.Yellow);

#if DEBUG
            var entityCount = $"Entities Count: {_ecs.Entities.Count}";
            var removedEntityCount = $"Removed Entities Count: {_ecs.TotalEntitiesRemovedCount}";
            var totalEntityCount = $"Total Entities Count: {_ecs.TotalEntitiesCreatedCount}";

            _spriteBatch.DrawString(_font, entityCount, new Vector2(32, 62), Color.Yellow);
            _spriteBatch.DrawString(_font, removedEntityCount, new Vector2(32, 92), Color.Yellow);
            _spriteBatch.DrawString(_font, totalEntityCount, new Vector2(32, 122), Color.Yellow);
#endif

            _spriteBatch.End();
        }

        private void InitializeEnemyShips()
        {
            var viewport = GraphicsDevice.Viewport;

            var random = new Random();
            for (var index = 0; 2 > index; ++index)
            {
                var entity = _ecs.CreateEntityFromTemplate(EnemyShipTemplate.Name);

                var transform = entity.Get<TransformComponent>();
                var position = new Vector2
                {
                    X = random.Next(viewport.Width - 100) + 50,
                    Y = random.Next((int) (viewport.Height * 0.75 + 0.5)) + 50
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

            var entity = _ecs.CreateEntity();
            entity.Group = "SHIPS";

            var transform = entity.Attach<TransformComponent>();
            var position = new Vector2
            {
                X = viewport.Width * 0.5f,
                Y = viewport.Height - 50
            };
            transform.Position = position;

            var spatial = entity.Attach<SpatialFormComponent>();
            spatial.SpatialFormFile = "PlayerShip";

            var health = entity.Attach<HealthComponent>();
            health.Health = health.MaximumHealth = 30;

            entity.Attach<PlayerComponent>();
        }

    }
}
