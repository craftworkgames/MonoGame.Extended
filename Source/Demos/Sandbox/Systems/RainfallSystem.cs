using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Sandbox.Components;

namespace Sandbox.Systems
{
    public class RainfallSystem : EntityUpdateSystem
    {
        private readonly FastRandom _random = new FastRandom();
        private ComponentMapper<Transform2> _transformMapper;
        private ComponentMapper<Raindrop> _raindropMapper;
        private ComponentMapper<Expiry> _expiryMapper;

        private const float _minSpawnDelay = 0.0f;
        private const float _maxSpawnDelay = 0.0f;
        private float _spawnDelay = _maxSpawnDelay;

        public RainfallSystem()
            : base(Aspect.All(typeof(Transform2), typeof(Raindrop)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transformMapper = mapperService.GetMapper<Transform2>();
            _raindropMapper = mapperService.GetMapper<Raindrop>();
            _expiryMapper = mapperService.GetMapper<Expiry>();
        }

        public override void Update(GameTime gameTime)
        {
            var elapsedSeconds = gameTime.GetElapsedSeconds();

            foreach (var entityId in ActiveEntities)
            {
                var transform = _transformMapper.Get(entityId);
                var raindrop = _raindropMapper.Get(entityId);

                raindrop.Velocity += new Vector2(0, 500) * elapsedSeconds;
                transform.Position += raindrop.Velocity * elapsedSeconds;

                if (transform.Position.Y >= 480 && !_expiryMapper.Has(entityId))
                {
                    for (var i = 0; i < 3; i++)
                    {
                        var velocity = new Vector2(_random.NextSingle(-100, 100), -raindrop.Velocity.Y * _random.NextSingle(0.1f, 0.2f));
                        var id = CreateRaindrop(transform.Position.SetY(479), velocity, (i + 1) * 0.5f);
                        _expiryMapper.Put(id, new Expiry(1f));
                    }

                    DestroyEntity(entityId);
                }
            }

            _spawnDelay -= gameTime.GetElapsedSeconds();

            if (_spawnDelay <= 0)
            {
                for (var q = 0; q < 50; q++)
                {
                    var position = new Vector2(_random.NextSingle(0, 800), _random.NextSingle(-240, -480));
                    CreateRaindrop(position);
                }

                _spawnDelay = _random.NextSingle(_minSpawnDelay, _maxSpawnDelay);
            }
        }

        private int CreateRaindrop(Vector2 position, Vector2 velocity = default(Vector2), float size = 3)
        {
            // TODO: entity templates
            // TODO: component pooling
            var entity = CreateEntity();
            entity.Attach(new Transform2(position));
            entity.Attach(new Raindrop { Velocity = velocity, Size = size });
            return entity.Id;
        }
    }
}