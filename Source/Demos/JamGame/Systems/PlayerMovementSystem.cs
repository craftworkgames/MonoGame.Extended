using JamGame.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;

namespace JamGame.Systems
{
    public class PlayerMovementSystem : EntityUpdateSystem
    {
        private readonly MainGame _mainGame;
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Sprite> _spriteMapper;
        private const int _playerSpeed = 64;

        public PlayerMovementSystem(MainGame mainGame) 
            : base(Aspect.All(typeof(Player), typeof(Transform2), typeof(Sprite)))
        {
            _mainGame = mainGame;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _spriteMapper = mapperService.GetMapper<Sprite>();
        }
        
        public override void Update(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();

            foreach (var entity in ActiveEntities)
            {
                var transform = _transformMapper.Get(entity);
                var sprite = _spriteMapper.Get(entity);

                if (_mainGame.KeyboardState.IsKeyDown(Keys.Left))
                {
                    transform.Position += new Vector2(-_playerSpeed, 0) * elapsedSeconds;
                    sprite.Effect = SpriteEffects.FlipHorizontally;
                }

                if (_mainGame.KeyboardState.IsKeyDown(Keys.Right))
                {
                    transform.Position += new Vector2(_playerSpeed, 0) * elapsedSeconds;
                    sprite.Effect = SpriteEffects.None;
                }

                if (_mainGame.KeyboardState.IsKeyDown(Keys.Up))
                    transform.Position += new Vector2(0, -_playerSpeed) * elapsedSeconds;

                if (_mainGame.KeyboardState.IsKeyDown(Keys.Down))
                    transform.Position += new Vector2(0, _playerSpeed) * elapsedSeconds;
            }
        }
    }
}