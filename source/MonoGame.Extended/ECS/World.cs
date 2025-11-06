using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.ECS.Systems;

namespace MonoGame.Extended.ECS
{
    public class World : SimpleDrawableGameComponent
    {
        private readonly Bag<IUpdateSystem> _updateSystems;
        private readonly Bag<IDrawSystem> _drawSystems;

        internal EntityManager EntityManager { get; }
        internal ComponentManager ComponentManager { get; }

        public event Action<int> EntityAdded;
        public event Action<int> EntityRemoved;
        public event Action<int> EntityChanged;

        public int EntityCount => EntityManager.ActiveCount;

        internal World()
        {
            _updateSystems = new Bag<IUpdateSystem>();
            _drawSystems = new Bag<IDrawSystem>();

            RegisterSystem(ComponentManager = new ComponentManager());
            RegisterSystem(EntityManager = new EntityManager(ComponentManager));

            EntityManager.EntityAdded += OnEntityAdded;
            EntityManager.EntityRemoved += OnEntityRemoved;
            EntityManager.EntityRemoved += OnEntityChanged;
        }

        public override void Dispose()
        {
            EntityManager.EntityAdded -= OnEntityAdded;
            EntityManager.EntityRemoved -= OnEntityRemoved;
            EntityManager.EntityChanged -= OnEntityChanged;

            foreach (var updateSystem in _updateSystems)
            {
                updateSystem.Dispose();
            }

            foreach (var drawSystem in _drawSystems)
            {
                drawSystem.Dispose();
            }

            _updateSystems.Clear();
            _drawSystems.Clear();

            base.Dispose();
        }

        internal void RegisterSystem(ISystem system)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (system is IUpdateSystem updateSystem)
            {
                _updateSystems.Add(updateSystem);
            }

            if (system is IDrawSystem drawSystem)
            {
                _drawSystems.Add(drawSystem);
            }

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
            {
                system.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var system in _drawSystems)
            {
                system.Draw(gameTime);
            }
        }

        private void OnEntityAdded(int entityId)
        {
            if (EntityAdded != null)
            {
                EntityAdded.Invoke(entityId);
            }
        }

        private void OnEntityRemoved(int entityId)
        {
            if (EntityRemoved != null)
            {
                EntityRemoved.Invoke(entityId);
            }
        }

        private void OnEntityChanged(int entityId)
        {
            if(EntityChanged != null)
            {
                EntityChanged.Invoke(entityId);
            }
        }
    }
}
