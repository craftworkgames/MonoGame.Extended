using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.Features.Demos
{
    public class GuiLayoutScreen : GuiScreen
    {
        private readonly Texture2D _texture;

        public GuiLayoutScreen(GuiSkin skin, GraphicsDevice graphicsDevice)
            : base(skin)
        {
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { new Color(Color.Black, 0.5f) });
        }

        protected override void Dispose(bool isDisposing)
        {
            _texture.Dispose();
        }

        public override void Initialize()
        {
            var backgroundRegion = new TextureRegion2D(_texture);

            var uniformGrid = new GuiUniformGrid(backgroundRegion)
            {
                Text = "Uniform Grid",
                Controls =
                {
                    new GuiCanvas(backgroundRegion)
                    {
                        Text = "Canvas",
                        Controls =
                        {
                            Skin.Create<GuiButton>("white-button", c => { c.Text = "Reasonably long text"; c.Position = new Vector2(0, 0); }),
                            Skin.Create<GuiButton>("white-button", c => { c.Text = "Child 2"; c.Position = new Vector2(50, 50); }),
                            Skin.Create<GuiButton>("white-button", c => { c.Text = "Child 3"; c.Position = new Vector2(100, 100); }),
                        }
                    },
                    new GuiStackPanel(backgroundRegion)
                    {
                        Text = "Stack Panel (Vertical)",
                        Orientation = GuiOrientation.Vertical,
                        Controls =
                        {
                            Skin.Create<GuiButton>("white-button", c => { c.Text = "Child 1"; c.HorizontalAlignment = HorizontalAlignment.Centre; }),
                            Skin.Create<GuiButton>("white-button", c => { c.Text = "Child 2"; c.HorizontalAlignment = HorizontalAlignment.Right; }),
                            Skin.Create<GuiButton>("white-button", c => { c.Text = "Child 3"; c.HorizontalAlignment = HorizontalAlignment.Left; }),
                            Skin.Create<GuiButton>("white-button", c => { c.Text = "Child 4"; }),
                        }
                    },
                    new GuiStackPanel(backgroundRegion)
                    {
                        Text = "Stack Panel (Horizontal)",
                        Orientation = GuiOrientation.Horizontal,
                        Controls =
                        {
                            Skin.Create<GuiButton>("white-button", c => { c.Text = "Child 1"; c.Width = 80; c.VerticalAlignment = VerticalAlignment.Centre; }),
                            Skin.Create<GuiButton>("white-button", c => { c.Text = "Child 2"; c.Width = 80; c.VerticalAlignment = VerticalAlignment.Top; }),
                            Skin.Create<GuiButton>("white-button", c => { c.Text = "Child 3"; c.Width = 80; c.VerticalAlignment = VerticalAlignment.Bottom; }),
                            Skin.Create<GuiButton>("white-button", c => { c.Text = "Child 3"; c.Width = 80; }),
                        }
                    },
                    new GuiListBox(backgroundRegion)
                    {
                        Name = "DisplayModesListBox"
                    }
                }
            };

            //var stackPanel = ;
            //stackPanel.PerformLayout();

            Controls.Add(uniformGrid);

            var listBox = FindControl<GuiListBox>("DisplayModesListBox");
            listBox.Items.AddRange(GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.Select(i => $"{i.Width}x{i.Height}"));
        }
    }

    public class GuiLayoutDemo : DemoBase
    {
        public override string Name => "GUI Layouts";

        private Camera2D _camera;
        private GuiSystem _guiSystem;

        public GuiLayoutDemo(GameMain game) : base(game)
        {
        }

        protected override void LoadContent()
        {
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            var skin = LoadSkin(@"Raw/adventure-gui-skin.json");
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, _camera.GetViewMatrix);

            _guiSystem = new GuiSystem(viewportAdapter, guiRenderer)
            {
                Screen = new GuiLayoutScreen(skin, GraphicsDevice)
            };
        }

        private GuiSkin LoadSkin(string assetName)
        {
            using (var stream = TitleContainer.OpenStream(assetName))
            {
                return GuiSkin.FromStream(stream, Content);
            }
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = gameTime.GetElapsedSeconds();
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