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
    public class PlayerControlSystem : EntityUpdateSystem
    {
        private readonly MainGame _mainGame;
        private readonly EntityFactory _entityFactory;
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Sprite> _spriteMapper;
        private const int _playerSpeed = 64;
        private KeyboardState _previousKeyboardState;

        private Entity _spawningFireball;

        public PlayerControlSystem(MainGame mainGame, EntityFactory entityFactory) 
            : base(Aspect.All(typeof(Player), typeof(Transform2), typeof(Sprite)))
        {
            _mainGame = mainGame;
            _entityFactory = entityFactory;
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

                if (_previousKeyboardState.IsKeyUp(Keys.Space) && _mainGame.KeyboardState.IsKeyDown(Keys.Space))
                    _spawningFireball = _entityFactory.SpawnFireball(transform.Position.X + 16, transform.Position.Y);

                // Kinda hacky but it works for now.
                var spawningFireballBody = _spawningFireball?.Get<Body>();
                if (spawningFireballBody != null && _previousKeyboardState.IsKeyDown(Keys.Space) && _mainGame.KeyboardState.IsKeyUp(Keys.Space))
                {
                    spawningFireballBody.Velocity = new Vector2(200, 0);
                    _spawningFireball = null;
                }

                var spawningFireballTransform = _spawningFireball?.Get<Transform2>();
                if(spawningFireballTransform != null)
                    spawningFireballTransform.Position = new Vector2(transform.Position.X + 16, transform.Position.Y);
            }

            _previousKeyboardState = _mainGame.KeyboardState;
        }
    }
}