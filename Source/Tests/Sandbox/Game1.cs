using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.ViewportAdapters;

// The Sandbox project is used for experiementing outside the normal demos.
// Any code found here should be considered experimental work in progress.
namespace Sandbox
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphicsDeviceManager;
        private ViewportAdapter _viewportAdapter;
        private Camera2D _camera;
        private GuiSystem _guiSystem;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            IsMouseVisible = false;

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(_viewportAdapter);

            var skin = GuiSkin.FromFile(Content, @"Content/adventure-gui-skin.json");
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, _camera.GetViewMatrix);

            _guiSystem = new GuiSystem(_viewportAdapter, guiRenderer)
            {
                Screen = new GuiScreen(skin)
                {
                    Controls =
                    {
                        new GuiLabel { Text = "Hello World" }
                    }
                }
            };

        }
        
        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _guiSystem.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _guiSystem.Draw(gameTime);
        }
    }
}
