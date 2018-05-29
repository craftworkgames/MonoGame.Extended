using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Tweening;
using MonoGame.Extended.BitmapFonts;

namespace Tweening
{
    public class MainGame : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private readonly Tweener _tweener = new Tweener();
        private BitmapFont _bitmapFont;

        public MainGame()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);
        }

        public Vector2 Linear = new Vector2(200, 50);
        public Vector2 Quadratic = new Vector2(200, 100);
        public Vector2 Exponential = new Vector2(200, 150);
        public Vector2 Bounce = new Vector2(200, 200);
        public Vector2 Back = new Vector2(200, 250);
        public Vector2 Elastic = new Vector2(200, 300);
        public Vector2 Size = new Vector2(50, 50);

        protected override void LoadContent()
        {
            _bitmapFont = Content.Load<BitmapFont>("kenney-rocket-square");

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _tweener.TweenTo(this, a => a.Linear, new Vector2(550, 50), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 0.2f)
                .AutoReverse()
                .Easing(EasingFunctions.Linear);

            _tweener.TweenTo(this, a => a.Quadratic, new Vector2(550, 100), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 0.2f)
                .AutoReverse()
                .Easing(EasingFunctions.QuadraticInOut);

            _tweener.TweenTo(this, a => a.Exponential, new Vector2(550, 150), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 0.2f)
                .AutoReverse()
                .Easing(EasingFunctions.ExponentialInOut);

            _tweener.TweenTo(this, a => a.Bounce, new Vector2(550, 200), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 0.2f)
                .AutoReverse()
                .Easing(EasingFunctions.BounceOut);

            _tweener.TweenTo(this, a => a.Back, new Vector2(550, 250), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 0.2f)
                .AutoReverse()
                .Easing(EasingFunctions.BackOut);

            _tweener.TweenTo(this, a => a.Elastic, new Vector2(550, 300), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 0.2f)
                .AutoReverse()
                .Easing(EasingFunctions.ElasticOut);
        }

        protected override void UnloadContent()
        {
            _spriteBatch.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = KeyboardExtended.GetState();
            var mouseState = MouseExtended.GetState();
            var elapsedSeconds = gameTime.GetElapsedSeconds();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (keyboardState.WasKeyJustDown(Keys.Space))
                _tweener.CancelAll();

            if (keyboardState.WasKeyJustDown(Keys.Tab))
                _tweener.CancelAndCompleteAll();

            if (mouseState.IsButtonDown(MouseButton.Left))
            {
                _tweener.TweenTo(this, a => a.Linear, mouseState.Position.ToVector2(), 1.0f)
                    .Easing(EasingFunctions.QuadraticOut);
            }

            _tweener.Update(elapsedSeconds);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _spriteBatch.FillRectangle(Linear.X, Linear.Y, Size.X, Size.X, Color.Red);
            _spriteBatch.FillRectangle(Quadratic.X, Quadratic.Y, Size.X, Size.X, Color.Green);
            _spriteBatch.FillRectangle(Exponential.X, Exponential.Y, Size.X, Size.X, Color.Blue);
            _spriteBatch.FillRectangle(Bounce.X, Bounce.Y, Size.X, Size.X, Color.DarkOrange);
            _spriteBatch.FillRectangle(Back.X, Back.Y, Size.X, Size.X, Color.Purple);
            _spriteBatch.FillRectangle(Elastic.X, Elastic.Y, Size.X, Size.X, Color.Yellow);

            _spriteBatch.DrawString(_bitmapFont, $"{_tweener.AllocationCount}", Vector2.One, Color.WhiteSmoke);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
