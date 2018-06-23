using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using Sandbox.Components;
using Sandbox.Systems;

namespace Sandbox
{
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
            _world.RegisterSystem(new RenderSystem(GraphicsDevice));
            _world.RegisterSystem(new RainfallSystem());

            var random = new FastRandom();

            for(var i = 0; i < 2000; i++)
            {
                var entity = _world.CreateEntity();
                entity.Attach(new Transform2(random.NextSingle(0, 800), random.NextSingle(-480, 480)));
                entity.Attach(new Raindrop { Velocity = new Vector2(random.Next(-3, 3), random.Next(-10, 10)) });
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
