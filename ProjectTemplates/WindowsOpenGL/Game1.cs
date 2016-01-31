using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;

namespace WindowsOpenGL
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private Camera2D _camera;
        private Sprite _sprite;
        private readonly bool _isFullScreen;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            _isFullScreen = false;

            Content.RootDirectory = "Content";
            Window.Title = "MonoGame.Extended Game";
            Window.AllowUserResizing = true;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (_isFullScreen)
            {
                _graphicsDeviceManager.IsFullScreen = true;
                _graphicsDeviceManager.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                _graphicsDeviceManager.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                _graphicsDeviceManager.ApplyChanges();
            }
        }

        protected override void LoadContent()
        {
            var viewportAdapter = new BoxingViewportAdapter(GraphicsDevice, 800, 480);

            _camera = new Camera2D(viewportAdapter);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var texture = Content.Load<Texture2D>("logo-square-512");
            _sprite = new Sprite(texture)
            {
                Position = new Vector2(viewportAdapter.VirtualWidth / 2f, viewportAdapter.VirtualHeight / 2f)
            };
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _sprite.Rotation += deltaTime;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_sprite);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
