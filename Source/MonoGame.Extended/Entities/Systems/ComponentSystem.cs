using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Components;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class ComponentSystem
    {
        private EntityComponentSystem _parent;

        protected ComponentSystem()
        {
        }

        internal EntityComponentSystem Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null)
                    throw new InvalidOperationException("ComponentSystem already registered");

                _parent = value;

                if (_parent != null)
                {
                    _parent.EntityCreated += (sender, entity) => OnEntityCreated(entity);
                    _parent.EntityDestroyed += (sender, entity) => OnEntityDestroyed(entity);
                    _parent.ComponentAttached += (sender, component) => OnComponentAttached(component);
                    _parent.ComponentDetached += (sender, component) => OnComponentDetached(component);
                }
            }
        }

        protected IEnumerable<T> GetComponents<T>()
        {
            return Parent.GetComponents<T>();
        }

        protected Entity GetEntity(string name)
        {
            return Parent.GetEntity(name);
        }

        protected virtual void OnEntityCreated(Entity entity)
        {
        }

        protected virtual void OnEntityDestroyed(Entity entity)
        {
        }

        protected virtual void OnComponentAttached(EntityComponent component)
        {
        }

        protected virtual void OnComponentDetached(EntityComponent component)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime)
        {
        }
    }
}