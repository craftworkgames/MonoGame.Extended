using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Gui;
using MonoGame.Extended.ViewportAdapters;
using Sandbox.Experiments;
using MonoGame.Extended.Gui.Controls;

// The Sandbox project is used for experiementing outside the normal demos.
// Any code found here should be considered experimental work in progress.
namespace Sandbox
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            Name = "Alistrasza";
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private ViewportAdapter _viewportAdapter;
        private GuiSystem _guiSystem;

        private GuiScreen _screen;
        private GuiSkin _skin;
        private int _screenUpdate = 2500;
        private int _i = 0;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            IsMouseVisible = false;

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, _graphicsDeviceManager.PreferredBackBufferWidth, _graphicsDeviceManager.PreferredBackBufferHeight);

            _skin = GuiSkin.FromFile(Content, @"Content/adventure-gui-skin.json", typeof(MyPanel));
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, _viewportAdapter.GetScaleMatrix);

            var viewModel = new ViewModel();

            _screen = GuiScreen.FromFile(Content, "Content/adventure-gui-screen.json", typeof(MyPanel));

            var control = _screen.FindControl<GuiListBox>("Listbox");
            control.Items.Add(new { Name = "Hello World" });
            control.Items.Add(new { Name = "Hello World" });
            control.Items.Add(new { Name = "Hello World" });
            control.Items.Add(new { Name = "Hello World" });

            _guiSystem = new GuiSystem(_viewportAdapter, guiRenderer)
            {
                Screens =
                {
                    _screen
                    // new BindingScreen(_skin, viewModel) 
                }
            };
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            //_screenUpdate -= gameTime.ElapsedGameTime.Milliseconds;
            //if (_screenUpdate <= 0)
            //{
            //    _screenUpdate += 2500;

            //    var control = _screen.FindControl<GuiListBox>("Listbox");
            //    control.Items.Add(new { Name = $"Hello World: {_i++}" });
            //}

            _guiSystem.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _guiSystem.Draw(gameTime);
        }
    }
}
