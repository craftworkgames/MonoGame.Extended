using System;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities
{
    public abstract class ComponentMapper
    {
        protected ComponentMapper(int id, Type componentType)
        {
            Id = id;
            ComponentType = componentType;
        }

        public int Id { get; }
        public Type ComponentType { get; }
    }

    public class ComponentMapper<T> : ComponentMapper
        where T : class
    {
        public ComponentMapper(int id)
            : base(id, typeof(T))
        {
            Components = new Bag<T>();
        }

        public Bag<T> Components { get; }

        public void Put(int entityId, T component)
        {
            Components[entityId] = component;
        }

        public T Get(Entity entity)
        {
            return Get(entity.Id);
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