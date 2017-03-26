using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities
{
    public sealed partial class EntityComponentSystemManager
    {
        private readonly Bag<IComponentPool> _componentPools = new Bag<IComponentPool>();
        private readonly Bag<Bag<Component>> _componentsByType = new Bag<Bag<Component>>();
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
        //            entityComponents.Add(component);
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

        internal Component AddComponent(Entity entity, ComponentType type)
        {
            if (type.Index >= _componentsByType.Capacity)
                _componentsByType[type.Index] = null;

            var components = _componentsByType[type.Index];
            if (components == null)
            {
                _componentsByType[type.Index] = components = new Bag<Component>();
            }

            var component = GetComponentFromPool(type);
            if (component == null)
                return null;
            component.Entity = entity;

            components[entity.Index] = component;
            entity.AddTypeBit(type.Bit);

            ComponentAdded?.Invoke(entity, component);

            Refresh(entity);

            return component;
        }

        public T GetComponent<T>(Entity entity) where T : Component
        {
            return (T)GetComponent(entity, ComponentTypeManager.GetTypeFor(typeof(T)));
        }

        public Component GetComponent(Entity entity, ComponentType componentType)
        {
            if (componentType.Index >= _componentsByType.Capacity)
                return null;
            var entityIndex = entity.Index;
            var components = _componentsByType[componentType.Index];
            if (components == null || entityIndex >= components.Capacity)
                return null;
            return components[entityIndex];
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

                var componentToBeRemoved = components[entityIndex];
                if (componentToBeRemoved != null)
                {
                    OnRemovedComponent(componentToBeRemoved);
                    ComponentRemoved?.Invoke(entity, componentToBeRemoved);
                }

                components[entityIndex] = null;
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

                var componentToBeRemoved = components[entityIndex];
                if (componentToBeRemoved != null)
                {
                    OnRemovedComponent(componentToBeRemoved);
                    ComponentRemoved?.Invoke(entity, componentToBeRemoved);
                }

                entity.RemoveTypeBit(componentType.Bit);
                Refresh(entity);
                components[entityIndex] = null;
            }

            _componentsToBeRemoved.Clear();
        }

        private static void OnRemovedComponent(Component component)
        {
            var poolable = component as IPoolable;
            poolable?.Return();
        }

        internal Component GetComponentFromPool(ComponentType componentType)
        {
            var componentPool = _componentPools[componentType.Index] ?? CreateComponentPool(componentType);
            return componentPool.New();
        }

        internal IComponentPool CreateComponentPool(ComponentType componentType, int? capacity = null)
        {
            var poolType = typeof(ComponentPool<>).MakeGenericType(componentType.Type);
            var componentPool = (IComponentPool)Activator.CreateInstance(poolType, capacity);
            return _componentPools[componentType.Index] = componentPool;
        }
    }
}
