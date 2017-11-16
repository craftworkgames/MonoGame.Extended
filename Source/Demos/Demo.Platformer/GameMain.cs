using System.Linq;
using System.Reflection;
using Demo.Platformer.Entities;
using Demo.Platformer.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.Platformer
{
    public class GameMain : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        private SpriteBatch _spriteBatch;
        private OrthographicCamera _camera;
        private TiledMap _map;
        private TiledMapRenderer _mapRenderer;
        private EntityComponentSystem _ecs;
        private EntityFactory _entityFactory;

        public GameMain()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            _ecs = new EntityComponentSystem(this);
            Services.AddService(_ecs);

            _ecs.Scan(Assembly.GetExecutingAssembly());

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new OrthographicCamera(viewportAdapter);
            Services.AddService(_camera);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(_spriteBatch);

            _map = Content.Load<TiledMap>("level-1");
            _mapRenderer = new TiledMapRenderer(GraphicsDevice, _map);
            _entityFactory = new EntityFactory(_ecs, Content);

            MapLayers.BackgroundLayer = _map.Layers.IndexOf(_map.GetLayer("background"));
            MapLayers.SolidsLayer = _map.Layers.IndexOf(_map.GetLayer("solids"));
            MapLayers.DecorationsLayer = _map.Layers.IndexOf(_map.GetLayer("decorations"));
            MapLayers.Decorations2Layer = _map.Layers.IndexOf(_map.GetLayer("decorations2"));
            MapLayers.DeadliesLayer = _map.Layers.IndexOf(_map.GetLayer("deadlies"));

            var service = new TiledObjectToEntityService(_entityFactory);
            var spawnPoint = _map.GetLayer<TiledMapObjectLayer>("entities").Objects.Single(i => i.Type == "Spawn").Position;

            service.CreateEntities(_map.GetLayer<TiledMapObjectLayer>("entities").Objects);
        }

        private static class MapLayers
        {
            public static int BackgroundLayer { get; set; }
            public static int SolidsLayer { get; set; }
            public static int DecorationsLayer { get; set; }
            public static int Decorations2Layer { get; set; }
            public static int DeadliesLayer { get; set; }
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);

            _ecs.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var viewMatrix = _camera.GetViewMatrix();
            var projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, 0, 0f, -1f);

            // painter's algorithm; just draw things in the expected order

            _mapRenderer.Draw(MapLayers.BackgroundLayer, ref viewMatrix, ref projectionMatrix);
            _mapRenderer.Draw(MapLayers.SolidsLayer);
            _mapRenderer.Draw(MapLayers.DecorationsLayer, ref viewMatrix, ref projectionMatrix);

            _ecs.Draw(gameTime);

            _mapRenderer.Draw(MapLayers.Decorations2Layer, ref viewMatrix, ref projectionMatrix);
            _mapRenderer.Draw(MapLayers.DeadliesLayer, ref viewMatrix, ref projectionMatrix);

            base.Draw(gameTime);
        }
    }
}