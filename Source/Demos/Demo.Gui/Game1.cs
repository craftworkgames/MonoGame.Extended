using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.Gui
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private GuiManager _guiManager;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            var font = Content.Load<BitmapFont>("small-font");
            var textureAtlas = Content.Load<TextureAtlas>("adventure-gui-atlas");
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            var renderer = new GuiSpriteBatchRenderer(GraphicsDevice, font);

            var buttonRegion = textureAtlas["buttonLong_grey"];
            var buttonRegionPressed = textureAtlas["buttonLong_grey_pressed"];

            _guiManager = new GuiManager(viewportAdapter, renderer);

            var screen = new GuiScreen
            {
                Controls =
                {
                    new GuiButton(buttonRegion)
                    {
                        Name = "MyButton",
                        Position = new Vector2(400, 240),
                        Text = "Hello World",
                        TextOffset = new Vector2(0, -2),
                        Color = Color.DarkRed,
                        PressedStyle = new GuiControlStyle(typeof(GuiButton))
                        {
                            Setters =
                            {
                                { nameof(GuiButton.TextureRegion), buttonRegionPressed },
                                { nameof(GuiButton.TextOffset), new Vector2(0, 2) }
                            }
                        },
                        HoverStyle = new GuiControlStyle(typeof(GuiButton))
                        {
                            Setters =
                            {
                                { nameof(GuiButton.Color), Color.Red }
                            }
                        }

                    }
                }
            };

            _guiManager.Screen = screen;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _guiManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _guiManager.Draw(gameTime);

        }
    }
}
