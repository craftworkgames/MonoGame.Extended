using System;
using System.Linq;
using Demo.Platformer.Entities.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.ViewportAdapters;
using Demo.Platformer.Entities.Components;
using MonoGame.Extended.Entities.Components;
using Demo.Platformer.Entities.Factories;
using Demo.Platformer.Entities;

namespace Demo.Platformer.Screens
{
    public class GameScreen : Screen
    {
        public GameScreen(IServiceProvider services, GraphicsDevice graphicsDevice, GameWindow window)
        {
            Services = services;
            GraphicsDevice = graphicsDevice;
            Window = window;
        }

        private Camera2D _camera;
        private TiledMap _map;
        private TiledMapRenderer _mapRenderer;
        private EntityComponentSystem _entityComponentSystem;

        public IServiceProvider Services { get; }
        public ContentManager Content { get; private set; }
        public GraphicsDevice GraphicsDevice { get; }
        public GameWindow Window { get; }

        public override void Initialize()
        {
            base.Initialize();
            Content = new ContentManager(Services, "Content");
        }

        public override void LoadContent()
        {
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);


            _entityComponentSystem = new EntityComponentSystem();
            var factories = new TiledEntityFactoryCollection();

            _entityComponentSystem.RegisterComponents();
            _entityComponentSystem.RegisterEntities(factories);
            _entityComponentSystem.RegisterSystems(new SpriteBatch(GraphicsDevice), _camera);

            _entityComponentSystem.LoadContent(Content);
            factories.LoadContent(Content);

            _map = Content.Load<TiledMap>("level-1");
            _mapRenderer = new TiledMapRenderer(GraphicsDevice);

            factories.BuildFromMap(_entityComponentSystem, _map);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _entityComponentSystem.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.Black);

            var viewMatrix = _camera.GetViewMatrix();
            var projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, 0, 0f, -1f);

            // painter's algorithm; just draw things in the expected order

            var backgroundLayer = _map.GetLayer("background");
            _mapRenderer.Draw(backgroundLayer, ref viewMatrix, ref projectionMatrix);

            var solidsLayer = _map.GetLayer("solids");
            _mapRenderer.Draw(solidsLayer);

            var decorationsLayer = _map.GetLayer("decorations");
            _mapRenderer.Draw(decorationsLayer, ref viewMatrix, ref projectionMatrix);

            _entityComponentSystem.Draw(gameTime);

            var decorations2Layer = _map.GetLayer("decorations2");
            _mapRenderer.Draw(decorations2Layer, ref viewMatrix, ref projectionMatrix);

            var deadliesLayer = _map.GetLayer("deadlies");
            _mapRenderer.Draw(deadliesLayer, ref viewMatrix, ref projectionMatrix);
        }
    }
}
