using System;
using Autofac;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Platformer.Systems;

namespace Platformer
{
    public class GameMain : GameBase
    {
        private TiledMap _map;
        private TiledMapRenderer _renderer;
        private EntityFactory _entityFactory;
        private OrthographicCamera _camera;
        private Entity _playerEntity;

        public GameMain()
        {
        }

        protected override void RegisterDependencies(ContainerBuilder builder)
        {
            _camera = new OrthographicCamera(GraphicsDevice);

            builder.RegisterInstance(new SpriteBatch(GraphicsDevice));
            builder.RegisterInstance(_camera);
            builder.RegisterType<RenderSystem>();
            builder.RegisterType<PlayerSystem>();
            builder.RegisterType<WorldSystem>();
        }

        protected override void LoadContent()
        {
            _entityFactory = new EntityFactory(EntityComponentSystem.EntityManager, Content);
            _playerEntity = _entityFactory.CreatePlayer(new Vector2(100, 240));

            // TOOD: Load maps and collision data more nicely :)
            _map = Content.Load<TiledMap>("test-map");
            _renderer = new TiledMapRenderer(GraphicsDevice, _map);

            foreach (var tileLayer in _map.TileLayers)
            {
                for (var x = 0; x < tileLayer.Width; x++)
                {
                    for (var y = 0; y < tileLayer.Height; y++)
                    {
                        var tile = tileLayer.GetTile((ushort)x, (ushort)y);

                        if (tile.GlobalIdentifier == 1)
                        {
                            var tileWidth = _map.TileWidth;
                            var tileHeight = _map.TileHeight;
                            _entityFactory.CreateTile(x, y, tileWidth, tileHeight);
                        }
                    }
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Using global shared input state is really bad!

            //var keyboardState = KeyboardExtended.GetState();

            //if (keyboardState.IsKeyDown(Keys.Escape))
            //    Exit();

            _renderer.Update(gameTime);
            //_camera.LookAt(_playerEntity.Get<Transform2>().Position);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _renderer.Draw(_camera.GetViewMatrix());

            base.Draw(gameTime);
        }
    }
}
