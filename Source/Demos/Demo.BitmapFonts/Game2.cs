using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
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

            _rotation += deltaTime * 0.01f;

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

            const string helloWorld = "The quick brown fox jumps over the lazy dog\r\nThe lazy dog jumps back over the quick brown fox";
            
            var position = new Vector2(400, 240);
            var offset = new Vector2(0, 100);
            var scale = Vector2.One * 1.2f;
            var color = Color.White;
            //_rotation = MathHelper.Pi/16f;

            // sprite font
            var spriteFontSize = _spriteFont.MeasureString(helloWorld);
            
            _spriteBatch.DrawString(
                spriteFont: _spriteFont,
                text: helloWorld,
                position: position - offset,
                color: color,
                rotation: _rotation,
                origin: new Vector2(spriteFontSize.X / 2f, spriteFontSize.Y / 2f),
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0);
            
            //_spriteBatch.DrawRectangle(position - origin - offset, spriteFontSize, Color.Magenta);

            // bitmap font
            var bitmapFontSize = _bitmapFont.MeasureString(helloWorld);

            _spriteBatch.DrawString(
                bitmapFont: _bitmapFont,
                text: helloWorld,
                position: position + offset,
                color: color,
                rotation: _rotation,
                origin: new Vector2(bitmapFontSize.Width / 2f, bitmapFontSize.Height / 2f),
                scale: scale,
                effect: SpriteEffects.None,
                layerDepth: 0);
            
            //_spriteBatch.DrawRectangle(position - origin + offset, bitmapFontSize, Color.Red);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}