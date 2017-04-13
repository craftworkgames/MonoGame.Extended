using System.Collections.Generic;
using Demo.Features.Demos;
using Demo.Features.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.Features
{
    public class PlatformConfig
    {
        public bool IsFullScreen { get; set; } = true;
    }

    public class GameMain : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly FramesPerSecondCounter _fpsCounter = new FramesPerSecondCounter();
        private readonly List<DemoBase> _demos;

        private string _demoTitle;
        private GuiSystem _guiSystem;
        private int _demoIndex = 0;

        public ViewportAdapter ViewportAdapter { get; private set; }

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

            _demos = new List<DemoBase>
            {
                new GuiLayoutDemo(this),
                new GuiDemo(this),
                //new ScreensDemo(this),
                new ViewportAdaptersDemo(this),
                new TiledMapsDemo(this),
                new AnimationsDemo(this),
                new SpritesDemo(this),
                new BatchingDemo(this),
                new TweeningDemo(this),
                new InputListenersDemo(this),
                new SceneGraphsDemo(this),
                new ParticlesDemo(this),
                new CameraDemo(this),
                new BitmapFontsDemo(this)
            };
            //_demos.Sort();
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            // TODO: Allow switching to full-screen mode from the UI
            //if (_isFullScreen)
            //{
            //    _graphicsDeviceManager.IsFullScreen = true;
            //    _graphicsDeviceManager.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            //    _graphicsDeviceManager.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            //    _graphicsDeviceManager.ApplyChanges();
            //}
        }

        protected override void LoadContent()
        {
            ViewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);

            var skin = LoadSkin(@"Raw/adventure-gui-skin.json");
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, ViewportAdapter.GetScaleMatrix);

            _guiSystem = new GuiSystem(ViewportAdapter, guiRenderer)
            {
                Screen = new DemoScreen(skin, NextDemo)
            };

            LoadDemo(_demoIndex);
        }

        private void LoadDemo(int index)
        {
            _demos[index].Load();
            _demoTitle = _demos[index].GetType().Name;
        }

        private void NextDemo()
        {
            _demos[_demoIndex].Unload();

            if (_demoIndex == _demos.Count - 1)
                _demoIndex = 0;
            else
                _demoIndex++;

            LoadDemo(_demoIndex);
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

            _fpsCounter.Update(gameTime);
            _guiSystem.Update(gameTime);
            _demos[_demoIndex].OnUpdate(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _fpsCounter.Draw(gameTime);
            Window.Title = $"{_demoTitle} - {_fpsCounter.FramesPerSecond}";

            base.Draw(gameTime);

            _demos[_demoIndex].OnDraw(gameTime);
            _guiSystem.Draw(gameTime);
        }
    }
}
