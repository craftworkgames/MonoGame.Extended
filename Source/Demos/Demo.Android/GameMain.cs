using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Serialization;
using MonoGame.Extended.ViewportAdapters;
using Newtonsoft.Json;

namespace Demo.Android
{
    public class GameMain : Game
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private Camera2D _camera;
        private GuiSystem _guiSystem;
        private GuiProgressBar _progressBar;
        private float _progressDelta = 0.2f;

        public GameMain()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                IsFullScreen = true,
                SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight
            };
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            var titleScreen = LoadScreen(@"title-screen.json");
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, titleScreen.Skin.DefaultFont, _camera.GetViewMatrix);
            _guiSystem = new GuiSystem(viewportAdapter, guiRenderer) { Screen = titleScreen };

            var panel = titleScreen.FindControl<GuiPanel>("MainPanel");

            var closeButton = titleScreen.FindControl<GuiButton>("CloseButton");
            closeButton.Clicked += (sender, args) => { panel.IsVisible = false; };

            var quitButton = titleScreen.FindControl<GuiButton>("QuitButton");
            quitButton.Clicked += (sender, args) => Exit();

            _progressBar = titleScreen.FindControl<GuiProgressBar>("ProgressBar");
        }

        private GuiScreen LoadScreen(string name)
        {
            var skinService = new GuiSkinService();
            var serializer = new GuiJsonSerializer(Content)
            {
                Converters =
                {
                    new GuiSkinJsonConverter(Content, skinService),
                    new GuiControlJsonConverter(skinService)
                }
            };

            using (var stream = TitleContainer.OpenStream(name))
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var screen = serializer.Deserialize<GuiScreen>(jsonReader);
                return screen;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _progressBar.Progress += deltaTime * _progressDelta;

            if (_progressBar.Progress >= 1.1f)
                _progressDelta = -_progressDelta;
            else if (_progressBar.Progress <= -0.1f)
                _progressDelta = -_progressDelta;

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