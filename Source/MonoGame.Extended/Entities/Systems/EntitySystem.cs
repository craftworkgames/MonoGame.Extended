using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Components;
using System.Collections.Generic;

namespace MonoGame.Extended.Entities.Systems
{
    /// <summary>
    /// <para>
    /// The <see cref="EntitySystem"/> is where the logic of an <see cref="Entities.Entity"/> happens.
    /// An <see cref="EntitySystem"/> cannot directly interact with each other. 
    /// Instead, an <see cref="EntitySystem"/> accesses its parent <see cref="Entities.Entity"/>'s
    /// components to register itself to <see cref="EntityComponent"/>'s delegates,
    /// or invokes them so another <see cref="EntitySystem"/> can handle the event.
    /// </para>
    /// 
    /// <para>
    /// Every system must implement <see cref="OnRemoved"/> in order to remove themselves
    /// from any delegates they subscribed to. Not doing so can cause memory leaks, as the
    /// object is still held in memory by the delegate, and will thus not be garbage collected.
    /// </para>
    /// </summary>
    public abstract class EntitySystem
    {
        public Entity Entity { get; private set; }

        protected TEntityComponent GetComponent<TEntityComponent>() 
            where TEntityComponent : EntityComponent
        {
            return Entity.GetComponent<TEntityComponent>();
        }

        internal void Initialize(Entity entity)
        {
            Entity = entity;
            OnInitialized();
        }

        protected virtual void OnInitialized() { }

        /// <summary>
        /// Removes this system from the entity.
        /// </summary>
        protected void Remove()
        {
            OnRemoved();
            Entity.DetachSystem(this);
            Entity = null;
        }

        /// <summary>
        /// Called when the system is removed from the entity. Use this method
        /// to remove the system from any event handlers in order to avoid
        /// memory leaks.
        /// </summary>
        protected virtual void OnRemoved() { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime) { }
    }
}
