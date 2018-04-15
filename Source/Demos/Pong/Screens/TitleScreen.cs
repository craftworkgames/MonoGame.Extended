using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace Pong.Screens
{
    public class TitleScreen : GameScreen
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _background;

        public TitleScreen(Game game)
            : base(game)
        {
            game.IsMouseVisible = true;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _background = Content.Load<Texture2D>("title-screen");
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
                ScreenManager.LoadScreen(new PongGameScreen(Game), new FadeTransition(GraphicsDevice, Color.Black, 0.5f));
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Magenta);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_background, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            _spriteBatch.End();
        }
    }
}