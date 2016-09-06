using System.Linq;
using Demo.Platformer.Entities;
using Demo.Platformer.Entities.Systems;
using Demo.Platformer.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.Platformer
{
    public class GameMain : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private Camera2D _camera;
        private TiledMap _tiledMap;
        private EntityComponentSystem _entityComponentSystem;
        private EntityFactory _entityFactory;

        public GameMain()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            _tiledMap = Content.Load<TiledMap>("level-1");

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


            //var viewport = GraphicsDevice.Viewport;
            //_alphaTestEffect = new AlphaTestEffect(GraphicsDevice)
            //{
            //    Projection =
            //        Matrix.CreateTranslation(-0.5f, -0.5f, 0)*
            //        Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1),
            //    VertexColorEnabled = true,
            //    ReferenceAlpha = 128
            //};
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _entityComponentSystem.Update(gameTime);

            base.Update(gameTime);
        }

        //private AlphaTestEffect _alphaTestEffect;

        protected override void Draw(GameTime gameTime)
        {
            var viewMatrix = _camera.GetViewMatrix();

            GraphicsDevice.Clear(Color.Black);

            //_spriteBatch.Begin(
            //    sortMode: SpriteSortMode.FrontToBack, 
            //    samplerState: SamplerState.PointClamp, 
            //    depthStencilState: DepthStencilState.Default,
            //    blendState: BlendState.AlphaBlend,
            //    effect: _alphaTestEffect,
            //    transformMatrix: viewMatrix);

            //_spriteBatch.Draw(_logo, position: new Vector2(305, 160), color: Color.White, layerDepth: 0.5f);
            //_spriteBatch.Draw(_logo, position: new Vector2(375, 210), color: Color.Green, layerDepth: 0.25f);

            //_spriteBatch.End();
            
            _tiledMap.Draw(viewMatrix);
            _entityComponentSystem.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}