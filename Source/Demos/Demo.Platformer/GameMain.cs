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
using MonoGame.Extended.ViewportAdapters;

namespace Demo.Platformer
{
    public class GameMain : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        private SpriteBatch _spriteBatch;
        private Camera2D _camera;
        private TiledMap _map;
        private TiledMapRenderer _mapRenderer;
        private EntityComponentSystemManager _ecs;
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
            _ecs = new EntityComponentSystemManager(this);
            Services.AddService(_ecs);

            _ecs.Scan(Assembly.GetExecutingAssembly());

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);
            Services.AddService(_camera);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(_spriteBatch);

            _map = Content.Load<TiledMap>("level-1");
            _mapRenderer = new TiledMapRenderer(GraphicsDevice);
            _entityFactory = new EntityFactory(_ecs, Content);

            var service = new TiledObjectToEntityService(_entityFactory);
            var spawnPoint = _map.GetLayer<TiledMapObjectLayer>("entities").Objects.Single(i => i.Type == "Spawn").Position;

            service.CreateEntities(_map.GetLayer<TiledMapObjectLayer>("entities").Objects);
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

            var backgroundLayer = _map.GetLayer("background");
            _mapRenderer.Draw(backgroundLayer, ref viewMatrix, ref projectionMatrix);

            var solidsLayer = _map.GetLayer("solids");
            _mapRenderer.Draw(solidsLayer);

            var decorationsLayer = _map.GetLayer("decorations");
            _mapRenderer.Draw(decorationsLayer, ref viewMatrix, ref projectionMatrix);

            _ecs.Draw(gameTime);

            var decorations2Layer = _map.GetLayer("decorations2");
            _mapRenderer.Draw(decorations2Layer, ref viewMatrix, ref projectionMatrix);

            var deadliesLayer = _map.GetLayer("deadlies");
            _mapRenderer.Draw(deadliesLayer, ref viewMatrix, ref projectionMatrix);
            base.Draw(gameTime);
        }
    }
}