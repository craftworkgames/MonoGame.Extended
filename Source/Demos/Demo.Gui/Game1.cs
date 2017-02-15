using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Serialization;
using MonoGame.Extended.ViewportAdapters;
using Newtonsoft.Json;

namespace Demo.Gui
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private Camera2D _camera;
        private GuiSystem _guiSystem;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            var titleScreen = LoadScreen(@"Content/title-screen.json");
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, titleScreen.Skin.DefaultFont, _camera.GetViewMatrix);
            _guiSystem = new GuiSystem(viewportAdapter, guiRenderer) { Screen = titleScreen };

            var panel = titleScreen.FindControl<GuiPanel>("MainPanel");

            var closeButton = titleScreen.FindControl<GuiButton>("CloseButton");
            closeButton.Clicked += (sender, args) => { panel.IsVisible = false; };

            var quitButton = titleScreen.FindControl<GuiButton>("QuitButton");
            quitButton.Clicked += (sender, args) => Exit();

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

        protected override void UnloadContent()
        {
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
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _guiSystem.Draw(gameTime);
        }
    }
}
