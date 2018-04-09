using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;

namespace Pong.Screens
{
    public class MagentaScreen : Screen
    {
        private readonly Game _game;
        private SpriteBatch _spriteBatch;

        public MagentaScreen(Game game)
        {
            _game = game;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
                ScreenManager.LoadScreen(new GameScreen(_game));
        }

        public override void Draw(GameTime gameTime)
        {
            _game.GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _spriteBatch.FillRectangle(new RectangleF(100, 100, 100, 100), Color.Magenta);
            _spriteBatch.End();
        }
    }
}