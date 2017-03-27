using System;
using System.Collections.Generic;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities
{
    public sealed partial class EntityComponentSystemManager
    {
        private readonly Bag<IComponentPool> _componentPools = new Bag<IComponentPool>();
        private readonly Bag<Dictionary<Entity, Component>> _componentsByType = new Bag<Dictionary<Entity, Component>>();
        private readonly Bag<ValueTuple<Entity, ComponentType>> _componentsToBeRemoved = new Bag<ValueTuple<Entity, ComponentType>>();

        public event EntityComponentDelegate ComponentAdded;
        public event EntityComponentDelegate ComponentRemoved;

        //public Bag<Component> GetComponents(Entity entity)
        //{
        //    if (entity == null)
        //        return null;

        //    var entityComponents = new Bag<Component>();
        //    var entityIndex = entity.Index;
        //    for (int i = 0, b = _componentsByType.Count; b > i; ++i)
        //    {
        //        var components = _componentsByType[i];
        //        if (components == null || entityIndex >= components.Count)
        //            continue;
        //        var component = components[entityIndex];
        //        if (component != null)
        //            entityComponents.Attach(component);
        //    }

        //    return entityComponents;
        //}

        internal void AddComponent(Entity entity, Type componentType)
        {
            if (entity == null)
                return;
            var type = ComponentTypeManager.GetTypeFor(componentType);
            AddComponent(entity, type);
        }

        public T AddComponent<T>(Entity entity) where T : Component
        {
            var type = ComponentTypeManager.GetTypeFor<T>();
            return (T)AddComponent(entity, type);
        }

        internal Component AddComponent(Entity entity, ComponentType componentType)
        {
            if (componentType.Index >= _componentsByType.Capacity)
                _componentsByType[componentType.Index] = null;

            var components = _componentsByType[componentType.Index];
            if (components == null)
                _componentsByType[componentType.Index] = components = new Dictionary<Entity, Component>();

            Component component;

            if (componentType.IsPooled)
            {
                var componentPool = _componentPools[componentType.Index];
                component = componentPool.New();
            }
            else
            {
                component = (Component)Activator.CreateInstance(componentType.Type);
            }

            if (component == null)
                return null;

            component.Entity = entity;
            components[entity] = component;

            entity.AddTypeBit(componentType.Bit);

            OnComponentAdded(entity, component);

            return component;
        }

        private void OnComponentAdded(Entity entity, Component component)
        {
            ComponentAdded?.Invoke(entity, component);
            Refresh(entity);
        }

        public T GetComponent<T>(Entity entity) where T : Component
        {
            return (T)GetComponent(entity, ComponentTypeManager.GetTypeFor(typeof(T)));
        }

        public Component GetComponent(Entity entity, ComponentType componentType)
        {
            if (componentType.Index >= _componentsByType.Capacity)
                return null;
            var components = _componentsByType[componentType.Index];
            if (components == null)
                return null;
            Component component;
            components.TryGetValue(entity, out component);
            return component;
        }

        internal void RemoveComponent<T>(Entity entity) where T : Component
        {
            RemoveComponent(entity, ComponentType<T>.Type);
        }

        internal void RemoveComponent(Entity entity, ComponentType componentType)
        {
            if (entity == null || componentType == null)
                return;

            var pair = new ValueTuple<Entity, ComponentType>(entity, componentType);
            if (!_componentsToBeRemoved.Contains(pair))
                _componentsToBeRemoved.Add(pair);
        }

        internal void RemoveComponents(Entity entity)
        {
            entity.TypeBits = 0;
            Refresh(entity);

            var entityIndex = entity.Index;
            for (var i = _componentsByType.Count - 1; i >= 0; --i)
            {
                var components = _componentsByType[i];
                if (components == null || entityIndex >= components.Count)
                    continue;

                Component component;
                if (!components.TryGetValue(entity, out component))
                    continue;

                components.Remove(entity);
                OnRemovedComponent(entity, component);
            }
        }

        internal void RemoveMarkedComponents()
        {
            for (var index = _componentsToBeRemoved.Count - 1; index >= 0; --index)
            {
                var pair = _componentsToBeRemoved[index];
                var entity = pair.Item1;
                var componentType = pair.Item2;
                var entityIndex = entity.Index;
                var components = _componentsByType[componentType.Index];

                if (components == null || entityIndex >= components.Count)
                    continue;

                Component component;
                if (!components.TryGetValue(entity, out component))
                    continue;

                entity.RemoveTypeBit(componentType.Bit);
                Refresh(entity);

                components.Remove(entity);
                OnRemovedComponent(entity, component);
            }

            _componentsToBeRemoved.Clear();
        }

        private void OnRemovedComponent(Entity entity, Component component)
        {
            var poolable = component as IPoolable;
            poolable?.Return();

            ComponentRemoved?.Invoke(entity, component);
        }

        internal IComponentPool CreateComponentPool(ComponentType componentType, int? capacity = null)
        {
            if (componentType.IsPooled)
                return _componentPools[componentType.Index];

            componentType.IsPooled = true;
            var poolType = typeof(ComponentPool<>).MakeGenericType(componentType.Type);
            var componentPool = (IComponentPool)Activator.CreateInstance(poolType, capacity);
            return _componentPools[componentType.Index] = componentPool;
        }
    }
}
