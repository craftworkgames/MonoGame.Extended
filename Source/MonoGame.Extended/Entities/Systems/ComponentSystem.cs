using System.Collections.Generic;

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
    }
}