using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities.Systems;

namespace MonoGame.Extended.Entities
{
    public class EntityWorld : SimpleDrawableGameComponent
    {
        // TODO: Make these private again
        public EntityManager EntityManager { get; }
        public ComponentManager ComponentManager { get; }

        private readonly Bag<UpdateSystem> _updateSystems;
        private readonly Bag<DrawSystem> _drawSystems;

        public EntityWorld()
        {
            ComponentManager = new ComponentManager();
            EntityManager = new EntityManager(ComponentManager);

            _updateSystems = new Bag<UpdateSystem>()
            {
                ComponentManager,
                EntityManager
            };
            _drawSystems = new Bag<DrawSystem>();
        }

        // TODO: Move this to world configuration
        public void RegisterSystem(BaseSystem system)
        {
            switch (system)
            {
                case DrawSystem drawSystem:
                    _drawSystems.Add(drawSystem);
                    break;
                case UpdateSystem updateSystem:
                    _updateSystems.Add(updateSystem);
                    break;
            }

            system.World = this;
            system.Initialize(ComponentManager);
        }

        public Entity CreateEntity()
        {
            return EntityManager.CreateEntity();
        }

        public void DestroyEntity(int entityId)
        {
            EntityManager.DestroyEntity(entityId);
        }

        public void DestroyEntity(Entity entity)
        {
            EntityManager.DestroyEntity(entity);
        }
        
        public override void Update(GameTime gameTime)
        {
            foreach (var system in _updateSystems)
                system.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var system in _drawSystems)
                system.Draw(gameTime);
        }
    }
}