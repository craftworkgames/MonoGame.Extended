// Original code dervied from:
// https://github.com/thelinuxlich/artemis_CSharp/blob/master/Artemis_XNA_INDEPENDENT/Manager/EntityManager.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityManager.cs" company="GAMADU.COM">
//     Copyright � 2013 GAMADU.COM. All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without modification, are
//     permitted provided that the following conditions are met:
//
//        1. Redistributions of source code must retain the above copyright notice, this list of
//           conditions and the following disclaimer.
//
//        2. Redistributions in binary form must reproduce the above copyright notice, this list
//           of conditions and the following disclaimer in the documentation and/or other materials
//           provided with the distribution.
//
//     THIS SOFTWARE IS PROVIDED BY GAMADU.COM 'AS IS' AND ANY EXPRESS OR IMPLIED
//     WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//     FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL GAMADU.COM OR
//     CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//     CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//     SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
//     ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//     NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
//     ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     The views and conclusions contained in the software and documentation are those of the
//     authors and should not be interpreted as representing official policies, either expressed
//     or implied, of GAMADU.COM.
// </copyright>
// <summary>
//   The Entity Manager.
// </summary>
// -----------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities
{
    public class EntityManager
    {
        private readonly SystemManager _systemManager;
        private readonly ObjectPool<Entity> _pool;
        private readonly Dictionary<string, Entity> _entitiesByName;
        private readonly Dictionary<string, EntityTemplate> _entityTemplatesByName;
        private readonly Dictionary<string, Bag<Entity>> _entitiesByGroup;
        private readonly List<Entity> _markedEntities;

        private readonly Dictionary<int, IComponentPool> _componentPoolsByComponentTypeIndex;
        private readonly Bag<Dictionary<Entity, EntityComponent>> _componentTypeEntitiesToComponents;
        private readonly List<EntityComponentTypePair> _componentsToRemove;

        private readonly Dictionary<Type, EntityComponentType> _componentTypes = new Dictionary<Type, EntityComponentType>();

        public int TotalEntitiesCount => _pool.TotalCount;
        public int ActiveEntitiesCount => _pool.InUseCount;

        public event EntityDelegate EntityCreated;
        public event EntityDelegate EntityAdded;
        public event EntityDelegate EntityRemoved;
        public event EntityDelegate EntityDestroyed;

        internal EntityManager(SystemManager systemManager)
        {
            _systemManager = systemManager;

            _pool = new ObjectPool<Entity>(CreateEntityObject, 100, ObjectPoolIsFullPolicy.IncreaseSize);
            _pool.ItemUsed += OnEntityCreated;
            _pool.ItemReturned += OnEntityDestroyed;

            _entitiesByName = new Dictionary<string, Entity>();
            _entityTemplatesByName = new Dictionary<string, EntityTemplate>();
            _entitiesByGroup = new Dictionary<string, Bag<Entity>>();

            _markedEntities = new List<Entity>();

            _componentPoolsByComponentTypeIndex = new Dictionary<int, IComponentPool>();
            _componentTypeEntitiesToComponents = new Bag<Dictionary<Entity, EntityComponent>>();
            _componentsToRemove = new List<EntityComponentTypePair>();
        }

        private Entity CreateEntityObject()
        {
            return new Entity(_systemManager.ProcessingSystems.Count, _componentTypes.Count);
        }

        public Entity CreateEntity()
        {
            var entity = _pool.New();
            MarkEntityToBeAdded(entity);
            return entity;
        }

        public Entity CreateEntityFromTemplate(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var entity = _pool.New();
            MarkEntityToBeAdded(entity);

            EntityTemplate entityTemplate;
            _entityTemplatesByName.TryGetValue(name, out entityTemplate);
            if (entityTemplate == null)
                throw new InvalidOperationException($"EntityTemplate '{name}' is not registered.");

            entityTemplate.Build(entity);
            return entity;
        }

        internal void AddEntityTemplate(string name, EntityTemplate template)
        {
            Debug.Assert(!string.IsNullOrEmpty(name));
            Debug.Assert(template != null);
            Debug.Assert(!_entityTemplatesByName.ContainsKey(name));

            _entityTemplatesByName.Add(name, template);
        }

        public Entity GetEntityByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            Entity entity;
            _entitiesByName.TryGetValue(name, out entity);
            if (entity != null)
                return entity;
            RemoveEntityName(name);
            return null;
        }

        internal void AddEntityName(string name, Entity entity)
        {
            Debug.Assert(entity != null);

            if (string.IsNullOrEmpty(name))
                return;
            if (!string.IsNullOrEmpty(entity._name))
                RemoveEntityName(entity._name);
            _entitiesByName.Add(name, entity);
        }

        internal void RemoveEntityName(string name)
        {
            _entitiesByName.Remove(name);
        }

        public Bag<Entity> GetEntitiesByGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return null;

            Bag<Entity> bag;
            _entitiesByGroup.TryGetValue(groupName, out bag);
            return bag;
        }

        internal void AddEntityToGroup(string group, Entity entity)
        {
            Debug.Assert(entity != null);

            if (string.IsNullOrEmpty(group))
                return;

            RemoveEntityFromGroup(entity);

            entity._group = group;

            Bag<Entity> entities;
            if (!_entitiesByGroup.TryGetValue(group, out entities))
            {
                entities = new Bag<Entity>();
                _entitiesByGroup.Add(group, entities);
            }

            entities.Add(entity);
        }

        internal void RemoveEntityFromGroup(Entity entity)
        {
            Bag<Entity> entities;
            if (_entitiesByGroup.TryGetValue(entity._group, out entities))
                entities.Remove(entity);
        }

        internal void DestroyEntity(Entity entity)
        {
            Debug.Assert(entity != null);

            RemoveEntityFromGroup(entity);
            RemoveComponents(entity);
            entity.Return();
        }

        internal void MarkEntityToBeAdded(Entity entity)
        {
            Debug.Assert(entity != null);

            entity.WaitingToBeAdded = true;
            if (entity.WaitingToRefreshComponents)
                return;
            entity.WaitingToRefreshComponents = true;
            _markedEntities.Add(entity);
        }

        internal void MarkEntityToBeRefreshed(Entity entity)
        {
            Debug.Assert(entity != null);

            if (entity.WaitingToRefreshComponents)
                return;
            entity.WaitingToRefreshComponents = true;
            _markedEntities.Add(entity);
        }

        internal void MarkEntityToBeRemoved(Entity entity)
        {
            Debug.Assert(entity != null);
            Debug.Assert(entity != null);

            entity.WaitingToBeRemoved = true;
            if (entity.WaitingToRefreshComponents)
                return;
            entity.WaitingToRefreshComponents = true;
            _markedEntities.Add(entity);
            OnEntityRemoved(entity);
        }

        internal void ProcessMarkedEntitiesWith(List<EntityProcessingSystem> processingSystems)
        {
            Debug.Assert(processingSystems != null);

            foreach (var entity in _markedEntities)
            {
                foreach (var system in processingSystems)
                    system.RefreshEntityComponents(entity);

                if (entity.WaitingToBeAdded)
                {
                    entity.WaitingToBeAdded = false;
                    OnEntityAdded(entity);
                }

                if (entity.WaitingToBeRemoved)
                {
                    entity.WaitingToBeRemoved = false;
                    DestroyEntity(entity);
                }

                entity.WaitingToRefreshComponents = false;
            }

            _markedEntities.Clear();
        }

        private void OnEntityCreated(Entity entity)
        {
            entity.Manager = this;
            EntityCreated?.Invoke(entity);
        }

        private void OnEntityDestroyed(Entity entity)
        {
            EntityDestroyed?.Invoke(entity);
        }

        private void OnEntityAdded(Entity entity)
        {
            EntityAdded?.Invoke(entity);
        }

        private void OnEntityRemoved(Entity entity)
        {
            EntityRemoved?.Invoke(entity);
        }

        internal T AddComponent<T>(Entity entity) where T : EntityComponent
        {
            Debug.Assert(entity != null);

            var componentType = GetComponentTypeFrom(typeof(T));
            return (T) AddComponent(entity, componentType);
        }

        internal EntityComponent AddComponent(Entity entity, EntityComponentType componentType)
        {
            Debug.Assert(entity != null);
            Debug.Assert(componentType != null);

            if (componentType.Index >= _componentTypeEntitiesToComponents.Capacity)
                _componentTypeEntitiesToComponents[componentType.Index] = null;
            var components = _componentTypeEntitiesToComponents[componentType.Index];
            if (components == null)
                _componentTypeEntitiesToComponents[componentType.Index] = components = new Dictionary<Entity, EntityComponent>();

            EntityComponent component;

            IComponentPool componentPool;
            if (_componentPoolsByComponentTypeIndex.TryGetValue(componentType.Index, out componentPool))
            {
                component = componentPool.New();
                if (component == null)
                    return null;
            }
            else
            {
                component = (EntityComponent)Activator.CreateInstance(componentType.Type);
            }

            component.Entity = entity;
            components[entity] = component;

            entity.ComponentBits[componentType.Index] = true;

            MarkEntityToBeRefreshed(entity);

            return component;
        }

        internal T GetComponent<T>(Entity entity) where T : EntityComponent
        {
            Debug.Assert(entity != null);

            var componentType = GetComponentTypeFrom(typeof(T));
            return (T)GetComponent(entity, componentType);
        }

        internal EntityComponent GetComponent(Entity entity, EntityComponentType componentType)
        {
            Debug.Assert(entity != null);
            Debug.Assert(componentType != null);
            Debug.Assert(componentType.Index < _componentTypeEntitiesToComponents.Count);

            var components = _componentTypeEntitiesToComponents[componentType.Index];

            Debug.Assert(components != null);

            EntityComponent component;
            components.TryGetValue(entity, out component);
            return component;
        }

        internal void MarkComponentToBeRemoved<T>(Entity entity) where T : EntityComponent
        {
            Debug.Assert(entity != null);

            MarkComponentToBeRemoved(entity, GetComponentTypeFrom(typeof(T)));
        }

        internal void MarkComponentToBeRemoved(Entity entity, EntityComponentType componentType)
        {
            Debug.Assert(entity != null);
            Debug.Assert(componentType != null);

            var pair = new EntityComponentTypePair(entity, componentType);
            if (!_componentsToRemove.Contains(pair))
                _componentsToRemove.Add(pair);
        }

        internal void RemoveMarkedComponents()
        {
            for (var i = _componentsToRemove.Count - 1; i >= 0; --i)
            {
                var pair = _componentsToRemove[i];
                var entity = pair.Entity;
                var componentType = pair.ComponentType;
                var components = _componentTypeEntitiesToComponents[componentType.Index];

                Debug.Assert(components != null);

                EntityComponent component;
                if (!components.TryGetValue(entity, out component))
                    continue;

                entity.ComponentBits[componentType.Index] = false;
                MarkEntityToBeRefreshed(entity);

                components.Remove(entity);
                component.Return();
            }

            _componentsToRemove.Clear();
        }

        internal void RemoveComponents(Entity entity)
        {
            Debug.Assert(entity != null);

            MarkEntityToBeRefreshed(entity);

            for (var i = _componentTypeEntitiesToComponents.Count - 1; i >= 0; --i)
            {
                var components = _componentTypeEntitiesToComponents[i];

                Debug.Assert(components != null);

                EntityComponent component;
                if (!components.TryGetValue(entity, out component))
                    continue;

                components.Remove(entity);
                component.Return();
            }
        }

        internal void CreateComponentTypesFrom(List<TypeInfo> componentTypeInfos)
        {
            foreach (var componentTypeInfo in componentTypeInfos)
            {
                var type = componentTypeInfo.AsType();
                var componentType = new EntityComponentType(type);
                _componentTypes.Add(type, componentType);
            }
        }

        internal EntityComponentType GetComponentTypeFrom(Type type)
        {
            EntityComponentType result;

            if (!_componentTypes.TryGetValue(type, out result))
                throw new InvalidOperationException($"{type.Name} is not marked with the EntityComponent attribute");

            return result;
        }

        internal void FillComponentBits(BitVector bits, Type[] types)
        {
            foreach (var type in types)
            {
                var componentType = GetComponentTypeFrom(type);
                bits[componentType.Index] = true;
            }
        }

        internal void CreateComponentPoolsFrom(List<Tuple<TypeInfo, EntityComponentPoolAttribute>> componentTypeInfos)
        {
            foreach (var tuple in componentTypeInfos)
            {
                var type = tuple.Item1.AsType();
                var attribute = tuple.Item2;
                var componentType = GetComponentTypeFrom(type);
                CreateComponentPool(componentType, attribute.InitialSize, attribute.IsFullPolicy);
            }
        }

        private void CreateComponentPool(EntityComponentType componentType, int initialSize, ObjectPoolIsFullPolicy isFullPolicy)
        {
            Debug.Assert(componentType != null);
            Debug.Assert(initialSize > 0);
            Debug.Assert(!_componentPoolsByComponentTypeIndex.ContainsKey(componentType.Index));

            var poolType = typeof(ComponentPool<>).MakeGenericType(componentType.Type);
            var componentPool = (IComponentPool)Activator.CreateInstance(poolType, initialSize, isFullPolicy);

            _componentPoolsByComponentTypeIndex.Add(componentType.Index, componentPool);
        }

        internal struct EntityComponentTypePair
        {
            public Entity Entity;
            public EntityComponentType ComponentType;

            public EntityComponentTypePair(Entity entity, EntityComponentType componentType)
            {
                Entity = entity;
                ComponentType = componentType;
            }
        }
    }
}
