using System;
using System.Linq;
using Demo.Platformer.Entities;
using Demo.Platformer.Entities.Systems;
using Demo.Platformer.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Maps.Renderers;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;

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
        private TiledMap _tiledMap;
        private IMapRenderer _mapRenderer;
        private EntityComponentSystem _entityComponentSystem;
        private EntityFactory _entityFactory;

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
            base.LoadContent();

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            _tiledMap = Content.Load<TiledMap>("level-1");
            _mapRenderer = new FullMapRenderer(GraphicsDevice, new MapRendererConfig {DrawObjectLayers = false});
            _mapRenderer.SwapMap(_tiledMap);

            _entityComponentSystem = new EntityComponentSystem();
            _entityFactory = new EntityFactory(_entityComponentSystem, Content);

            var service = new TiledObjectToEntityService(_entityFactory);
            var spawnPoint = _tiledMap.GetObjectGroup("entities").Objects.Single(i => i.Type == "Spawn").Position;

            _entityComponentSystem.RegisterSystem(new PlayerMovementSystem());
            _entityComponentSystem.RegisterSystem(new EnemyMovementSystem());
            _entityComponentSystem.RegisterSystem(new CharacterStateSystem(_entityFactory, spawnPoint));
            _entityComponentSystem.RegisterSystem(new BasicCollisionSystem(gravity: new Vector2(0, 1150)));
            _entityComponentSystem.RegisterSystem(new ParticleEmitterSystem());
            _entityComponentSystem.RegisterSystem(new AnimatedSpriteSystem());
            _entityComponentSystem.RegisterSystem(new SpriteBatchSystem(GraphicsDevice, _camera) { SamplerState = SamplerState.PointClamp });

            service.CreateEntities(_tiledMap.GetObjectGroup("entities").Objects);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _entityComponentSystem.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var viewMatrix = _camera.GetViewMatrix();

            GraphicsDevice.Clear(Color.Black);

            _mapRenderer.Draw(viewMatrix);
            _entityComponentSystem.Draw(gameTime);
        }
    }
}
