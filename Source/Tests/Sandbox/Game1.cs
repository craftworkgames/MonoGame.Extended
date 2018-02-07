using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Gui;
using MonoGame.Extended.ViewportAdapters;
using Sandbox.Experiments;
using MonoGame.Extended.Gui.Controls;
using System;

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

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
        }

        private void LoadGuiButtonScreen()
        {
            _screen = GuiScreen.FromFile(Content, "Content/adventure-gui-button-screen.json", typeof(MyPanel));

            var disable = _screen.FindControl<GuiButton>("Disable");
            var disabled = false;
            disable.Clicked += (sender, e) =>
            {
                disabled = !disabled;
                foreach(var selector in new List<string>() { "Button1", "Button2", "Button3", "Button4" })
                {
                    var item = _screen.FindControl<GuiButton>(selector);
                    if (item != null)
                    {
                        item.IsEnabled = !disabled;
                    }
                }
            };
            Window.ClientSizeChanged += OnClientSizeChanged;
        }

        protected override void LoadContent()
        {
            IsMouseVisible = false;

            // _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, _graphicsDeviceManager.PreferredBackBufferWidth, _graphicsDeviceManager.PreferredBackBufferHeight);
            _viewportAdapter = new WindowViewportAdapter(Window, GraphicsDevice);

            _skin = GuiSkin.FromFile(Content, @"Content/adventure-gui-skin.json", typeof(MyPanel));
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, _viewportAdapter.GetScaleMatrix);

            var viewModel = new ViewModel();

            LoadGuiButtonScreen();

            //_screen = GuiScreen.FromFile(Content, "Content/test-addition-screen.json", typeof(MyPanel));
            //Window.ClientSizeChanged += OnClientSizeChanged;

            //_screen = GuiScreen.FromFile(Content, "Content/adventure-gui-screen.json", typeof(MyPanel));

            //var listBox = _screen.FindControl<GuiListBox>("Listbox");
            //listBox.Items.Add(new { Name = "Hello World 1" });
            //listBox.Items.Add(new { Name = "Hello World 2" });
            //listBox.Items.Add(new { Name = "Hello World 3" });
            //listBox.Items.Add(new { Name = "Hello World 4" });

            //var comboBox = _screen.FindControl<GuiComboBox>("ComboBox");
            //comboBox.Items.Add(new { Name = "Hello World 1" });
            //comboBox.Items.Add(new { Name = "Hello World 2" });
            //comboBox.Items.Add(new { Name = "Hello World 3" });
            //comboBox.Items.Add(new { Name = "Hello World 4" });

            //var submit = _screen.FindControl<GuiSubmit>("FormSubmit");
            //submit.SetBinding(nameof(GuiButton.Text), nameof(viewModel.Name));
            //submit.Clicked += OnFormSubmitClicked;
            //submit.Clicked += (s, e) => { viewModel.Name = viewModel.Name == "Change" ? "Alistrasza" : "Change"; };

            _screen.BindingContext = viewModel;

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

            _guiSystem.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _guiSystem.Draw(gameTime);
        }

        private void OnFormSubmitClicked(object sender, EventArgs e)
        {
            var listBox = _screen.FindControl<GuiListBox>("Listbox");
            var textBox = _screen.FindControl<GuiTextBox>("TextBox");
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                listBox.Items.Add(new { Name = textBox.Text });
            }
        }
        private void OnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            if (_guiSystem != null)
            {
                foreach (var screen in _guiSystem.Screens)
                {
                    screen.Layout(_guiSystem, _guiSystem.BoundingRectangle);
                }
            }
        }
    }
}
