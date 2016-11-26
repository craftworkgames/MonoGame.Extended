using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
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
        private SpriteFont _spriteFont;
        private SpriteBatch _spriteBatch;
        private float _rotation = 0;

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
            _bitmapFont = Content.Load<BitmapFont>("impact-32");
            _spriteFont = Content.Load<SpriteFont>("test");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
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

            //_rotation += deltaTime;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(
                samplerState: SamplerState.LinearClamp, 
                blendState: BlendState.AlphaBlend, 
                transformMatrix: _viewportAdapter.GetScaleMatrix());
            //_spriteBatch.Draw(_backgroundTexture, _viewportAdapter.BoundingRectangle, Color.White);

            const string helloWorld = "The quick brown fox jumps over the lazy dog\r\nLorem ipsum dolor sit amet, consectetuer.";

            //DrawText(helloWorld, new Vector2(20, 30), Color.White);
            //DrawText("New\r\nLine", new Vector2(220, 30), Color.White);
            //DrawText("This is a very long line that should be wrapped", new Vector2(20, 230), Color.White);

            var position = new Vector2(20, 240);
            var offset = new Vector2(0, 50);
            var origin = Vector2.Zero;

            // sprite font
            _spriteBatch.DrawString(
                spriteFont: _spriteFont,
                text: helloWorld,
                position: position - offset,
                color: Color.White,
                rotation: _rotation,
                origin: origin,
                scale: Vector2.One,
                effects: SpriteEffects.None,
                layerDepth: 0);
            _spriteBatch.DrawRectangle(position - origin - offset, _spriteFont.MeasureString(helloWorld), Color.Magenta);

            // bitmap font
            _spriteBatch.DrawString(
                bitmapFont: _bitmapFont,
                text: helloWorld,
                position: position + offset,
                color: Color.White,
                rotation: _rotation,
                origin: origin,
                scale: Vector2.One,
                effects: SpriteEffects.None,
                layerDepth: 0);
            _spriteBatch.DrawRectangle(position - origin + offset, _bitmapFont.MeasureString(helloWorld), Color.Red);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawText(string text, Vector2 position, Color color)
        {
            var stringRectangle = _bitmapFont.GetStringRectangle(text, position);
            _spriteBatch.DrawRectangle(stringRectangle, Color.Red);
            _spriteBatch.DrawString(_bitmapFont, text, position, color, wrapWidth: 400);
        }
    }
}