using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace Pong.Screens
{
    public class MagentaScreen : GameScreen
    {
        private SpriteBatch _spriteBatch;

        public MagentaScreen(Game game)
            : base(game)
        {
            game.IsMouseVisible = true;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
                ScreenManager.LoadScreen(new PongGameScreen(Game), new ExpandTransition(GraphicsDevice, Color.Black, 0.5f));
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Magenta);

            _spriteBatch.Begin();
            _spriteBatch.FillRectangle(new RectangleF(100, 200, 300, 400), Color.AliceBlue);
            _spriteBatch.End();
        }
    }
}