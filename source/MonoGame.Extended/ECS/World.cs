using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.ECS.Systems;

namespace MonoGame.Extended.ECS
{
    public class World : SimpleDrawableGameComponent
    {
        private readonly Bag<IUpdateSystem> _updateSystems;
        private readonly Bag<IDrawSystem> _drawSystems;

        internal World()
        {
            _updateSystems = new Bag<IUpdateSystem>();
            _drawSystems = new Bag<IDrawSystem>();

            RegisterSystem(ComponentManager = new ComponentManager());
            RegisterSystem(EntityManager = new EntityManager(ComponentManager));
        }

        public override void Dispose()
        {
            foreach (var updateSystem in _updateSystems)
                updateSystem.Dispose();

            foreach (var drawSystem in _drawSystems)
                drawSystem.Dispose();

            _updateSystems.Clear();
            _drawSystems.Clear();
            
            base.Dispose();
        }

        internal EntityManager EntityManager { get; }
        internal ComponentManager ComponentManager { get; }

        public int EntityCount => EntityManager.ActiveCount;

        internal void RegisterSystem(ISystem system)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (system is IUpdateSystem updateSystem)
                _updateSystems.Add(updateSystem);

            if (system is IDrawSystem drawSystem)
                _drawSystems.Add(drawSystem);

            system.Initialize(this);
        }

        public Entity GetEntity(int entityId)
        {
            return EntityManager.Get(entityId);
        }

        public Entity CreateEntity()
        {
            return EntityManager.Create();
        }

        public void DestroyEntity(int entityId)
        {
            EntityManager.Destroy(entityId);
        }

        public void DestroyEntity(Entity entity)
        {
            EntityManager.Destroy(entity);
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