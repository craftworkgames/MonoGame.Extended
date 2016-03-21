using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.BitmapFonts
{
    public class Game1 : Game
    {
        private Texture2D _backgroundTexture;
        private BitmapFont _bitmapFont;
        private Camera2D _camera;
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphicsDeviceManager;
        private Vector2 _labelPosition = Vector2.Zero;
        private string _labelText = "";
        private SpriteBatch _spriteBatch;
        private ViewportAdapter _viewportAdapter;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            //Window.Position = Point.Zero;
        }

        protected override void LoadContent()
        {
            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new Camera2D(_viewportAdapter);
            _backgroundTexture = Content.Load<Texture2D>("vignette");
            _bitmapFont = Content.Load<BitmapFont>("montserrat-32");

            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            //if (keyboardState.IsKeyDown(Keys.Escape))
            //{
            //    Exit();
            //}

            _labelText = $"{mouseState.X}, {mouseState.Y}";
            var stringRectangle = _bitmapFont.GetStringRectangle(_labelText, Vector2.Zero);
            _labelPosition = new Vector2(400 - stringRectangle.Width / 2, 440);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());
            _spriteBatch.Draw(_backgroundTexture, _viewportAdapter.BoundingRectangle, Color.White);
            _spriteBatch.DrawString(_bitmapFont, "MonoGame.Extended BitmapFont Sample", new Vector2(50, 10), Color.White);
            _spriteBatch.DrawString(_bitmapFont, "Contrary to popular belief, Lorem Ipsum is not simply random text.\n\n" + "It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard " + "McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin " + "words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, " + "discovered the undoubtable source.", new Vector2(50, 50), new Color(Color.Black, 0.5f), 750);
            _spriteBatch.DrawString(_bitmapFont, _labelText, _labelPosition, Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}