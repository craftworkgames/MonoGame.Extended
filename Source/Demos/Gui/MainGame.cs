using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.ViewportAdapters;

namespace Gui
{
    public class MainGame : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private GuiSystem _guiSystem;

        public MainGame()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);

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

            var stackTest = new DemoViewModel("Stack Panels",
                    new StackPanel
                    {
                        Items =
                        {
                            new Button { Content = "Press Me", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top },
                            new Button { Content = "Press Me", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom  },
                            new Button { Content = "Press Me", HorizontalAlignment = HorizontalAlignment.Centre, VerticalAlignment = VerticalAlignment.Centre  },
                            new Button { Content = "Press Me", HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch },
                        }
                    });

            var dockTest = new DemoViewModel("Dock Panels",
                new DockPanel
                {
                    Items =
                    {
                        new Button { Content = "Dock.Top", AttachedProperties = { { DockPanel.DockProperty, Dock.Top } } },
                        new Button { Content = "Dock.Bottom", AttachedProperties = { { DockPanel.DockProperty, Dock.Bottom } } },
                        new Button { Content = "Dock.Left", AttachedProperties = { { DockPanel.DockProperty, Dock.Left } } },
                        new Button { Content = "Dock.Right", AttachedProperties = { { DockPanel.DockProperty, Dock.Right } } },
                        new Button { Content = "Fill" }
                    }
                });

            var controlTest = new DemoViewModel("Basic Controls",
                new StackPanel
                {
                    Margin = 5,
                    Orientation = Orientation.Vertical,
                    Items =
                    {
                        new Label("Buttons") { Margin = 5 },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing = 5,
                            Items =
                            {
                                new Button { Content = "Enabled" },
                                new Button { Content = "Disabled", IsEnabled = false },
                                new ToggleButton { Content = "ToggleButton" }
                            }
                        },

                        new Label("TextBox") { Margin = 5 },
                        new TextBox {Text = "TextBox" },

                        new Label("CheckBox") { Margin = 5 },
                        new CheckBox {Content = "Check me please!"},

                        new Label("ListBox") { Margin = 5 },
                        new ListBox {Items = {"ListBoxItem1", "ListBoxItem2", "ListBoxItem3"}, SelectedIndex = 0},

                        new Label("ProgressBar") { Margin = 5 },
                        new ProgressBar {Progress = 0.5f, Width = 100},

                        new Label("ComboBox") { Margin = 5 },
                        new ComboBox {Items = {"ComboBoxItemA", "ComboBoxItemB", "ComboBoxItemC"}, SelectedIndex = 0, HorizontalAlignment = HorizontalAlignment.Left}
                    }
                });

            var demoScreen = new Screen
            {
                Content = new DockPanel
                {
                    LastChildFill = true,
                    Items =
                    {
                        new ListBox
                        {
                            Name = "DemoList",
                            AttachedProperties = { { DockPanel.DockProperty, Dock.Left} },
                            ItemPadding = new Thickness(5),
                            VerticalAlignment = VerticalAlignment.Stretch,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            SelectedIndex = 0,
                            Items = { controlTest, stackTest, dockTest }
                        },
                        new ContentControl
                        {
                            Name = "Content",
                            BackgroundColor = new Color(30, 30, 30)
                        }
                    }
                }
            };

            _guiSystem = new GuiSystem(viewportAdapter, guiRenderer) { ActiveScreen = demoScreen };

            var demoList = demoScreen.FindControl<ListBox>("DemoList");
            var demoContent = demoScreen.FindControl<ContentControl>("Content");

            demoList.SelectedIndexChanged += (sender, args) => demoContent.Content = (demoList.SelectedItem as DemoViewModel)?.Content;
            demoContent.Content = (demoList.SelectedItem as DemoViewModel)?.Content;
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
    }

    public class DemoViewModel
    {
        public DemoViewModel(string name, object content)
        {
            Name = name;
            Content = content;
        }

        public string Name { get; }
        public object Content { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
