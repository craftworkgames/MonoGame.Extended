using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;

namespace JamGame.Systems
{
    public class SpriteRenderingSystem : EntityDrawSystem
    {
        private readonly SpriteBatch _spriteBatch;

        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Sprite> _spriteMapper;
        private ComponentMapper<AnimatedSprite> _animatedSpriteMapper;
        
        public SpriteRenderingSystem(GraphicsDevice graphicsDevice, Texture2D texture)
            : base(Aspect.All(typeof(Transform2)).One(typeof(Sprite), typeof(AnimatedSprite)))
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _spriteMapper = mapperService.GetMapper<Sprite>();
            _animatedSpriteMapper = mapperService.GetMapper<AnimatedSprite>();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Matrix.CreateScale(2));

            foreach (var entity in ActiveEntities)
            {
                var transform = _transformMapper.Get(entity);
                var animatedSprite = _animatedSpriteMapper.Get(entity);
                var sprite = _spriteMapper.Get(entity);

                animatedSprite?.Update(gameTime);

                _spriteBatch.Draw(sprite ?? animatedSprite, transform);
            }

            _spriteBatch.End();
        }
    }
}