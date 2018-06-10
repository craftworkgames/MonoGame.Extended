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

        public void CreateComponent(int entityId, T component)
        {
            Components[entityId] = component;
        }

        public T GetComponent(Entity entity)
        {
            return GetComponent(entity.Id);
        }

        public T GetComponent(int entityId)
        {
            return Components[entityId];
        }

        public bool HasComponent(int entityId)
        {
            return Components[entityId] != null;
        }

        public void DeleteComponent(int entityId)
        {
            Components[entityId] = null;
        }
    }
}