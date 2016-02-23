using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;

namespace Demo.SpriteSheetAnimations
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

    public class Zombie : IUpdate, IActorTarget
    {
        public Zombie(SpriteSheetAnimationGroup animationGroup)
        {
            _animator = new SpriteSheetAnimator(animationGroup);
            _sprite = _animator.Sprite;

            State = ZombieState.Appearing;
            IsOnGround = false;
        }

        private float _direction = -1.0f;
        private readonly Sprite _sprite;
        private readonly SpriteSheetAnimator _animator;

        public bool IsOnGround { get; private set; }

        private ZombieState _state;
        public ZombieState State
        {
            get { return _state;}
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

        public bool IsReady => State != ZombieState.Appearing && State != ZombieState.Dying;

        public Vector2 Velocity { get; set; }

        public Vector2 Position
        {
            get { return _sprite.Position; }
            set { _sprite.Position = value; }
        }

        public RectangleF BoundingBox => _sprite.GetBoundingRectangle();

        public void Update(GameTime gameTime)
        {
            _animator.Update(gameTime);

            IsOnGround = false;

            if(State == ZombieState.Walking && Math.Abs(Velocity.X) < 0.1f)
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
            if(IsReady)
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

        public void OnCollision(CollisionInfo c)
        {
            Position -= c.PenetrationVector;
            Velocity = Vector2.Zero;
            //if (c.PenetrationVector.X != 0)
            //{
            //    Position -= c.PenetrationVector;
            //    Velocity = new Vector2(0, Velocity.Y);
            //}
            //else
            //{
            //    if (Velocity.Y > 0)
            //        IsOnGround = true;

            //    var d = Position.Y < c.Other.BoundingBox.Center.Y
            //        ? c.IntersectingRectangle.Height
            //        : -c.IntersectingRectangle.Height;
            //    Position = new Vector2(Position.X, Position.Y - d);
            //    Velocity = new Vector2(Velocity.X, 0);
            //}
        }
    }
}