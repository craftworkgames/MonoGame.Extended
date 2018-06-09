using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Sandbox
{
    public class MyRenderSystem : DrawSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private ComponentMapper<Transform2> _transformMapper;

        public MyRenderSystem(GraphicsDevice graphicsDevice)
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Initialize(ComponentManager componentManager)
        {
            _transformMapper = componentManager.GetMapper<Transform2>();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            foreach (var entity in World.EntityManager.Entities)
            {
                var transform = _transformMapper.GetComponent(entity);

                _spriteBatch.DrawRectangle(transform.Position, new Size2(100, 100), Color.Black);
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

            var entity = _world.CreateEntity();
            entity.Attach(new Transform2(new Vector2(400, 240)));

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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _world.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
