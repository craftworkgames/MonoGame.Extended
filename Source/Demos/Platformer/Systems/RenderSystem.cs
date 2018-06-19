using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;

namespace Platformer.Systems
{
    public class RenderSystem : EntityDrawSystem
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly OrthographicCamera _camera;
        private ComponentMapper<AnimatedSprite> _animatedSpriteMapper;
        private ComponentMapper<Sprite> _spriteMapper;
        private ComponentMapper<Transform2> _transforMapper;

        public RenderSystem(SpriteBatch spriteBatch, OrthographicCamera camera)
         : base(Aspect.All(typeof(Transform2)).One(typeof(AnimatedSprite), typeof(Sprite)))
        {
            _spriteBatch = spriteBatch;
            _camera = camera;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transforMapper = mapperService.GetMapper<Transform2>();
            _animatedSpriteMapper = mapperService.GetMapper<AnimatedSprite>();
            _spriteMapper = mapperService.GetMapper<Sprite>();
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());

            foreach (var entity in ActiveEntities)
            {
                var sprite = _animatedSpriteMapper.Has(entity)
                    ? _animatedSpriteMapper.Get(entity)
                    : _spriteMapper.Get(entity);
                var transform = _transforMapper.Get(entity);

                if(sprite is AnimatedSprite animatedSprite)
                    animatedSprite.Update(gameTime.GetElapsedSeconds());

                _spriteBatch.Draw(sprite, transform);

            }

            _spriteBatch.End();
        }
    }
}
