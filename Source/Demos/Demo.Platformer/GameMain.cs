using System.Linq;
using Demo.Platformer.Entities;
using Demo.Platformer.Entities.Components;
using Demo.Platformer.Entities.Systems;
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

            _entityComponentSystem = new EntityComponentSystem();
            _entityComponentSystem.RegisterSystem(new PlayerMovementSystem());
            _entityComponentSystem.RegisterSystem(new BasicCollisionSystem(gravity: new Vector2(0, 1150)));
            _entityComponentSystem.RegisterSystem(new SpriteBatchComponentSystem(GraphicsDevice, _camera) { SamplerState = SamplerState.PointClamp });

            _tiledMap = Content.Load<TiledMap>("level-1");

            var entitiesLayer = _tiledMap.GetObjectGroup("entities");
            var spawn = entitiesLayer.Objects.FirstOrDefault(i => i.Type == "Spawn");

            foreach (var solidObject in entitiesLayer.Objects.Where(i => i.Type == "Solid"))
            {
                var entity = _entityComponentSystem.CreateEntity(position: new Vector2(solidObject.X, solidObject.Y));
                entity.AttachComponent(new BasicCollisionBody(new SizeF(solidObject.Width, solidObject.Height), Vector2.Zero) { IsStatic = true });
            }

            _entityFactory = new EntityFactory(_entityComponentSystem, Content);
            
            if (spawn != null)
                _entityFactory.CreatePlayer(new Vector2(spawn.X, spawn.Y));

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