using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Sandbox
{
    public class Raindrop
    {
        public Vector2 Velocity;
    }

    public class RainfallSystem : UpdateSystem
    {
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Raindrop> _raindropMapper;

        public RainfallSystem()
            : base(Aspect.All(typeof(Transform2), typeof(Raindrop)))
        {
        }

        public override void Initialize(ComponentManager componentManager)
        {
            _transformMapper = componentManager.GetMapper<Transform2>();
            _raindropMapper = componentManager.GetMapper<Raindrop>();
        }

        public override void Update(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();

            foreach (var entity in GetEntities())
            {
                var transform = _transformMapper.Get(entity);
                var raindrop = _raindropMapper.Get(entity);

                raindrop.Velocity += new Vector2(0, 10) * elapsedSeconds;
                transform.Position += raindrop.Velocity * elapsedSeconds;

                if (transform.Position.Y >= 480)
                    transform.Position = new Vector2(transform.Position.X, 0);
            }
        }
    }

    public class MyRenderSystem : DrawSystem
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private ComponentMapper<Transform2> _transformMapper;

        public MyRenderSystem(GraphicsDevice graphicsDevice)
            : base(Aspect.All(typeof(Transform2)))
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Initialize(ComponentManager componentManager)
        {
            _transformMapper = componentManager.GetMapper<Transform2>();
        }

        public override void Draw(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            foreach (var entity in GetEntities())
            {
                var transform = _transformMapper.Get(entity);

                _spriteBatch.FillRectangle(transform.Position, new Size2(3, 3), Color.LightBlue);
            }

            _spriteBatch.End();
        }

    }

    public class MainGame : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private EntityWorld _world;

        public MainGame()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _world = new EntityWorld();
            _world.RegisterSystem(new MyRenderSystem(GraphicsDevice));
            _world.RegisterSystem(new RainfallSystem());

            var random = new FastRandom();

            for(var i = 0; i < 1000; i++)
            {
                var entity = _world.CreateEntity();
                entity.Attach(new Transform2(random.NextSingle(0, 800), random.NextSingle(-480, 480)));
                entity.Attach(new Raindrop { Velocity = new Vector2(random.Next(-3, 3), random.Next(-100)) });
            }
        }

        protected override void UnloadContent()
        {
            _spriteBatch.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _world.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _world.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
