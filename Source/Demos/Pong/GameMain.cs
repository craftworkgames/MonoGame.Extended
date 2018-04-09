using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
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

        private const int _screenWidth = 800;
        private const int _screenHeight = 480;
        private Paddle _bluePaddle;
        private Paddle _redPaddle;
        private Ball _ball;
        private Texture2D _court;
        private BitmapFont _font;
        private int _leftScore;
        private int _rightScore;
        private readonly FastRandom _random = new FastRandom();

        private SoundEffect _plopSoundEffect;

        public GameMain()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = _screenWidth,
                PreferredBackBufferHeight = _screenHeight,
                SynchronizeWithVerticalRetrace = false
            };
            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _plopSoundEffect = Content.Load<SoundEffect>("pip");

            _font = Content.Load<BitmapFont>("kenney-rocket-square");

            _court = Content.Load<Texture2D>("court");

            _bluePaddle = new Paddle
            {
                Position = new Vector2(50, _screenWidth / 2f),
                Sprite = new Sprite(Content.Load<Texture2D>("paddleBlue"))
            };

            _redPaddle = new Paddle
            {
                Position = new Vector2(_screenWidth - 50, _screenHeight / 2f),
                Sprite = new Sprite(Content.Load<Texture2D>("paddleRed"))
            };

            _ball = new Ball
            {
                Position = new Vector2(_screenWidth / 2f, _screenHeight / 2f),
                Sprite = new Sprite(Content.Load<Texture2D>("ballGrey")),
                Velocity = new Vector2(250, 200)
            };
        }

        protected override void Update(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            MovePaddlePlayer(mouseState);

            MovePaddleAi(_redPaddle, elapsedSeconds);

            ConstrainPaddle(_bluePaddle);
            ConstrainPaddle(_redPaddle);
            
            MoveBall(elapsedSeconds);

            if (BallHitPaddle(_ball, _bluePaddle))
            {
                // TODO: Play pong sound
                // TODO: Change the angle of the bounce
                _ball.Velocity *= 1.05f;
                _plopSoundEffect.Play(1.0f, _random.NextSingle(0.5f, 1.0f), -1f);
            }

            if (BallHitPaddle(_ball, _redPaddle))
            {
                // TODO: Play ping sound
                // TODO: Change the angle of the bounce
                _ball.Velocity *= 1.05f;
                _plopSoundEffect.Play(1f, _random.NextSingle(-1f, 1f), 1f);
            }

            base.Update(gameTime);
        }

        private void MovePaddlePlayer(MouseState mouseState)
        {
            _bluePaddle.Position.Y = mouseState.Position.Y;
        }

        private static bool BallHitPaddle(Ball ball, Paddle paddle)
        {
            if (ball.BoundingRectangle.Intersects(paddle.BoundingRectangle))
            {
                if (ball.BoundingRectangle.Left < paddle.BoundingRectangle.Left)
                    ball.Position.X = paddle.BoundingRectangle.Left - ball.BoundingRectangle.Width / 2;

                if (ball.BoundingRectangle.Right > paddle.BoundingRectangle.Right)
                    ball.Position.X = paddle.BoundingRectangle.Right + ball.BoundingRectangle.Width / 2;
                
                ball.Velocity.X = -ball.Velocity.X;
                return true;
            }

            return false;
        }

        private void MoveBall(float elapsedSeconds)
        {
            _ball.Position += _ball.Velocity * elapsedSeconds;

            var halfHeight = _ball.BoundingRectangle.Height / 2;
            var halfWidth = _ball.BoundingRectangle.Width / 2;

            // top and bottom walls
            // TODO: Play 'tink' sound
            if (_ball.Position.Y - halfHeight < 0)
            {
                _ball.Position.Y = halfHeight;
                _ball.Velocity.Y = -_ball.Velocity.Y;
            }

            if (_ball.Position.Y + halfHeight > _screenHeight)
            {
                _ball.Position.Y = _screenHeight - halfHeight;
                _ball.Velocity.Y = -_ball.Velocity.Y;
            }

            // left and right is out of bounds 
            // TODO: Play sound and update score
            // TODO: Reset ball to default velocity
            if (_ball.Position.X > _screenWidth + halfWidth && _ball.Velocity.X > 0)
            {
                _ball.Position = new Vector2(_screenWidth / 2f, _screenHeight / 2f);
                _ball.Velocity = new Vector2(_random.Next(2, 5) * -100, 100);
                _leftScore++;
            }

            if (_ball.Position.X < -halfWidth && _ball.Velocity.X < 0)
            {
                _ball.Position = new Vector2(_screenWidth / 2f, _screenHeight / 2f);
                _ball.Velocity = new Vector2(_random.Next(2, 5) * 100, 100);
                _rightScore++;
            }
        }

        private static void ConstrainPaddle(Paddle paddle)
        {
            if (paddle.BoundingRectangle.Left < 0)
                paddle.Position.X = paddle.BoundingRectangle.Width / 2f;

            if (paddle.BoundingRectangle.Right > _screenWidth)
                paddle.Position.X = _screenWidth - paddle.BoundingRectangle.Width / 2f;

            if (paddle.BoundingRectangle.Top < 0)
                paddle.Position.Y = paddle.BoundingRectangle.Height / 2f;

            if (paddle.BoundingRectangle.Bottom > _screenHeight)
                paddle.Position.Y = _screenHeight - paddle.BoundingRectangle.Height / 2f;
        }

        private void MovePaddleAi(Paddle paddle, float elapsedSeconds)
        {
            const float difficulty = 0.80f;
            var paddleSpeed = Math.Abs(_ball.Velocity.Y) * difficulty;

            if (paddleSpeed < 0)
                paddleSpeed = -paddleSpeed;

            //ball moving down
            if (_ball.Velocity.Y > 0)
            {
                if (_ball.Position.Y > paddle.Position.Y)
                    paddle.Position.Y += paddleSpeed * elapsedSeconds;
                else
                    paddle.Position.Y -= paddleSpeed * elapsedSeconds;
            }

            //ball moving up
            if (_ball.Velocity.Y < 0)
            {
                if (_ball.Position.Y < paddle.Position.Y)
                    paddle.Position.Y -= paddleSpeed * elapsedSeconds;
                else
                    paddle.Position.Y += paddleSpeed * elapsedSeconds;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_court, new Rectangle(0, 0, _screenWidth, _screenHeight), Color.White);

            DrawScores();

            _spriteBatch.Draw(_redPaddle.Sprite, _redPaddle.Position);
            _spriteBatch.Draw(_bluePaddle.Sprite, _bluePaddle.Position); 
            _spriteBatch.Draw(_ball.Sprite, _ball.Position);
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }

        private void DrawScores()
        {
            _spriteBatch.DrawString(_font, $"{_leftScore:00}", new Vector2(172, 10), new Color(0.2f, 0.2f, 0.2f), 0, Vector2.Zero, Vector2.One * 4f, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_font, $"{_rightScore:00}", new Vector2(430, 10), new Color(0.2f, 0.2f, 0.2f), 0, Vector2.Zero, Vector2.One * 4f, SpriteEffects.None, 0);
        }
    }
}
