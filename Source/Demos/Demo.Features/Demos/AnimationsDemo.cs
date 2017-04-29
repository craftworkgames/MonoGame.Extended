using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace Demo.Features.Demos
{
    public class AnimationsDemo : DemoBase
    {
        public override string Name => "Animations";

        private SpriteBatch _spriteBatch;
        private Zombie _zombie;
        private SpriteSheetAnimation _animation;
        private Sprite _fireballSprite;
        private AnimatedSprite _motwSprite;

        public AnimationsDemo(GameMain game) : base(game)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            //_viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            //_camera = new Camera2D(_viewportAdapter)
            //{
            //    MinimumZoom = 0.1f,
            //    MaximumZoom = 2.0f,
            //    Zoom = 1f,
            //    Origin = new Vector2(400, 240),
            //    Position = new Vector2(408, 270)
            //};
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var zombieAnimations = Content.Load<SpriteSheetAnimationFactory>("Animations/zombie-animations");
            _zombie = new Zombie(zombieAnimations) {Position = new Vector2(100, 100)};

            var fireballTexture = Content.Load<Texture2D>("Animations/fireball");
            var fireballAtlas = TextureAtlas.Create("Animations/fireball-atlas", fireballTexture, 130, 50);
            _animation = new SpriteSheetAnimation("fireballAnimation", fireballAtlas.Regions.ToArray()) { FrameDuration = 0.2f };
            _fireballSprite = new Sprite(_animation.CurrentFrame) { Position = new Vector2(-150, 100) };

            var motwTexture = Content.Load<Texture2D>("Animations/motw");
            var motwAtlas = TextureAtlas.Create("Animations/fireball-atlas", motwTexture, 52, 72);
            var motwAnimationFactory = new SpriteSheetAnimationFactory(motwAtlas);
            motwAnimationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
            motwAnimationFactory.Add("walkSouth", new SpriteSheetAnimationData(new[] { 0, 1, 2, 1 }, isLooping: false));
            motwAnimationFactory.Add("walkWest", new SpriteSheetAnimationData(new[] { 12, 13, 14, 13 }, isLooping: false));
            motwAnimationFactory.Add("walkEast", new SpriteSheetAnimationData(new[] { 24, 25, 26, 25 }, isLooping: false));
            motwAnimationFactory.Add("walkNorth", new SpriteSheetAnimationData(new[] { 36, 37, 38, 37 }, isLooping: false));
            _motwSprite = new AnimatedSprite(motwAnimationFactory) { Position = new Vector2(20, 20) };
            _motwSprite.Play("walkSouth").IsLooping = true;
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();

            // motw
            if (keyboardState.IsKeyDown(Keys.W))
                _motwSprite.Play("walkNorth");

            if (keyboardState.IsKeyDown(Keys.A))
                _motwSprite.Play("walkWest");

            if (keyboardState.IsKeyDown(Keys.S))
                _motwSprite.Play("walkSouth");

            if (keyboardState.IsKeyDown(Keys.D))
                _motwSprite.Play("walkEast");

            // camera
            if (keyboardState.IsKeyDown(Keys.R))
                Camera.ZoomIn(deltaSeconds);

            if (keyboardState.IsKeyDown(Keys.F))
                Camera.ZoomOut(deltaSeconds);

            // zombie
            if (keyboardState.IsKeyDown(Keys.Left))
                _zombie.Walk(-1.0f);

            if (keyboardState.IsKeyDown(Keys.Right))
                _zombie.Walk(1.0f);

            if (keyboardState.IsKeyDown(Keys.Space))
                _zombie.Attack();

            //if (keyboardState.IsKeyDown(Keys.Up))
            //    _zombie.Jump();

            if (keyboardState.IsKeyDown(Keys.Enter))
                _zombie.Die();

            // update must be called before collision detection
            _zombie.Update(gameTime);
            //_world.Update(gameTime);
            Camera.LookAt(_zombie.Position);

            _animation.Update(deltaSeconds);
            _fireballSprite.TextureRegion = _animation.CurrentFrame;

            _motwSprite.Update(deltaSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: Camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
            _zombie.Draw(_spriteBatch);
            _spriteBatch.Draw(_fireballSprite);
            _spriteBatch.End();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.GetViewMatrix());

            _spriteBatch.Draw(_motwSprite);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public enum ZombieState
    {
        None,
        Appearing,
        Idle,
        Walking,
        Attacking,
        Dying
    }

    public class Zombie : IUpdate
    {
        private readonly AnimatedSprite _sprite;
        private float _direction = -1.0f;
        private ZombieState _state;

        public RectangleF BoundingBox => _sprite.BoundingRectangle;
        public bool IsOnGround { get; private set; }
        public bool IsReady => State != ZombieState.Appearing && State != ZombieState.Dying;

        public Vector2 Position
        {
            get { return _sprite.Position; }
            set { _sprite.Position = value; }
        }

        public ZombieState State
        {
            get { return _state; }
            private set
            {
                if (_state != value)
                {
                    _state = value;

                    switch (_state)
                    {
                        case ZombieState.Attacking:
                            _sprite.Play("attack", () => State = ZombieState.Idle);
                            break;
                        case ZombieState.Dying:
                            _sprite.Play("die", () => State = ZombieState.Appearing);
                            break;
                        case ZombieState.Idle:
                            _sprite.Play("idle");
                            break;
                        case ZombieState.Appearing:
                            _sprite.Play("appear", () => State = ZombieState.Idle);
                            break;
                        case ZombieState.Walking:
                            _sprite.Play("walk", () => State = ZombieState.Idle);
                            break;
                    }
                }
            }
        }

        public Vector2 Velocity { get; set; }

        public Zombie(SpriteSheetAnimationFactory animations)
        {
            _sprite = new AnimatedSprite(animations);

            State = ZombieState.Appearing;
            IsOnGround = false;
        }

        public void Update(GameTime gameTime)
        {
            _sprite.Update(gameTime);

            IsOnGround = false;

            if (State == ZombieState.Walking && Math.Abs(Velocity.X) < 0.1f)
                State = ZombieState.Idle;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite);
        }

        public void Walk(float direction)
        {
            _sprite.Effect = _direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            _direction = direction;

            Velocity = new Vector2(200f * _direction, Velocity.Y);

            if (IsReady)
                State = ZombieState.Walking;
        }

        public void Attack()
        {
            if (IsReady)
                State = ZombieState.Attacking;
        }

        public void Die()
        {
            State = ZombieState.Dying;
        }

        public void Jump()
        {
            if (IsReady && IsOnGround)
            {
                State = ZombieState.None;
                Velocity = new Vector2(Velocity.X, -650);
            }
        }
    }
}