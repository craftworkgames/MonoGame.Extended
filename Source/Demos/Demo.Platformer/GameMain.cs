using Demo.Platformer.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;

namespace Demo.Platformer
{
    public class GameMain : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        
        public GameMain()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            var screens = new [] { new GameScreen(Services, GraphicsDevice, Window) };

            Components.Add(new ScreenComponent(this, screens));

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        //protected override void Draw(GameTime gameTime)
        //{
        //    GraphicsDevice.Clear(Color.Black);

        //    base.Draw(gameTime);
        //}
    }
}