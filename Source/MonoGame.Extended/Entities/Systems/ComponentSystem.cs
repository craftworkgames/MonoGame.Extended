using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Entities.Systems
{
    public abstract class ComponentSystem
    {
        protected ComponentSystem()
        {
        }

        internal EntityComponentSystem EntityComponentSystem { get; set; }

        protected IEnumerable<T> GetComponents<T>()
        {
            return EntityComponentSystem.GetComponents<T>();
        }

        protected Entity GetEntity(string name)
        {
            return EntityComponentSystem.GetEntity(name);
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime)
        {
        }
    }
}