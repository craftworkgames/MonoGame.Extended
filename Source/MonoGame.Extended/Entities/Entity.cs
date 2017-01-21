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

        public void Destroy() => Ecs.DestroyEntity(Guid);

        public void AddComponent(Type componentType) => Ecs.AddComponent(Guid, componentType);

        public object GetComponent(Type componentType) => Ecs.GetEntityComponent(Guid, componentType);
        public IEnumerable GetComponents() => Ecs.GetEntityComponents(Guid);
        public IEnumerable GetComponents(Type componentType) => Ecs.GetEntityComponents(Guid, componentType);
        public IEnumerable GetComponents<T>() => Ecs.GetEntityComponents(Guid, typeof(T));

        public void RemoveComponent(Type componentType, object component) => Ecs.RemoveComponent(Guid, componentType, component);
        public void RemoveComponents(Type componentType) => Ecs.RemoveComponents(Guid, componentType);
    }
}
