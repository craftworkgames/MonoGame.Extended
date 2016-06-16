using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;

namespace Demo.Collisions
{
    public class Game1 : Game
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        private CollisionSimulation _collisionSimulation;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.Position = Point.Zero;
        }

        protected override void Initialize()
        {
            _collisionSimulation = new CollisionSimulation();

            var body = _collisionSimulation.CreateBody(null);
            var fixture = _collisionSimulation.CreateFixture(body, CollisionShape2D.Create(new[]
            {
                new Vector2(0, 0),
                new Vector2(50, 0),
                new Vector2(0, 100)
            }));

            fixture.ToString();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var graphicsDevice = GraphicsDevice;
            var viewport = graphicsDevice.Viewport;

            var collisionDebugEffect = new CollisionDebugEffect(Content.Load<Effect>("CollisionDebugEffect"))
            {
                World = Matrix.CreateScale(new Vector3(1, 1, 1)),
                View = Matrix.Identity,
                Projection = Matrix.CreateOrthographicOffCenter(viewport.Width * -0.5f, viewport.Width * 0.5f, viewport.Height * -0.5f, viewport.Height * 0.5f, 0, 1)
            };

            _collisionSimulation.DebugDrawer = new CollisionDebugDrawer(graphicsDevice, collisionDebugEffect);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            _collisionSimulation.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _collisionSimulation.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
