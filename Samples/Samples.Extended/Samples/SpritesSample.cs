using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;

namespace Samples.Extended.Samples
{
    public class SpritesSample : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        
        private Sprite _axeSprite;
        private Sprite _spikeyBallSprite;
        private Sprite _particleSprite;
        private float _particleOpacity;

        public SpritesSample()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            var axeTexture = Content.Load<Texture2D>("axe");
            _axeSprite = new Sprite(axeTexture)
            {
                Origin = new Vector2(243, 679),
                Position = new Vector2(400, 480),
                Scale = Vector2.One * 0.5f
            };

            var spikeyBallTexture = Content.Load<Texture2D>("spike_ball");
            _spikeyBallSprite = new Sprite(spikeyBallTexture)
            {
                Position = new Vector2(400, 240)
            };

            var particleTexture = Content.Load<Texture2D>("particle");
            _particleSprite = new Sprite(particleTexture)
            {
                Color = new Color(Color.White, 0.0f),
                Position = new Vector2(600, 240)
            };
            _particleOpacity = 0.0f;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _axeSprite.Rotation += deltaTime * 2.5f;
            _spikeyBallSprite.Rotation -= deltaTime;
            _particleOpacity = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds);
            _particleSprite.Color = new Color(_particleSprite.Color, _particleOpacity);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_axeSprite);
            _spriteBatch.Draw(_spikeyBallSprite);
            _spriteBatch.Draw(_particleSprite);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
