using Autofac;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Platformer.Collisions;
using Platformer.Systems;

namespace Platformer
{
    public class GameMain : GameBase
    {
        //private EntityFactory _entityFactory;

        private SpriteBatch _spriteBatch;
        private World _world;
        private Body _player;

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
            //_entityFactory = new EntityFactory(EntityComponentSystem.EntityManager);
            //_entityFactory.CreatePlayer(new Vector2(400, 240));

            //for (var x = 0; x < 25; x++)
            //{
            //    for (var y = 0; y < 15; y++)
            //    {
            //        if(x == 0 || y == 0 || x == 24 || y == 14)
            //            _entityFactory.CreateTile(x, y);

            //    }
            //}

            _spriteBatch = new SpriteBatch(GraphicsDevice);
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
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var currentKeyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _world.Update(deltaTime);
            _world.OnCollision = OnCollision;

            _player.Velocity = new Vector2(0, _player.Velocity.Y);

            if (KeyboardInputService.IsKeyDown(Keys.Right))
                _player.Velocity = new Vector2(350, _player.Velocity.Y);

            if (KeyboardInputService.IsKeyDown(Keys.Left))
                _player.Velocity = new Vector2(-350, _player.Velocity.Y);

            if (KeyboardInputService.IsKeyDown(Keys.Up))
                _player.Velocity = new Vector2(_player.Velocity.X, -900);

            if (KeyboardInputService.IsKeyDown(Keys.Down))
                _player.Velocity += new Vector2(0, 50) * deltaTime;

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                var mousePosition = new Vector2(mouseState.X, mouseState.Y);
                _mouseBox = !_mouseBox.HasValue ? new AABB(mousePosition, mousePosition) : new AABB(_mouseBox.Value.Min, mousePosition);
            }
            else
            {
                if (_mouseBox.HasValue)
                {
                    var box = _mouseBox.Value;
                    _world.Bodies.Add(new Body
                    {
                        BodyType = BodyType.Static,
                        Position = box.Min + box.Center,
                        Size = new Vector2(box.Width, box.Height)
                    });
                    _mouseBox = null;
                }
            }

            base.Update(gameTime);
        }

        private AABB? _mouseBox;

        private static void OnCollision(Manifold manifold)
        {
            var player = manifold.BodyB.BodyType == BodyType.Dynamic ? manifold.BodyB : manifold.BodyA;

            player.Position -= manifold.Normal * manifold.Penetration;

            if(manifold.Normal.Y < 0 || manifold.Normal.Y > 0)
                player.Velocity = new Vector2(player.Velocity.X, 0);

            if (manifold.Normal.X < 0 || manifold.Normal.X > 0)
                player.Velocity = new Vector2(0, player.Velocity.Y);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();

            foreach (var body in _world.Bodies)
            {
                var box = body.BoundingBox;
                _spriteBatch.FillRectangle(new RectangleF(box.Min.X, box.Min.Y, box.Width, box.Height), body.BodyType == BodyType.Static ? Color.WhiteSmoke : Color.Green);
            }

            if (_mouseBox.HasValue)
            {
                var box = _mouseBox.Value;
                _spriteBatch.DrawRectangle(new RectangleF(box.Min.X, box.Min.Y, box.Width, box.Height), Color.Magenta);
            }

            _spriteBatch.End();
        }
    }
}
