using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Tweening.NewTweening;

namespace Tweening
{
    public class MainGame : Game
    {
        public Vector2 Position = new Vector2(100, 100);
        public float Alpha { get; set; } = 1.0f;

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

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _tweener.Create(this, a => a.Position, new Vector2(700, 380), 5);
                //.Easing(Ease.CubeInOut);

            _tweener.Create(this, a => a.Alpha, 0, 5);
            //.Easing(Ease.CubeInOut);
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.FillRectangle(Position.X, Position.Y, 50, 50, new Color(Alpha, Alpha, Alpha));
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
