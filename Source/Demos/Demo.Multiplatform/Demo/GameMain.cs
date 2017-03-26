using Demo.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.ViewportAdapters;

namespace Demo
{
    public class PlatformConfig
    {
        public bool IsFullScreen { get; set; } = true;
    }

    public class GameMain : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private GuiSystem _guiSystem;

        public GameMain(PlatformConfig config)
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                IsFullScreen = config.IsFullScreen,
                SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            var camera = new Camera2D(viewportAdapter);

            var skin = LoadSkin(@"Raw/adventure-gui-skin.json");
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, camera.GetViewMatrix);

            _guiSystem = new GuiSystem(viewportAdapter, guiRenderer)
            {
                Screen = new DemoScreen(skin)
            };
        }

        private GuiSkin LoadSkin(string assetName)
        {
            using (var stream = TitleContainer.OpenStream(assetName))
            {
                return GuiSkin.FromStream(stream, Content);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _guiSystem.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _guiSystem.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
