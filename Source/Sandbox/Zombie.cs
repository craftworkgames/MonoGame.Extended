using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace Sandbox
{
    public class Zombie : IUpdate
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

            _animator.PlayAnimation("appear", ResumeIdle);
        }

        private float _direction = -1.0f;

        private void ResumeIdle()
        {
            IsWalking = false;
             _animator.PlayAnimation("idle");
        }

        private readonly Sprite _sprite;
        private readonly SpriteSheetAnimator _animator;

        public bool IsWalking { get; private set; }

        public Vector2 Position
        {
            get { return _sprite.Position; }
            set { _sprite.Position = value; }
        }

        public void Update(GameTime gameTime)
        {
            _animator.Update(gameTime);

            if(IsWalking)
                Position += new Vector2(50f * _direction, 0) * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite);
        }

        public void Walk(float direction)
        {
            _sprite.Effect = _direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            _direction = direction;
            IsWalking = true;
            _animator.PlayAnimation("walk", ResumeIdle);
        }

        public void Attack()
        {
            IsWalking = false;
            _animator.PlayAnimation("attack", ResumeIdle);
        }

        public void Die()
        {
            IsWalking = false;
            _animator.PlayAnimation("die", () => _animator.PlayAnimation("appear", ResumeIdle));
        }
    }
}