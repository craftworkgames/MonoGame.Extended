using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;

namespace Game1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private Camera2D _camera;
        private Texture2D[] _backgroundTexture;
        private Texture2D _backgroundTextureClouds;
        private Texture2D _backgroundTextureSky;
        private MouseState _previousMouseState;
        private ViewportAdapter _viewportAdapter;
        private BitmapFont _bitmapFont;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.IsBorderless = false;
            Window.Position = new Point(50, 50);
            Window.Title = "MonoGame.Extended.Sandbox";

            _graphicsDeviceManager.PreferredBackBufferWidth = 1024;
            _graphicsDeviceManager.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize()
        {
            _viewportAdapter = new DefaultViewportAdapter(GraphicsDevice);
            _camera = new Camera2D(_viewportAdapter)
            {
                Zoom = 0.5f,
                Position = new Vector2(900, 650)
            };

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (s, e) => _viewportAdapter.OnClientSizeChanged();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _backgroundTexture = new Texture2D[4];
            _backgroundTexture[0] = Content.Load<Texture2D>("Hills_1");
            _backgroundTexture[1] = Content.Load<Texture2D>("Hills_2");
            _backgroundTexture[2] = Content.Load<Texture2D>("Hills_3");
            _backgroundTexture[3] = Content.Load<Texture2D>("Hills_4");
            _backgroundTextureClouds = Content.Load<Texture2D>("Hills_Couds");
            _backgroundTextureSky = Content.Load<Texture2D>("Hills_Sky");

            //_bitmapFont = Content.Load<BitmapFont>("courier-new-32.fnt");
            _bitmapFont = new BitmapFontLoader().Load(Content, "courier-new-32.fnt");

            //var texture = Content.Load<Texture2D>("shadedDark42");
            //new TextureRegion2D(texture, 5, 5, 32, 32);
        }

        protected override void UnloadContent()
        {
        }

        private int _previousScrollWheelValue = 0;

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            var gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
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
            
            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(new Vector2(0.25f, 1.0f)));
            _spriteBatch.Draw(_backgroundTextureSky, Vector2.Zero);
            _spriteBatch.Draw(_backgroundTextureClouds, Vector2.Zero);
            _spriteBatch.End();

            for (var i = 0; i < 4; i++)
            {
                var parallaxFactor = new Vector2(0.5f + 0.5f * i, 1.0f);
                var viewMatrix = _camera.GetViewMatrix(parallaxFactor);
                _spriteBatch.Begin(transformMatrix: viewMatrix);
                _spriteBatch.Draw(_backgroundTexture[i], Vector2.Zero);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
