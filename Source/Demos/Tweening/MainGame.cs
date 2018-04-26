using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;

namespace Tweening
{
    public class MainGame : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private readonly Tweener _tweener = new Tweener();

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

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _tweener.Create(this, a => a.Linear, new Vector2(550, 50), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 1)
                .AutoReverse()
                .Easing(EasingFunctions.Linear);

            _tweener.Create(this, a => a.Quadratic, new Vector2(550, 100), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 1)
                .AutoReverse()
                .Easing(EasingFunctions.QuadraticInOut);

            _tweener.Create(this, a => a.Exponential, new Vector2(550, 150), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 1)
                .AutoReverse()
                .Easing(EasingFunctions.ExponentialInOut);

            _tweener.Create(this, a => a.Bounce, new Vector2(550, 200), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 1)
                .AutoReverse()
                .Easing(EasingFunctions.BounceOut);

            _tweener.Create(this, a => a.Back, new Vector2(550, 250), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 1)
                .AutoReverse()
                .Easing(EasingFunctions.BackOut);

            _tweener.Create(this, a => a.Elastic, new Vector2(550, 300), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 1)
                .AutoReverse()
                .Easing(EasingFunctions.ElasticOut);
        }

        protected override void UnloadContent()
        {
            _spriteBatch.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _tweener.Update(gameTime.GetElapsedSeconds());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.FillRectangle(Linear.X, Linear.Y, 40, 40, Color.White);
            _spriteBatch.FillRectangle(Quadratic.X, Quadratic.Y, 40, 40, Color.White);
            _spriteBatch.FillRectangle(Exponential.X, Exponential.Y, 40, 40, Color.White);
            _spriteBatch.FillRectangle(Bounce.X, Bounce.Y, 40, 40, Color.White);
            _spriteBatch.FillRectangle(Back.X, Back.Y, 40, 40, Color.White);
            _spriteBatch.FillRectangle(Elastic.X, Elastic.Y, 40, 40, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
