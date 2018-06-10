using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities.Systems;

namespace MonoGame.Extended.Entities
{
    public class EntityManager : UpdateSystem
    {
        public EntityManager(ComponentManager componentManager)
            : base(Aspect.All())
        {
            _componentManager = componentManager;
            Entities = new Bag<Entity>(128);
        }

        private readonly ComponentManager _componentManager;
        private int _nextId;

        public Bag<Entity> Entities { get; }

        public Entity CreateEntity()
        {
            // TODO: Recycle dead entites
            var id = _nextId++;
            var entity = new Entity(id, _componentManager);
            Entities[id] = entity;
            return entity;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Initialize(ComponentManager componentManager)
        {
        }
    }
}