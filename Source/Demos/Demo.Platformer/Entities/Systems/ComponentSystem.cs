using System.Collections.Generic;

namespace Demo.Platformer.Entities.Systems
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
    }
}