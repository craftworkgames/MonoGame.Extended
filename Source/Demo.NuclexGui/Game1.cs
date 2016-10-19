using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.NuclexGui;
using MonoGame.Extended.NuclexGui.Controls.Desktop;
using MonoGame.Extended.InputListeners;

namespace Demo.NuclexGui
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private Sprite _sprite;
        private Camera2D _camera;

        GuiManager _gui;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            _gui = new GuiManager(Services);
        }

        protected override void Initialize()
        {
            base.Initialize();

            _gui.Screen = new GuiScreen(800, 480);
            _gui.Screen.Desktop.Bounds = new UniRectangle(new UniScalar(0f, 0), new UniScalar(0f, 0), new UniScalar(1f, 0), new UniScalar(1f, 0));

            _gui.Initialize();

            GuiButtonControl button = new GuiButtonControl();
            button.Name = "button";
            button.Bounds = new UniRectangle(new UniScalar(0f, 0), new UniScalar(0f, 0), new UniScalar(0f, 50), new UniScalar(0f, 50));
            _gui.Screen.Desktop.Children.Add(button);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(viewportAdapter);

            var logoTexture = Content.Load<Texture2D>("logo-square-128");
            _sprite = new Sprite(logoTexture)
            {
                Position = viewportAdapter.Center.ToVector2()
            };
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _gui.Update(gameTime);

            _sprite.Rotation += deltaTime;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());

            _gui.Draw(gameTime);

            _spriteBatch.Draw(_sprite);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}