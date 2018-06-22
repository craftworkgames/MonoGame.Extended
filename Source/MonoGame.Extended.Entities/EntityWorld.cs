using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities.Systems;

namespace MonoGame.Extended.Entities
{
    public class EntityWorld : SimpleDrawableGameComponent
    {
        private readonly Bag<UpdateSystem> _updateSystems;
        private readonly Bag<EntityDrawSystem> _drawSystems;

        public EntityWorld()
        {
            ComponentManager = new ComponentManager();
            EntityManager = new EntityManager(ComponentManager);

            _updateSystems = new Bag<UpdateSystem>
            {
                ComponentManager,
                EntityManager
            };
            _drawSystems = new Bag<EntityDrawSystem>();
        }

        public override void Dispose()
        {
            foreach (var updateSystem in _updateSystems)
                updateSystem.Dispose();

            foreach (var drawSystem in _drawSystems)
                drawSystem.Dispose();

            base.Dispose();
        }

        internal EntityManager EntityManager { get; }
        internal ComponentManager ComponentManager { get; }
        
        public Bag<Entity> AllEntities => EntityManager.Entities;

        // TODO: Move this to world configuration
        public void RegisterSystem(UpdateSystem system)
        {
            switch (system)
            {
                case EntityDrawSystem drawSystem:
                    _drawSystems.Add(drawSystem);
                    break;
                case UpdateSystem updateSystem:
                    _updateSystems.Add(updateSystem);
                    break;
            }

            if (system is EntityUpdateSystem entitySystem)
            {
                entitySystem.World = this;
                entitySystem.Initialize(ComponentManager);
            }
        }

        public Entity GetEntity(int entityId)
        {
            return AllEntities[entityId];
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