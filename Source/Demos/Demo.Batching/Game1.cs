﻿using System;
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
        public Matrix2D TransformMatrix;
    }

    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        private Batcher2D _batcher;
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
        private Matrix _worldMatrix;
        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;

        public Game1()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = false;
            // disable fixed time step so max frames can be measured otherwise the update & draw frames would be capped to the default 60 fps timestep
            IsFixedTimeStep = false;

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

            _effect = new DefaultEffect2D(graphicsDevice);
            _batcher = new Batcher2D(graphicsDevice);
            _spriteBatch = new SpriteBatch(graphicsDevice);
            _bitmapFont = Content.Load<BitmapFont>("montserrat-32");

            // load the texture for the sprites
            _spriteTexture1 = Content.Load<Texture2D>("logo-square-128");
            _spriteTexture2 = Content.Load<Texture2D>("logo-square-128-copy");
            _spriteOrigin = new Vector2(_spriteTexture1.Width * 0.5f, _spriteTexture1.Height * 0.5f);

            var viewport = GraphicsDevice.Viewport;

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

                sprite.TransformMatrix = Matrix2D.CreateFrom(sprite.Position, sprite.Rotation, null, _spriteOrigin);

                _sprites[index] = sprite;
            }

            _fpsCounter.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var graphicsDevice = GraphicsDevice;

            graphicsDevice.Clear(Color.Black);

            // update the matrices
            _worldMatrix = Matrix.Identity;
            _viewMatrix = _effect.View = Matrix.Identity;
            _projectionMatrix = _effect.Projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, -1);

            // comment and uncomment either of the two below lines to compare

            DrawSpritesWithBatcher2D();
            //DrawSpritesWithSpriteBatch();

            //_batcher.Begin(viewMatrix: _viewMatrix, projectionMatrix: _projectionMatrix);

            //// use StringBuilder to prevent garbage
            //_stringBuilder.Clear();
            //_stringBuilder.Append("FPS: ");
            //_stringBuilder.Append(_fpsCounter.FramesPerSecond); // but, this StringBulder method causes a small amount of garbage...
            //_batcher.DrawString(_bitmapFont, _stringBuilder, Vector2.Zero);

            //_stringBuilder.Clear();
            //_stringBuilder.Append("Draw Calls: ");
            //_stringBuilder.Append(GraphicsDevice.Metrics.DrawCount);
            //_batcher.DrawString(_bitmapFont, _stringBuilder, new Vector2(0, _bitmapFont.LineHeight));

            //_batcher.End();

            base.Draw(gameTime);

            _fpsCounter.Draw(gameTime);
        }

        private void DrawSpritesWithBatcher2D()
        {
            _batcher.Begin(viewMatrix: _viewMatrix, projectionMatrix: _projectionMatrix);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < _sprites.Length; index++)
            {
                var sprite = _sprites[index];
                _batcher.DrawTexture(sprite.Texture, ref sprite.TransformMatrix, sprite.Color);
            }

            _batcher.End();
        }

        private void DrawSpritesWithSpriteBatch()
        {
            _effect.Projection = _projectionMatrix;
            _effect.View = _viewMatrix;
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
