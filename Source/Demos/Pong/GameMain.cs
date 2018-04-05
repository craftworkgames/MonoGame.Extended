using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;

namespace Pong
{
    public class GameObject
    {
        public Vector2 Position;
        public Sprite Sprite;
        public RectangleF BoundingRectangle => Sprite.GetBoundingRectangle(Position, 0, Vector2.One);
    }

    public class Paddle : GameObject
    {
    }

    public class Ball : GameObject
    {
        public Vector2 Velocity;
    }

    public class GameMain : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private const int _screenWidth = 480;
        private const int _screenHeight = 800;
        private Paddle _bluePaddle;
        private Paddle _redPaddle;
        private Ball _ball;

        public GameMain()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = _screenWidth,
                PreferredBackBufferHeight = _screenHeight
            };
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _bluePaddle = new Paddle
            {
                Position = new Vector2(_screenWidth / 2f, _screenHeight - 50),
                Sprite = new Sprite(Content.Load<Texture2D>("paddleBlue"))
            };

            _redPaddle = new Paddle
            {
                Position = new Vector2(_screenWidth / 2f, 50),
                Sprite = new Sprite(Content.Load<Texture2D>("paddleRed"))
            };

            _ball = new Ball
            {
                Position = new Vector2(_screenWidth / 2f, _screenHeight / 2f),
                Sprite = new Sprite(Content.Load<Texture2D>("ballGrey")),
                Velocity = new Vector2(50, 150)
            };
        }

        protected override void Update(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _ball.Position += _ball.Velocity * elapsedSeconds;

            _bluePaddle.Position.X = mouseState.Position.X;

            if (ConstrainBounds(_ball))
                _ball.Velocity.X = -_ball.Velocity.X;

            ConstrainBounds(_bluePaddle);
            ConstrainBounds(_redPaddle);

            base.Update(gameTime);
        }

        private static bool ConstrainBounds(GameObject gameObject)
        {
            if (gameObject.BoundingRectangle.Left < 0)
            {
                gameObject.Position.X = gameObject.BoundingRectangle.Width / 2f;
                return true;
            }

            if (gameObject.BoundingRectangle.Right > _screenWidth)
            {
                gameObject.Position.X = _screenWidth - gameObject.BoundingRectangle.Width / 2f;
                return true;
            }

            return false;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_redPaddle.Sprite, _redPaddle.Position);
            _spriteBatch.Draw(_bluePaddle.Sprite, _bluePaddle.Position);
            _spriteBatch.Draw(_ball.Sprite, _ball.Position);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
