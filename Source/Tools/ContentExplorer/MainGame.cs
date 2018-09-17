using System;
using System.IO;
using ContentExplorer.Features;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.Gui.Markup;
using MonoGame.Extended.ViewportAdapters;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace ContentExplorer
{
    public interface IPlatformSpecific
    {
        string OpenFile();
    }

    public class MainGame : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly IPlatformSpecific _platform;
        private GuiSystem _guiSystem;
        private Texture2D _texture;
        private SpriteBatch _spriteBatch;
        private MainWindowViewModel _viewModel = new MainWindowViewModel();

        public MainGame(IPlatformSpecific platform)
        {
            _platform = platform;
            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 800
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += WindowOnClientSizeChanged;
        }

        private void WindowOnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            _guiSystem.ClientSizeChanged();
        }

        protected override void LoadContent()
        {
            var viewportAdapter = new DefaultViewportAdapter(GraphicsDevice);
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, () => Matrix.Identity);
            var font = Content.Load<BitmapFont>("Sensation");
            BitmapFont.UseKernings = false;
            Skin.CreateDefault(font);

            var parser = new MarkupParser();

            var demoScreen = new Screen
            {
                Content = parser.Parse("Features/MainWindow.mgeml", _viewModel)
            };

            _guiSystem = new GuiSystem(viewportAdapter, guiRenderer) { ActiveScreen = demoScreen };

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            demoScreen.FindControl<Button>("OpenButton")
                .Clicked += (sender, args) =>
            {
                var filePath = _platform.OpenFile();

                if (filePath != null)
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        _texture?.Dispose();
                        _texture = Texture2D.FromStream(GraphicsDevice, stream);
                    }
                }
            };

            demoScreen.FindControl<Button>("QuitButton")
                .Clicked += (sender, args) => Exit();

            var comboBox = demoScreen.FindControl<ComboBox>("ComboBox");
            comboBox.Items.AddRange(new []{ "Hello World!", "This is cool" });
            comboBox.SelectedIndex = 0;
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
            GraphicsDevice.Clear(Color.Black);
            
            if (_texture != null)
            {
                var position = (GraphicsDevice.Viewport.Bounds.Center - _texture.Bounds.Center).ToVector2();
                _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                _spriteBatch.Draw(_texture, position, Color.White);
                _spriteBatch.End();
            }

            _guiSystem.Draw(gameTime);
        }
    }
}
