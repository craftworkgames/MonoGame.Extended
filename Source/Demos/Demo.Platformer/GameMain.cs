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
using MonoGame.Extended.Maps.Tiled.Services;
using MonoGame.Extended.Maps.Tiled.Systems;
using MonoGame.Extended.Maps.Tiled.Components;
using System.Text;
using MonoGame.Extended.BitmapFonts;

namespace Demo.Platformer
{
    public class GameMain : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private BitmapFont _bitmapFont;
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
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _bitmapFont = Content.Load<BitmapFont>("montserrat-32");

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            _tiledMap = Content.Load<TiledMap>("level-1");

            _entityComponentSystem = new EntityComponentSystem();
            _entityFactory = new EntityFactory(_entityComponentSystem, Content);

            var service = new TiledObjectToEntityService(_entityFactory);
            var spawnPoint = _tiledMap.GetObjectGroup("entities").Objects.Single(i => i.Type == "Spawn").Position;

            _entityComponentSystem.RegisterSystem(new LayerDepthSystem());
            _entityComponentSystem.RegisterSystem(new PlayerMovementSystem());
            _entityComponentSystem.RegisterSystem(new EnemyMovementSystem());
            _entityComponentSystem.RegisterSystem(new CharacterStateSystem(_entityFactory, spawnPoint));
            _entityComponentSystem.RegisterSystem(new BasicCollisionSystem(gravity: new Vector2(0, 1150)));
            _entityComponentSystem.RegisterSystem(new ParticleEmitterSystem());
            _entityComponentSystem.RegisterSystem(new AnimatedSpriteSystem());
            _entityComponentSystem.RegisterSystem(new SpriteBatchSystem(GraphicsDevice, _camera) { SamplerState = SamplerState.PointClamp, UseAlphaTest = true });
            
            service.CreateEntities(_tiledMap.GetObjectGroup("entities").Objects);

            var tileService = new TiledTileLayerToEntityService(_entityComponentSystem, _tiledMap);
            tileService.ConvertLayer("decorations");
            tileService.ConvertLayer("decorations2");
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

            if (keyboardState.IsKeyDown(Keys.T))
            {
                var player = _entityComponentSystem.GetEntity(Entities.Entities.Player);
                if (player.GetComponent<DepthComponent>().Level != 10)
                    player.GetComponent<DepthComponent>().Level++;
            }

            if (keyboardState.IsKeyDown(Keys.R))
            {
                var player = _entityComponentSystem.GetEntity(Entities.Entities.Player);
                if (player.GetComponent<DepthComponent>().Level != 0)
                    player.GetComponent<DepthComponent>().Level--;
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var viewMatrix = _camera.GetViewMatrix();

            GraphicsDevice.Clear(Color.Black);
            
            _tiledMap.Draw(viewMatrix);
            _entityComponentSystem.Draw(gameTime);

            #region Draw Player Depth Level

            var player = _entityComponentSystem.GetEntity(Entities.Entities.Player);
            var depthLevel = player.GetComponent<DepthComponent>().Level;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Player Depth Level: {depthLevel:0}");

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend);
            _spriteBatch.DrawString(_bitmapFont, stringBuilder.ToString(), new Vector2(5, 5), Color.DarkBlue);
            _spriteBatch.End();

            #endregion

            base.Draw(gameTime);
        }
    }
}