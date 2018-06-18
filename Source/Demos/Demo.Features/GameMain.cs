using System.Collections.Generic;
using System.Linq;
using Demo.Features.Demos;
using Demo.Features.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
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
        private readonly Dictionary<string, DemoBase> _demos;
        private DemoBase _currentDemo;

        private GuiSystem _guiSystem;

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

            _demos = new DemoBase[]
            {
                //new GuiLayoutDemo(this),
                //new GuiDemo(this),
                //new ScreensDemo(this),
                new ViewportAdaptersDemo(this),
                new CollisionDemo(this),
                new TiledMapsDemo(this),
                new AnimationsDemo(this),
                new SpritesDemo(this),
                new BatchingDemo(this),
                //new TweeningDemo(this),
                new InputListenersDemo(this),
                new SceneGraphsDemo(this),
                new ParticlesDemo(this),
                new CameraDemo(this),
                new BitmapFontsDemo(this)
            }.ToDictionary(d => d.Name);
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var demo in _demos.Values)
                demo.Dispose();

            base.Dispose(disposing);
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

            //var skin = GuiSkin.FromFile(Content, @"Raw/adventure-gui-skin.json");
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, ViewportAdapter.GetScaleMatrix);

            var font = Content.Load<BitmapFont>("small-font");
            BitmapFont.UseKernings = false;
            Skin.CreateDefault(font);
            _selectDemoScreen = new SelectDemoScreen(_demos, LoadDemo, Exit);

            _guiSystem = new GuiSystem(ViewportAdapter, guiRenderer)
            {
                ActiveScreen = _selectDemoScreen,
            };
        }

        private void LoadDemo(string name)
        {
            IsMouseVisible = true;
            _currentDemo?.Unload();
            _currentDemo?.Dispose();
            _currentDemo = _demos[name];
            _currentDemo.Load();
        }

        private KeyboardState _previousKeyboardState;
        private SelectDemoScreen _selectDemoScreen;

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape) && _previousKeyboardState.IsKeyUp(Keys.Escape))
                Back();

            _fpsCounter.Update(gameTime);
            _guiSystem.Update(gameTime);
            _currentDemo?.OnUpdate(gameTime);

            _previousKeyboardState = keyboardState;
            base.Update(gameTime);
        }

        public void Back()
        {
            if (_selectDemoScreen.IsVisible)
                Exit();

            IsMouseVisible = false;
            _currentDemo = null;
            _selectDemoScreen.IsVisible = true;
            _guiSystem.ActiveScreen = _selectDemoScreen;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _fpsCounter.Draw(gameTime);
            Window.Title = $"{_currentDemo?.Name} {_fpsCounter.FramesPerSecond}";

            base.Draw(gameTime);

            _currentDemo?.OnDraw(gameTime);

            _guiSystem.Draw(gameTime);
        }
    }
}
