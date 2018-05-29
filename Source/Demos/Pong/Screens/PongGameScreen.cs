using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tweening;
using Pong.GameObjects;

namespace Pong.Screens
{
    public class PongGameScreen : GameScreen
    {
        private SpriteBatch _spriteBatch;
        private Paddle _bluePaddle;
        private Paddle _redPaddle;
        private Ball _ball;
        private Texture2D _court;
        private BitmapFont _font;
        private SoundEffect _plopSoundEffect;
        private int _leftScore;
        private int _rightScore;
        private readonly FastRandom _random = new FastRandom();
        private Tweener _tweener = new Tweener();

        public PongGameScreen(Game game)
            : base(game)
        {
            game.IsMouseVisible = false;
        }

        public int ScreenWidth => GraphicsDevice.Viewport.Width;
        public int ScreenHeight => GraphicsDevice.Viewport.Height;

        public override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _plopSoundEffect = Content.Load<SoundEffect>("pip");
            _font = Content.Load<BitmapFont>("kenney-rocket-square");
            _court = Content.Load<Texture2D>("court");

            _bluePaddle = new Paddle
            {
                Position = new Vector2(50, ScreenWidth / 2f),
                Sprite = new Sprite(Content.Load<Texture2D>("paddleBlue"))
            };

            _redPaddle = new Paddle
            {
                Position = new Vector2(ScreenWidth - 50, ScreenHeight / 2f),
                Sprite = new Sprite(Content.Load<Texture2D>("paddleRed"))
            };

            _ball = new Ball
            {
                Position = new Vector2(ScreenWidth / 2f, ScreenHeight / 2f),
                Sprite = new Sprite(Content.Load<Texture2D>("ballGrey")),
                Velocity = new Vector2(250, 200)
            };
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            _spriteBatch.Dispose();
        }

        public override void Update(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();
            var mouseState = MouseExtended.GetState();
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.WasKeyJustDown(Keys.Escape))
                ScreenManager.LoadScreen(new TitleScreen(Game), new ExpandTransition(GraphicsDevice, Color.Black));

            MovePaddlePlayer(mouseState);

            MovePaddleAi(_redPaddle, elapsedSeconds);

            ConstrainPaddle(_bluePaddle);
            ConstrainPaddle(_redPaddle);

            MoveBall(elapsedSeconds);

            if (BallHitPaddle(_ball, _bluePaddle))
            {
                // TODO: Change the angle of the bounce
                //_tweener.TweenTo(_bluePaddle, p => p.Rotation, MathHelper.Pi / 16f, 0.2f)
                //    //.OnSet(v => new Vector2(v.X, _bluePaddle.Position.Y))
                //    .RepeatReverse()
                //    .Easing(EasingFunctions.ExponentialIn);
                    
                _plopSoundEffect.Play(1.0f, _random.NextSingle(0.5f, 1.0f), -1f);
            }

            if (BallHitPaddle(_ball, _redPaddle))
            {
                // TODO: Change the angle of the bounce

                //_tweener.TweenTo(_redPaddle, p => p.Position, _redPaddle.Position + new Vector2(15, 0), 0.2f)
                //    .RepeatReverse()
                //    .Easing(EasingFunctions.ExponentialIn);

                _plopSoundEffect.Play(1f, _random.NextSingle(-1f, 1f), 1f);
            }

            _tweener.Update(elapsedSeconds);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_court, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);

            DrawScores();

            _spriteBatch.Draw(_redPaddle.Sprite, _redPaddle.Position, _redPaddle.Rotation, _redPaddle.Scale);
            _spriteBatch.Draw(_bluePaddle.Sprite, _bluePaddle.Position, _bluePaddle.Rotation, _bluePaddle.Scale);
            _spriteBatch.Draw(_ball.Sprite, _ball.Position, _ball.Rotation, _ball.Scale); 
            _spriteBatch.End();
        }

        private void DrawScores()
        {
            _spriteBatch.DrawString(_font, $"{_leftScore:00}", new Vector2(172, 10), new Color(0.2f, 0.2f, 0.2f), 0, Vector2.Zero, Vector2.One * 4f, SpriteEffects.None, 0);
            _spriteBatch.DrawString(_font, $"{_rightScore:00}", new Vector2(430, 10), new Color(0.2f, 0.2f, 0.2f), 0, Vector2.Zero, Vector2.One * 4f, SpriteEffects.None, 0);
        }

        private void MovePaddlePlayer(MouseStateExtended mouseState)
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

            if (_ball.Position.Y + halfHeight > ScreenHeight)
            {
                _ball.Position.Y = ScreenHeight - halfHeight;
                _ball.Velocity.Y = -_ball.Velocity.Y;
            }

            // left and right is out of bounds 
            // TODO: Play sound and update score
            // TODO: Reset ball to default velocity
            if (_ball.Position.X > ScreenWidth + halfWidth && _ball.Velocity.X > 0)
            {
                _ball.Position = new Vector2(ScreenWidth / 2f, ScreenHeight / 2f);
                _ball.Velocity = new Vector2(_random.Next(2, 5) * -100, 100);
                _leftScore++;
            }

            if (_ball.Position.X < -halfWidth && _ball.Velocity.X < 0)
            {
                _ball.Position = new Vector2(ScreenWidth / 2f, ScreenHeight / 2f);
                _ball.Velocity = new Vector2(_random.Next(2, 5) * 100, 100);
                _rightScore++;
            }
        }

        private void ConstrainPaddle(Paddle paddle)
        {
            if (paddle.BoundingRectangle.Left < 0)
                paddle.Position.X = paddle.BoundingRectangle.Width / 2f;

            if (paddle.BoundingRectangle.Right > ScreenWidth)
                paddle.Position.X = ScreenWidth - paddle.BoundingRectangle.Width / 2f;

            if (paddle.BoundingRectangle.Top < 0)
                paddle.Position.Y = paddle.BoundingRectangle.Height / 2f;

            if (paddle.BoundingRectangle.Bottom > ScreenHeight)
                paddle.Position.Y = ScreenHeight - paddle.BoundingRectangle.Height / 2f;
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
    }
}
