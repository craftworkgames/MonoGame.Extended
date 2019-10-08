using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace Tutorials.Demos
{
    public class AnimationsDemo : DemoBase
    {
        public override string Name => "Animations";

        private SpriteBatch _spriteBatch;
        //private Zombie _zombie;
        //private SpriteSheetAnimation _animation;
        //private AnimatedSprite _zombie;
        private AnimatedSprite _fireballSprite;
        private AnimatedSprite _motwSprite;
        public Vector2 _motwPosition;

        public AnimationsDemo(GameMain game) : base(game)
        {
            ContentTypeReaderManager.AddTypeCreator("TextureAtlas", () => new TextureAtlasJsonContentTypeReader());
            ContentTypeReaderManager.AddTypeCreator("Default", () => new JsonContentTypeReader<TexturePackerFile>());
        }

        //protected override void Initialize()
        //{
        //    base.Initialize();

        //    //_viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
        //    //_camera = new OrthographicCamera(_viewportAdapter)
        //    //{
        //    //    MinimumZoom = 0.1f,
        //    //    MaximumZoom = 2.0f,
        //    //    Zoom = 1f,
        //    //    Origin = new Vector2(400, 240),
        //    //    Position = new Vector2(408, 270)
        //    //};
        //}

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //var zombieAnimations = Content.Load<SpriteSheetAnimationFactory>("Animations/zombie-animations");
            //_zombie = new Zombie(zombieAnimations) { Position = new Vector2(100, 100) };

            _fireballSprite = MakeFireball();

            //_animation = new SpriteSheetAnimation("fireballAnimation", fireballAtlas.Regions.ToArray()) { FrameDuration = 0.2f };
            //_fireballSprite = new Sprite(_animation.CurrentFrame);// { Position = new Vector2(-150, 100) };

            //var motwTexture = Content.Load<Texture2D>("Animations/motw");
            //var motwAtlas = TextureAtlas.Create("Animations/fireball-atlas", motwTexture, 52, 72);
            //var motwAnimationFactory = new SpriteSheetAnimationFactory(motwAtlas);
            //motwAnimationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
            //motwAnimationFactory.Add("walkSouth", new SpriteSheetAnimationData(new[] { 0, 1, 2, 1 }, isLooping: false));
            //motwAnimationFactory.Add("walkWest", new SpriteSheetAnimationData(new[] { 12, 13, 14, 13 }, isLooping: false));
            //motwAnimationFactory.Add("walkEast", new SpriteSheetAnimationData(new[] { 24, 25, 26, 25 }, isLooping: false));
            //motwAnimationFactory.Add("walkNorth", new SpriteSheetAnimationData(new[] { 36, 37, 38, 37 }, isLooping: false));

            //_zombie = MakeZombie();
            _motwSprite = MakeMotw();
        }

        private AnimatedSprite MakeZombie()
        {
            var spriteSheet = Content.Load<SpriteSheet>("Animations/zombie.spritesheet", new JsonContentLoader());
            var sprite = new AnimatedSprite(spriteSheet);
            sprite.Play("idle");
            return sprite;
        }

        private AnimatedSprite MakeMotw()
        {
            var spriteSheet = Content.Load<SpriteSheet>("Animations/motw.sf", new JsonContentLoader());
            var sprite = new AnimatedSprite(spriteSheet);
            sprite.Play("idle");

            _motwPosition = new Vector2(100, 100);

            return sprite;
        }

        private AnimatedSprite MakeFireball()
        {
            // in this example, we're making an animated fireball manually in code.
            // after the texture is loaded, we can split it up into it's different sprites using texture atlas.
            // then we make a sprite sheet do define the frames of the animation.
            var texture = Content.Load<Texture2D>("Animations/fireball");
            var textureAtlas = TextureAtlas.Create("Animations/fireball-atlas", texture, 130, 50);
            var spriteSheet = new SpriteSheet
            {
                TextureAtlas = textureAtlas,
                Cycles =
                {
                    {
                        "flaming", new SpriteSheetAnimationCycle
                        {
                            IsLooping = true,
                            IsPingPong = true,
                            FrameDuration = 0.2f,
                            Frames =
                            {
                                // TODO: Fix per frame duration
                                new SpriteSheetAnimationFrame(0, duration: 0.1f),
                                new SpriteSheetAnimationFrame(1, duration: 0.15f),
                                new SpriteSheetAnimationFrame(2, duration: 0.3f)
                            }
                        }
                    }
                }
            };
            return new AnimatedSprite(spriteSheet, "flaming");
        }

        protected override void Update(GameTime gameTime)
        {
            var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var walkSpeed = deltaSeconds * 128;
            var keyboardState = Keyboard.GetState();
            var animation = "idle";

            // motw
            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            {
                animation = "walkNorth";
                _motwPosition.Y -= walkSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
            {
                animation = "walkSouth";
                _motwPosition.Y += walkSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
            {
                animation = "walkWest";
                _motwPosition.X -= walkSpeed;
            }

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
            {
                animation = "walkEast";
                _motwPosition.X += walkSpeed;
            }

            _motwSprite.Play(animation);

            // camera
            if (keyboardState.IsKeyDown(Keys.R))
                Camera.ZoomIn(deltaSeconds);

            if (keyboardState.IsKeyDown(Keys.F))
                Camera.ZoomOut(deltaSeconds);

            //// zombie
            //if (keyboardState.IsKeyDown(Keys.Left))
            //    _zombie.Walk(-1.0f);

            //if (keyboardState.IsKeyDown(Keys.Right))
            //    _zombie.Walk(1.0f);

            //if (keyboardState.IsKeyDown(Keys.Space))
            //    _zombie.Attack();

            ////if (keyboardState.IsKeyDown(Keys.Up))
            ////    _zombie.Jump();

            //if (keyboardState.IsKeyDown(Keys.Enter))
            //    _zombie.Die();

            //// update must be called before collision detection
            //_zombie.Update(gameTime);
            ////_world.Update(gameTime);
            //Camera.LookAt(_zombie.Position);

            //_animation.Update(deltaSeconds);
            //_fireballSprite.TextureRegion = _animation.CurrentFrame;

            //_zombie.Update(deltaSeconds);
            _motwSprite.Update(deltaSeconds);
            _fireballSprite.Update(deltaSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //_spriteBatch.Begin(transformMatrix: Camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
            //_zombie.Draw(_spriteBatch);
            ////_spriteBatch.Draw(_fireballSprite);
            //_spriteBatch.End();

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.GetViewMatrix());
            _spriteBatch.Draw(_motwSprite, _motwPosition);
            _spriteBatch.Draw(_fireballSprite, new Vector2(200, 100));
            //_spriteBatch.Draw(_zombie, new Vector2(300, 100));
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
        private Transform2 _transform;

        public RectangleF BoundingBox => _sprite.GetBoundingRectangle(_transform.Position, _transform.Rotation, _transform.Scale);
        public bool IsOnGround { get; private set; }
        public bool IsReady => State != ZombieState.Appearing && State != ZombieState.Dying;

        public Vector2 Position
        {
            get => _transform.Position;
            set => _transform.Position = value;
        }

        public ZombieState State
        {
            get => _state;
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

        public Zombie(SpriteSheet spriteSheet)
        {
            _sprite = new AnimatedSprite(spriteSheet);
            _transform = new Transform2();

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

        //public void Draw(SpriteBatch spriteBatch)
        //{
        //    spriteBatch.Draw(_sprite, _transform);
        //}

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