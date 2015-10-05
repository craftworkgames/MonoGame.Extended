using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace Sandbox
{
    public class Zombie
    {
        public Zombie(TextureAtlas textureAtlas)
        {
            _sprite = new Sprite(textureAtlas[0])
            {
                OriginNormalized = new Vector2(0.5f, 1.0f)
            };
            _animator = new SpriteSheetAnimator(_sprite, textureAtlas);
            _animator.AddAnimation("idle", framesPerSecond: 8, firstFrameIndex: 36, lastFrameIndex: 41);
            _animator.PlayAnimation("idle");
        }

        private readonly Sprite _sprite;
        private readonly SpriteSheetAnimator _animator;

        public Vector2 Position
        {
            get { return _sprite.Position; }
            set { _sprite.Position = value; }
        }

        public void Update(GameTime gameTime)
        {
            _animator.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite);
        }
    }
}