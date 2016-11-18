using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
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
        private SpriteBatch _spriteBatch;
        //private readonly GuiComponent _guiComponent;
        private GuiSpriteBatchRenderer _guiRenderer;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            //Components.Add(_guiComponent = new GuiComponent(this));
        }

        protected override void Initialize()
        {
            base.Initialize();

        }

        protected override void LoadContent()
        {
            //var layout = _guiComponent.LoadGui("title-screen.gui");
            //var button = layout.FindControl<GuiButton>("PlayButton");
            //button.IsEnabled = false;

            //var button2 = layout.FindControl<GuiButton>("PlayButton1");
            //button2.Click += (sender, args) => button.IsEnabled = !button.IsEnabled;
            var texture = Content.Load<Texture2D>("kenney-gui-blue");
            var textureRegion = new TextureRegion2D(texture, 190, 94, 100, 100);
            var bluePanel = new NinePatchRegion2D(textureRegion, 10, 10, 10, 10);

            var screen = new GuiScreen
            {
                Controls =
                {
                    new GuiPanel
                    {
                        Name = "HelloPanel",
                        Position = new Vector2(100, 100),
                        Size = new SizeF(400, 240),
                        BackgroundRegion = bluePanel
                    }
                }
            };

            _guiRenderer = new GuiSpriteBatchRenderer(screen);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            //var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap, blendState: BlendState.AlphaBlend);
            _guiRenderer.Draw(_spriteBatch);
            _spriteBatch.End();
        }
    }
}