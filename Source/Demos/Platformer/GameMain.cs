using Autofac;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Platformer.Collisions;
using Platformer.Systems;

namespace Platformer
{
    public class GameMain : GameBase
    {
        private World _world;
        private Body _player;
        private TiledMap _map;
        private TiledMapRenderer _renderer;
        private Entity _playerEntity;

        public GameMain()
        {
        }

        protected override void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterInstance(new SpriteBatch(GraphicsDevice));
            builder.RegisterType<RenderSystem>();
            builder.RegisterType<MovementSystem>();
            builder.RegisterType<PlayerSystem>();
        }

        protected override void LoadContent()
        {
            var dudeTexture = Content.Load<Texture2D>("hero");
            var dudeAtlas = TextureAtlas.Create("dudeAtlas", dudeTexture, 16, 16);
            var dude = EntityComponentSystem.EntityManager.CreateEntity("dude");
            dude.Attach(new Sprite(dudeAtlas[0]));
            dude.Attach(new Transform2(new Vector2(100, 100), 0, Vector2.One * 4));

            _playerEntity = dude;

            _map = Content.Load<TiledMap>("test-map");
            _renderer = new TiledMapRenderer(GraphicsDevice, _map);
            
            //_spriteBatch = new SpriteBatch(GraphicsDevice);
            _world = new World(new Vector2(0, 3600));

            var floor = new Body
            {
                Position = new Vector2(400, 480-16),
                Size = new Vector2(800, 16),
                BodyType = BodyType.Static
            };
            _world.Bodies.Add(floor);

            _player = new Body
            {
                Position = new Vector2(400, 240),
                Size = new Vector2(32, 64),
                BodyType = BodyType.Dynamic
            };
            _world.Bodies.Add(_player);

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
                            var block = new Body
                            {
                                Position = new Vector2(x * tileWidth + tileWidth * 0.5f, y * tileHeight + tileHeight * 0.5f),
                                Size = new Vector2(tileWidth, tileHeight),
                                BodyType = BodyType.Static
                            };
                            _world.Bodies.Add(block);

                        }
                    }
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var currentKeyboardState = KeyboardExtended.GetState();
            //var mouseState = Mouse.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _renderer.Update(gameTime);

            _world.Update(deltaTime);
            _world.OnCollision = OnCollision;

            //_player.Velocity = new Vector2(0, _player.Velocity.Y);

            if (KeyboardInputService.IsKeyDown(Keys.Right))
                _player.Velocity.X += 1000 * deltaTime;
            else if (KeyboardInputService.IsKeyDown(Keys.Left))
                _player.Velocity.X -= 1000 * deltaTime;
            else
                _player.Velocity.X = _player.Velocity.Y * 0.99f * deltaTime;

            if (currentKeyboardState.WasKeyJustUp(Keys.Up))
                _player.Velocity = new Vector2(_player.Velocity.X, -900);

            //if (mouseState.LeftButton == ButtonState.Pressed)
            //{
            //    var mousePosition = new Vector2(mouseState.X, mouseState.Y);
            //    _mouseBox = !_mouseBox.HasValue ? new AABB(mousePosition, mousePosition) : new AABB(_mouseBox.Value.Min, mousePosition);
            //}
            //else
            //{
            //    if (_mouseBox.HasValue)
            //    {
            //        var box = _mouseBox.Value;
            //        _world.Bodies.Add(new Body
            //        {
            //            BodyType = BodyType.Static,
            //            Position = box.Min + box.Center,
            //            Size = new Vector2(box.Width, box.Height)
            //        });
            //        _mouseBox = null;
            //    }
            //}

            _playerEntity.Get<Transform2>().Position = _player.Position;

            base.Update(gameTime);
        }

        //private AABB? _mouseBox;

        private static void OnCollision(Manifold manifold)
        {
            var body = manifold.BodyB.BodyType == BodyType.Dynamic ? manifold.BodyB : manifold.BodyA;

            body.Position -= manifold.Normal * manifold.Penetration;

            if (manifold.Normal.Y < 0 || manifold.Normal.Y > 0)
                body.Velocity = new Vector2(body.Velocity.X, 0);

            //if (manifold.Normal.X < 0 || manifold.Normal.X > 0)
            //    body.Velocity = new Vector2(0, body.Velocity.Y);
        }

        protected override void Draw(GameTime gameTime)
        {
            _renderer.Draw();

            //_spriteBatch.Begin();

            //foreach (var body in _world.Bodies.Where(b => b.BodyType == BodyType.Dynamic))
            //{
            //    var box = body.BoundingBox;
            //    _spriteBatch.FillRectangle(new RectangleF(box.Min.X, box.Min.Y, box.Width, box.Height), body.BodyType == BodyType.Static ? Color.WhiteSmoke : Color.Green);
            //}

            //if (_mouseBox.HasValue)
            //{
            //    var box = _mouseBox.Value;
            //    _spriteBatch.DrawRectangle(new RectangleF(box.Min.X, box.Min.Y, box.Width, box.Height), Color.Magenta);
            //}

            //_spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
