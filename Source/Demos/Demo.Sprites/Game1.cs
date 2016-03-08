using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;

namespace Demo.Sprites
{
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private Sprite _axeSprite;
        private Texture2D _backgroundTexture;
        private float _particleOpacity;
        private Sprite _particleSprite0;
        private Sprite _particleSprite1;
        private Sprite _spikeyBallSprite;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.Position = Point.Zero;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _backgroundTexture = Content.Load<Texture2D>("bg_sharbi");

            var axeTexture = Content.Load<Texture2D>("axe");
            _axeSprite = new Sprite(axeTexture)
            {
                Origin = new Vector2(243, 679),
                Position = new Vector2(400, 0),
                Scale = Vector2.One * 0.5f
            };

            var spikeyBallTexture = Content.Load<Texture2D>("spike_ball");
            _spikeyBallSprite = new Sprite(spikeyBallTexture)
            {
                Position = new Vector2(400, 340)
            };

            var particleTexture = Content.Load<Texture2D>("particle");
            _particleSprite0 = new Sprite(particleTexture)
            {
                Position = new Vector2(600, 340)
            };
            _particleSprite1 = new Sprite(particleTexture)
            {
                Position = new Vector2(200, 340)
            };
            _particleOpacity = 0.0f;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            _axeSprite.Rotation = MathHelper.ToRadians(180) + MathHelper.PiOver2 * 0.8f * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds);

            _spikeyBallSprite.Rotation -= deltaTime * 2.5f;
            _spikeyBallSprite.Position = new Vector2(mouseState.X, mouseState.Y);

            _particleOpacity = 0.5f + (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds);
            _particleSprite0.Color = Color.White * _particleOpacity;
            _particleSprite1.Color = Color.White * (1.0f - _particleOpacity);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            _spriteBatch.Draw(_axeSprite);
            _spriteBatch.Draw(_spikeyBallSprite);
            _spriteBatch.Draw(_particleSprite0);
            _spriteBatch.Draw(_particleSprite1);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}