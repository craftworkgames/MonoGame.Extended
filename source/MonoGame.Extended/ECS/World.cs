using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.ECS.Systems;

namespace MonoGame.Extended.ECS
{
    /// <summary>
    /// Represents an Entity Component System (ECS) world that manages entities, components, and system.s
    /// </summary>
    /// <remarks>
    /// The World is the central container for all ECS operations.  It manages the lifecycle of entities, coordinates
    /// component storage, and executes registered systems during update and draw cycles.
    /// </remarks>
    public class World : SimpleDrawableGameComponent
    {
        private readonly Bag<IUpdateSystem> _updateSystems;
        private readonly Bag<IDrawSystem> _drawSystems;

        internal EntityManager EntityManager { get; }
        internal ComponentManager ComponentManager { get; }

        /// <summary>
        /// Occurs when an entity is added to the world during the update cycle.
        /// </summary>
        /// <remarks>
        /// This event is raised after the entity has been fully initialized with it initial components.
        /// The entity ID is valid and can be used to retrieve the entity or query its components.
        /// Subscribers can use this even to perform initialization logic such as adding the entity to external
        /// collections, registering it with other systems, or creating associated resources.
        /// </remarks>
        public event Action<int> EntityAdded;

        /// <summary>
        /// Occurs when an entity is removed from the world during the update cycle.
        /// </summary>
        /// <remarks>
        /// This event is raised before the entity is returned to the pool and before its components are destroyed,
        /// allowing subscribers to perform cleanup operations such as removing the entity from external collections,
        /// unregistering it from other systems, or releasing associated resources.
        /// After this event completes, the entity ID becomes invalid and should not be used.
        /// </remarks>
        public event Action<int> EntityRemoved;

        /// <summary>
        /// Occurs when an entity's component composition changes during the update cycle.
        /// </summary>
        /// <remarks>
        /// This event is raised whenever components are attached to or detached from an entity.
        /// Subscribers can use this event to respond to composition changes, such as updating cached component
        /// references, recalculating entity classifications, or refreshing system queries.
        /// The event is not raised for component value modifications, only structural changes to which components are
        /// present on the entity.
        /// </remarks>
        public event Action<int> EntityChanged;

        /// <summary>
        /// Gets the number of active entities in the world.
        /// </summary>
        /// <remarks>
        /// This count reflects entities that have been created and not yet destroyed.
        /// Entities queued for destruction are still counted until the next update cycle completes.
        /// </remarks>
        public int EntityCount => EntityManager.ActiveCount;

        internal World()
        {
            _updateSystems = new Bag<IUpdateSystem>();
            _drawSystems = new Bag<IDrawSystem>();

            RegisterSystem(ComponentManager = new ComponentManager());
            RegisterSystem(EntityManager = new EntityManager(ComponentManager));

            EntityManager.EntityAdded += OnEntityAdded;
            EntityManager.EntityRemoved += OnEntityRemoved;
            EntityManager.EntityChanged += OnEntityChanged;
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
            if (EntityChanged != null)
            {
                EntityChanged.Invoke(entityId);
            }
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

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="entityId">The unique identifier of the entity to retrieve.</param>
        /// <returns>
        /// The entity with the specified identifier, or <c>null</c> if the entity has been destroyed or the ID is
        /// invalid.
        /// </returns>
        public Entity GetEntity(int entityId)
        {
            return EntityManager.Get(entityId);
        }

        /// <summary>
        /// Creates a new entity in the world.
        /// </summary>
        /// <remarks>
        /// The entity is created immediately, but the <see cref="EntityAdded"/> event is not raised until
        /// the next <see cref="Update(GameTime)"/> cycle.  The returned entity can be used immediately to
        /// attach components.
        /// Entity IDs are reused from a pool after entities are destroyed.
        /// </remarks>
        /// <returns>A new entity with a unique identifier.</returns>
        public Entity CreateEntity()
        {
            return EntityManager.Create();
        }

        /// <summary>
        /// Destroys an entity in the world.
        /// </summary>
        /// <remarks>
        /// The entity is queued for destruction and the <see cref="EntityRemoved"/> event is raised during
        /// the next <see cref="Update(GameTime)"/> cycle, before the entity and its components are removed from
        /// memory.
        /// Calling this method multiple times with the same entity ID has no additional effect.
        /// After destruction completes, the entity ID may be reused for new entities.
        /// </remarks>
        /// <param name="entityId">The unique identifier of the entity to destroy.</param>
        public void DestroyEntity(int entityId)
        {
            EntityManager.Destroy(entityId);
        }

        /// <summary>
        /// Destroys an entity in the world.
        /// </summary>
        /// <remarks>
        /// The entity is queued for destruction and the <see cref="EntityRemoved"/> event is raised during
        /// the next <see cref="Update(GameTime)"/> cycle, before the entity and its components are removed from
        /// memory.
        /// Calling this method multiple times with the same entity ID has no additional effect.
        /// After destruction completes, the entity ID may be reused for new entities.
        /// </remarks>
        /// <param name="entity">The entity to destroy.</param>
        public void DestroyEntity(Entity entity)
        {
            EntityManager.Destroy(entity);
        }

        /// <summary>
        /// Updates all registered systems in the world.
        /// </summary>
        /// <remarks>
        /// This method invokes <see cref="IUpdateSystem.Update(GameTime)"/> on all registered update systems in
        /// registration order.
        /// Entity lifecycle events (<see cref="EntityAdded"/>, <see cref="EntityRemoved"/>,
        /// <see cref="EntityChanged"/>) are raised during this update cycle as entities are processed by the
        /// <see cref="EntityManager"/>.
        /// </remarks>
        /// <param name="gameTime">A snapshot of the timing values for the current cycle.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (var system in _updateSystems)
            {
                system.Update(gameTime);
            }
        }

        /// <summary>
        /// Draws all registered systems in the world.
        /// </summary>
        /// <remarks>
        /// This method invokes <see cref="IDrawSystem.Draw(GameTime)"/> on all registered draw systems in
        /// registration order.
        /// </remarks>
        /// <param name="gameTime">A snapshot of the timing values for the current cycle.</param>
        public override void Draw(GameTime gameTime)
        {
            foreach (var system in _drawSystems)
            {
                system.Draw(gameTime);
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="World"/>
        /// </summary>
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
    }
}
