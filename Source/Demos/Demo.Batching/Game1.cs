using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Graphics.Effects;

namespace Demo.Batching
{
    public struct SpriteInfo
    {
        public Vector2 Position;
        public float Rotation;
        public Color Color;
        public Texture2D Texture;
    }

    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        private DynamicBatchRenderer2D _batch;
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private SpriteBatch _spriteBatch;
        private BitmapFont _bitmapFont;
        private Texture2D _spriteTexture1;
        private Texture2D _spriteTexture2;
        private Vector2 _spriteOrigin;
        private DefaultEffect2D _effect;

        private readonly Random _random = new Random();
        private readonly FramesPerSecondCounter _fpsCounter = new FramesPerSecondCounter();
        private readonly SpriteInfo[] _sprites = new SpriteInfo[2048];

        public Game1()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = false;
            // disable fixed time step so max frames can be measured otherwise the update & draw frames would be capped to the default 60 fps timestep
            //IsFixedTimeStep = false;

            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                // also disable v-sync so max frames can be measured otherwise draw frames would be capped to the screen's refresh rate 
                SynchronizeWithVerticalRetrace = false,
                PreferredBackBufferWidth = 1024,
                PreferredBackBufferHeight = 768
            };
        }

        protected override void LoadContent()
        {
            var graphicsDevice = GraphicsDevice;

            _batch = new DynamicBatchRenderer2D(graphicsDevice);
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _bitmapFont = Content.Load<BitmapFont>("montserrat-32");
            _effect = new DefaultEffect2D(graphicsDevice);

            // load the texture for the sprites
            _spriteTexture1 = Content.Load<Texture2D>("logo-square-128");
            _spriteTexture2 = Content.Load<Texture2D>("logo-square-128-copy");
            _spriteOrigin = new Vector2(_spriteTexture1.Width * 0.5f, _spriteTexture1.Height * 0.5f);

            var viewport = GraphicsDevice.Viewport;

            _effect.World = Matrix.Identity;
            _effect.View = Matrix.Identity;
            _effect.Projection = Matrix.CreateTranslation(-0.5f, -0.5f, 0) *
                                 Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _sprites.Length; index++)
            {
                var sprite = _sprites[index];
                sprite.Position = new Vector2(_random.Next(viewport.X, viewport.Width),
                    _random.Next(viewport.Y, viewport.Height));
                sprite.Rotation = MathHelper.ToRadians(_random.Next(0, 360));
                sprite.Texture = index % 2 == 0 ? _spriteTexture1 : _spriteTexture2;
                _sprites[index] = sprite;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            var gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
                Exit();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _sprites.Length; index++)
            {
                var sprite = _sprites[index];

                if (index % 2 == 0)
                    sprite.Rotation = (sprite.Rotation + MathHelper.ToRadians(1)) % MathHelper.TwoPi;
                else
                    sprite.Rotation = (sprite.Rotation - MathHelper.ToRadians(1) + MathHelper.TwoPi) % MathHelper.TwoPi;

                sprite.Color = ColorHelper.FromHsl(sprite.Rotation / MathHelper.TwoPi, 0.5f, 0.3f);

                _sprites[index] = sprite;
            }

            _fpsCounter.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var graphicsDevice = GraphicsDevice;

            graphicsDevice.Clear(Color.Black);

            // comment and uncomment either of the two below lines to compare
            DrawSpritesWithBatch2D();
            //DrawSpritesWithSpriteBatch();

            _batch.Begin();

            // use StringBuilder to prevent garbage
            _stringBuilder.Clear();
            _stringBuilder.Append("FPS: ");
            _stringBuilder.Append(_fpsCounter.FramesPerSecond); // but, this StringBulder method causes a small amount of garbage...
            _batch.DrawString(_bitmapFont, _stringBuilder, Vector2.Zero);

            _batch.End();

            base.Draw(gameTime);

            _fpsCounter.Draw(gameTime);
        }

        private void DrawSpritesWithBatch2D()
        {
            _batch.Begin(Batch2DSortMode.Texture, effect: _effect);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _sprites.Length; index++)
            {
                var sprite = _sprites[index];
                _batch.DrawSprite(sprite.Texture, sprite.Position, rotation: sprite.Rotation, origin: _spriteOrigin, color: sprite.Color);
            }

            _batch.End();
        }

        private void DrawSpritesWithSpriteBatch()
        {
            _spriteBatch.Begin(SpriteSortMode.Texture, effect: _effect);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _sprites.Length; index++)
            {
                var sprite = _sprites[index];
                _spriteBatch.Draw(sprite.Texture, sprite.Position, rotation: sprite.Rotation, origin: _spriteOrigin, color: sprite.Color);
            }

            _spriteBatch.End();
        }
    }
}
