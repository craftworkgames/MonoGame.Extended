using System;
using System.Collections;

namespace MonoGame.Extended.Entities
{
    public struct Entity
    {
        private EntityComponentSystem Ecs { get; }
        private Guid Guid { get; }

        internal Entity(EntityComponentSystem ecs, Guid guid)
        {
            Ecs = ecs;
            Guid = guid;
        }

        public bool IsAlive => Ecs.EntityExists(Guid);

        public void Destroy() => Ecs.DestroyEntity(Guid);

        public T AddComponent<T>(object component = null) where T : class => (T)Ecs.AddComponent(Guid, typeof(T), component);
        public object AddComponent(Type componentType, object component = null) => Ecs.AddComponent(Guid, componentType, component);

        public T GetComponent<T>() where T : class => Ecs.GetEntityComponent(Guid, typeof(T)) as T;
        public object GetComponent(Type componentType) => Ecs.GetEntityComponent(Guid, componentType);

        public IEnumerable GetComponents() => Ecs.GetEntityComponents(Guid);
        public IEnumerable GetComponents(Type componentType) => Ecs.GetEntityComponents(Guid, componentType);
        public IEnumerable GetComponents<T>() => Ecs.GetEntityComponents(Guid, typeof(T));

        public bool HasComponent<T>() where T : class => GetComponent<T>() != null;
        public bool HasComponent(Type componentType) => GetComponent(componentType) != null;

        public void RemoveComponent(Type componentType, object component) => Ecs.RemoveComponent(Guid, componentType, component);
        public void RemoveComponents(Type componentType) => Ecs.RemoveComponents(Guid, componentType);
    }
}
