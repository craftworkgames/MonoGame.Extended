using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities.Components;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class ComponentSystem
    {
        protected ComponentSystem()
        {
        }

        private EntityComponentSystem _parent;
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
                    _parent.ComponentAttached += (s, c) => OnComponentAttached(c);
                    _parent.ComponentDetached += (s, c) => OnComponentDetached(c);
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

        protected virtual void OnComponentAttached(EntityComponent component) { }
        protected virtual void OnComponentDetached(EntityComponent component) { }

        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
    }
}