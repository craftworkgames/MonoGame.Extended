using Demo.Screens.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.Screens
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            ScreenComponent screenComponent;
            Components.Add(screenComponent = new ScreenComponent(this));

            screenComponent.Register(new MainMenuScreen(Services, this));
            screenComponent.Register(new LoadGameScreen(Services));
            screenComponent.Register(new OptionsScreen(Services));
            screenComponent.Register(new AudioOptionsScreen(Services));
            screenComponent.Register(new VideoOptionsScreen(Services));
            screenComponent.Register(new KeyboardOptionsScreen(Services));
            screenComponent.Register(new MouseOptionsScreen(Services));
        }

        protected override void LoadContent()
        {
            //var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}