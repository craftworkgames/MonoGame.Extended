using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.BitmapFonts
{
    public class Game2 : Game
    {
        private GraphicsDeviceManager _graphicsDeviceManager;
        private BoxingViewportAdapter _viewportAdapter;
        private Texture2D _backgroundTexture;
        private BitmapFont _bitmapFont;
        private SpriteBatch _spriteBatch;

        public Game2()
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

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.LinearClamp, transformMatrix: _viewportAdapter.GetScaleMatrix());

            DrawText("Hello World!", new Vector2(20, 30), Color.White);
            DrawText("New\r\nLine", new Vector2(220, 30), Color.White);
            DrawText("New\r\nLine", new Vector2(220, 30), Color.White);
            DrawText("This is a very long line that should be wrapped", new Vector2(20, 230), Color.White);

            _spriteBatch.End();



            base.Draw(gameTime);
        }

        private void DrawText(string text, Vector2 position, Color color)
        {
            _spriteBatch.DrawRectangle(_bitmapFont.GetStringRectangle(text, position), Color.Red);
            _spriteBatch.DrawString(_bitmapFont, text, position, color, wrapWidth: 400);
        }
    }
}