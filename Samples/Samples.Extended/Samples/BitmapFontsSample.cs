using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;

namespace Samples.Extended.Samples
{
    public class BitmapFontsSample : Game
    {
        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private BitmapFont _bitmapFont;

        public BitmapFontsSample()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _bitmapFont = Content.Load<BitmapFont>("courier-new-32");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_bitmapFont, "MonoGame.Extended BitmapFont Sample", new Vector2(50, 10), Color.White);
            _spriteBatch.DrawString(_bitmapFont,
                "Contrary to popular belief, Lorem Ipsum is not simply random text.\n\n" +
                "It has roots in a piece of classical Latin literature from 45 BC, making it over 2000 years old. Richard " +
                "McClintock, a Latin professor at Hampden-Sydney College in Virginia, looked up one of the more obscure Latin " +
                "words, consectetur, from a Lorem Ipsum passage, and going through the cites of the word in classical literature, " +
                "discovered the undoubtable source.", new Vector2(50, 50), new Color(Color.Black, 0.5f), 
                wrapWidth: 750);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
