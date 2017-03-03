﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Graphics.Effects;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.BitmapFonts
{
    public class Game2 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphicsDeviceManager;
        private BoxingViewportAdapter _viewportAdapter;
        private Texture2D _backgroundTexture;
        private BitmapFont _bitmapFontImpact;
        
        private SpriteBatch _spriteBatch;
        private BitmapFont _bitmapFontMontserrat;
        private Rectangle _clippingRectangle = new Rectangle(100, 100, 300, 300);
        private MouseState _previousMouseState;

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
            _bitmapFontImpact = Content.Load<BitmapFont>("impact-32");
            _bitmapFontMontserrat = Content.Load<BitmapFont>("montserrat-32");
            
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(
                samplerState: SamplerState.LinearClamp, 
                blendState: BlendState.AlphaBlend, 
                transformMatrix: _viewportAdapter.GetScaleMatrix());
            _spriteBatch.Draw(_backgroundTexture, _viewportAdapter.BoundingRectangle, Color.DarkBlue);

            const string helloWorld = "The quick brown fox jumps over the lazy dog\nThe lazy dog jumps back over the quick brown fox";
            
            var position = new Point2(400, 140);
            var offset = new Vector2(0, 50);
            var scale = Vector2.One;
            var color = Color.White;
            var rotation = 0;//MathHelper.Pi/64f;

            // bitmap font
            var bitmapFontSize = _bitmapFontImpact.MeasureString(helloWorld);
            var bitmapFontOrigin = (Point2)(bitmapFontSize / 2f);

            _spriteBatch.DrawString(
                bitmapFont: _bitmapFontImpact,
                text: helloWorld,
                position: position + offset,
                color: color,
                rotation: rotation,
                origin: bitmapFontOrigin,
                scale: scale,
                effect: SpriteEffects.None,
                layerDepth: 0);
            
            _spriteBatch.DrawRectangle(position - bitmapFontOrigin + offset, bitmapFontSize, Color.Red);

            var bitmapFontMontserratSize = _bitmapFontMontserrat.MeasureString(helloWorld);
            var bitmapFontMontserratOrigin = bitmapFontMontserratSize / 2f;

            _spriteBatch.DrawString(
                bitmapFont: _bitmapFontMontserrat,
                text: helloWorld,
                position: position + offset * 3,
                color: color,
                rotation: rotation,
                origin: bitmapFontMontserratOrigin,
                scale: scale,
                effect: SpriteEffects.None,
                layerDepth: 0,
                clippingRectangle: _clippingRectangle);

            _spriteBatch.DrawRectangle(_clippingRectangle, Color.White);
            _spriteBatch.DrawRectangle(position - bitmapFontMontserratOrigin + offset * 3, bitmapFontMontserratSize, Color.Green);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}