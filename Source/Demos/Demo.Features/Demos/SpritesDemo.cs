using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace Demo.Features.Demos
{
    public class SpritesDemo : DemoBase
    {
        public override string Name => "Sprites";

        private Sprite _axeSprite;
        private Texture2D _backgroundTexture;
        private float _particleOpacity;
        private Sprite _particleSprite0;
        private Sprite _particleSprite1;
        private Sprite _spikeyBallSprite;
        private SpriteBatch _spriteBatch;

        private TextureRegion2D _clippingTextureRegion;

        public SpritesDemo(GameMain game) : base(game)
        {
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _backgroundTexture = Content.Load<Texture2D>("Textures/bg_sharbi");

            var testRegion = new TextureRegion2D(Content.Load<Texture2D>("Textures/clipping-test"));
            _clippingTextureRegion = new NinePatchRegion2D(testRegion, 16);

            var axeTexture = Content.Load<Texture2D>("Textures/axe");
            _axeSprite = new Sprite(axeTexture)
            {
                Origin = new Vector2(243, 679),
                Position = new Vector2(400, 0),
                Scale = Vector2.One * 0.5f
            };

            var spikeyBallTexture = Content.Load<Texture2D>("Textures/spike_ball");
            _spikeyBallSprite = new Sprite(spikeyBallTexture)
            {
                Position = new Vector2(400, 340)
            };

            var particleTexture = Content.Load<Texture2D>("Textures/particle");
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

        private MouseState _previousMouseState;

        protected override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _axeSprite.Rotation = MathHelper.ToRadians(180) + MathHelper.PiOver2 * 0.8f * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds);

            _spikeyBallSprite.Rotation -= deltaTime * 2.5f;
            _spikeyBallSprite.Position = new Vector2(mouseState.X, mouseState.Y);

            _particleOpacity = 0.5f + (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds);
            _particleSprite0.Color = Color.White * _particleOpacity;
            _particleSprite1.Color = Color.White * (1.0f - _particleOpacity);

            var dx = mouseState.X - _previousMouseState.X;
            var dy = mouseState.Y - _previousMouseState.Y;

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                _clippingRectangle.X += dx;
                _clippingRectangle.Y += dy;
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                _clippingRectangle.Width += dx;
                _clippingRectangle.Height += dy;
            }

            _previousMouseState = mouseState;

            base.Update(gameTime);
        }

        private Rectangle _clippingRectangle = new Rectangle(50 + 32, 250 - 32, 64, 128 + 64);

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            _spriteBatch.Draw(_axeSprite);
            _spriteBatch.Draw(_spikeyBallSprite);
            _spriteBatch.Draw(_particleSprite0);
            _spriteBatch.Draw(_particleSprite1);

            // clipping test
            _spriteBatch.Draw(_clippingTextureRegion, new Rectangle(50, 50, 128, 128), Color.White, clippingRectangle: null);
            _spriteBatch.Draw(_clippingTextureRegion, new Rectangle(50, 250, 512, 512), Color.White, clippingRectangle: _clippingRectangle);
            _spriteBatch.DrawRectangle(_clippingRectangle.ToRectangleF(), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}