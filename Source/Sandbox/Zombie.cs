using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace Sandbox
{
    public enum ZombieState
    {
        None,
        Appearing,
        Idle,
        Walking,
        Attacking,
        Dying
    }

    public class Zombie : IUpdate, ICollidable
    {
        public Zombie(TextureAtlas textureAtlas)
        {
            _sprite = new Sprite(textureAtlas[0])
            {
                OriginNormalized = new Vector2(0.5f, 1.0f)
            };

            _animator = new SpriteSheetAnimator(_sprite, textureAtlas);
            _animator.AddAnimation("appear", framesPerSecond: 8, firstFrameIndex: 0, lastFrameIndex: 10);
            _animator.AddAnimation("idle", framesPerSecond: 8, firstFrameIndex: 36, lastFrameIndex: 41);
            _animator.AddAnimation("walk", framesPerSecond: 8, firstFrameIndex: 19, lastFrameIndex: 28);
            _animator.AddAnimation("attack", framesPerSecond: 8, firstFrameIndex: 29, lastFrameIndex: 35);
            _animator.AddAnimation("die", framesPerSecond: 8, firstFrameIndex: 11, lastFrameIndex: 18);

            State = ZombieState.Appearing;
        }

        private float _direction = -1.0f;
        private readonly Sprite _sprite;
        private readonly SpriteSheetAnimator _animator;

        private ZombieState _state;
        public ZombieState State
        {
            get {  return _state;}
            private set
            {
                if (_state != value)
                {
                    _state = value;

                    switch (_state)
                    {
                        case ZombieState.Attacking:
                            _animator.PlayAnimation("attack", () => State = ZombieState.Idle);
                            break;
                        case ZombieState.Dying:
                            _animator.PlayAnimation("die", () => State = ZombieState.Appearing);
                            break;
                        case ZombieState.Idle:
                            _animator.PlayAnimation("idle");
                            break;
                        case ZombieState.Appearing:
                            _animator.PlayAnimation("appear", () => State = ZombieState.Idle);
                            break;
                        case ZombieState.Walking:
                            _animator.PlayAnimation("walk", () => State = ZombieState.Idle);
                            break;
                    }
                }
            }
        }

        public bool IsReady
        {
            get { return State != ZombieState.Appearing && State != ZombieState.Dying; }
        }

        public Vector2 Velocity { get; set; }

        public Rectangle GetAxisAlignedBoundingBox()
        {
            return _sprite.GetBoundingRectangle();
        }

        public Vector2 Position
        {
            get { return _sprite.Position; }
            set { _sprite.Position = value; }
        }

        public void Update(GameTime gameTime)
        {
            _animator.Update(gameTime);

            if(State == ZombieState.Walking)
                Position += new Vector2(100f * _direction, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite);
        }

        public void Walk(float direction)
        {
            _sprite.Effect = _direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            _direction = direction;

            if (IsReady)
                State = ZombieState.Walking;
        }

        public void Attack()
        {
            if(IsReady)
                State = ZombieState.Attacking;
        }

        public void Die()
        {
            State = ZombieState.Dying;
        }

        public void Jump()
        {
            if (IsReady)
            {
                State = ZombieState.None;
                Velocity = new Vector2(Velocity.X, -600);
            }
        }
    }
}