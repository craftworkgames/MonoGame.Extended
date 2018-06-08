using System;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities
{
    public abstract class ComponentMapper
    {
        protected ComponentMapper(Type componentType)
        {
            ComponentType = componentType;
        }

        public Type ComponentType { get; }
    }

    public class ComponentMapper<T> : ComponentMapper
        where T : class
    {
        public ComponentMapper()
            : base(typeof(T))
        {
            Components = new Bag<T>();
        }

        public Bag<T> Components { get; }

        public void Put(int entityId, T component)
        {
            Components[entityId] = component;
        }

        public T Get(int entityId)
        {
            return Components[entityId];
        }

        public bool Has(int entityId)
        {
            return Components[entityId] != null;
        }

        public void Delete(int entityId)
        {
            Components[entityId] = null;
        }
    }
}