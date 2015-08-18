using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;

namespace Sandbox
{
    /// <summary>
    /// You can use this sandbox game to test features that require manual interaction.
    /// Code in this class is typically throw away testing. It's not part of the library.
    /// </summary>
    public class SandboxGame : Game
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private Camera2D _camera;
        private Texture2D _backgroundTexture;
        private Sprite _stumpSprite;
        private MouseState _previousMouseState;
        private ViewportAdapter _viewportAdapter;
        private BitmapFont _bitmapFont;
        private TiledMap _tiledMap;

        public SandboxGame()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.IsBorderless = false;
            Window.Position = new Point(50, 50);
            Window.Title = string.Format("MonoGame.Extended - {0}", GetType().Name);

            _graphicsDeviceManager.PreferredBackBufferWidth = 1024;
            _graphicsDeviceManager.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize()
        {
            _viewportAdapter = new BoxingViewportAdapter(GraphicsDevice, 800, 480);
            _camera = new Camera2D(_viewportAdapter)
            {
                Zoom = 0.5f,
                Position = new Vector2(408, 270)
            };

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (s, e) => _viewportAdapter.OnClientSizeChanged();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _backgroundTexture = Content.Load<Texture2D>("hills");
            _bitmapFont = Content.Load<BitmapFont>("courier-new-32");
            _stumpSprite = new Sprite(Content.Load<Texture2D>("stump"));
            _stumpSprite.Position = new Vector2(200, 200);
            _tiledMap = Content.Load<TiledMap>("level01");
        }

        protected override void UnloadContent()
        {
        }

        private int _previousScrollWheelValue;

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            //var gamePadState = GamePad.GetState(PlayerIndex.One);

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            var up = new Vector2(0, -250);
            var right = new Vector2(250, 0);

            // rotation
            if (keyboardState.IsKeyDown(Keys.Q))
                _camera.Rotation -= deltaTime;

            if (keyboardState.IsKeyDown(Keys.W))
                _camera.Rotation += deltaTime;

            // movement
            var direction = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.Up))
                direction += up * deltaTime;

            if (keyboardState.IsKeyDown(Keys.Down))
                direction += -up * deltaTime;
            
            if (keyboardState.IsKeyDown(Keys.Left))
                direction += -right * deltaTime;
            
            if (keyboardState.IsKeyDown(Keys.Right))
                direction += right * deltaTime;

            _camera.Move(direction);
            
            // zoom
            var scrollWheelDelta = mouseState.ScrollWheelValue - _previousScrollWheelValue;

            if (scrollWheelDelta != 0)
                _camera.Zoom += scrollWheelDelta * 0.0001f;

            _previousScrollWheelValue = mouseState.ScrollWheelValue;

            // look at
            if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                var p = _viewportAdapter.PointToScreen(mouseState.X, mouseState.Y);
                Trace.WriteLine(string.Format("{0},{1} => {2},{3}", mouseState.X, mouseState.Y, p.X, p.Y));
            }

            _previousMouseState = mouseState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            //_spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            //_spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, _tiledMap.WidthInPixels, _tiledMap.HeightInPixels), Color.White);
            //_spriteBatch.End();

            //_tiledMap.Draw(_camera);

            //_spriteBatch.Begin();
            //_spriteBatch.DrawString(_bitmapFont, "This is MonoGame.Extended", new Vector2(50, 50), new Color(Color.Black, 0.5f));
            //_spriteBatch.DrawString(_bitmapFont, string.Format("Camera: {0}", _camera.Position), new Vector2(50, 100), Color.Black);
            ////_spriteBatch.DrawString(_bitmapFont, 
            ////    "Contrary to popular belief, Lorem Ipsum is not simply random text.\n\n" + 
            ////    "It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard " + 
            ////    "McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin " + 
            ////    "words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, " + 
            ////    "discovered the undoubtable source.", new Vector2(50, 100), new Color(Color.Black, 0.5f), _viewportAdapter.VirtualWidth - 50);
            //_spriteBatch.End();

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            //Console.WriteLine(_stumpSprite.Position);
            _spriteBatch.Draw(_stumpSprite);
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
