using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

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

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _camera = new Camera2D(GraphicsDevice.Viewport);
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

            //var texture = Content.Load<Texture2D>("shadedDark42");
            //new TextureRegion2D(texture, 5, 5, 32, 32);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();
            var gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            var up = new Vector2(0, -250);
            var right = new Vector2(250, 0);

            if (keyboardState.IsKeyDown(Keys.Q))
                _camera.Rotation -= deltaTime;

            if (keyboardState.IsKeyDown(Keys.W))
                _camera.Rotation += deltaTime;

            if (keyboardState.IsKeyDown(Keys.Up))
                _camera.Position += up * deltaTime;

            if (keyboardState.IsKeyDown(Keys.Down))
                _camera.Position += -up * deltaTime;
            
            if (keyboardState.IsKeyDown(Keys.Left))
                _camera.Position += -right * deltaTime;
            
            if (keyboardState.IsKeyDown(Keys.Right))
                _camera.Position += right * deltaTime;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_backgroundTextureSky, Vector2.Zero);
            _spriteBatch.Draw(_backgroundTextureClouds, Vector2.Zero);
            _spriteBatch.End();
            
            for(var i = 0; i < 4; i++)
            {
                var parallaxFactor = new Vector2(1.0f + 0.2f * i, 1.0f - 0.05f * i);
                var transformMatrix = _camera.GetViewMatrix(parallaxFactor);
                _spriteBatch.Begin(transformMatrix: transformMatrix);
                _spriteBatch.Draw(_backgroundTexture[i], Vector2.Zero);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
