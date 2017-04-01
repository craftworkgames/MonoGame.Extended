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
        private readonly List<Entity> _entitiesToRefresh;

        private readonly List<IComponentPool> _componentPools;
        private readonly Bag<Dictionary<Entity, Component>> _componentTypeEntitiesToComponents;
        private readonly List<EntityComponentTypePair> _componentsToRemove;

        private readonly Dictionary<Type, ComponentType> _componentTypes = new Dictionary<Type, ComponentType>();

        public int TotalEntitiesCount => _pool.TotalCount;
        public int ActiveEntitiesCount => _pool.InUseCount;

        internal EntityManager(SystemManager systemManager)
        {
            _systemManager = systemManager;

            _pool = new ObjectPool<Entity>(CreateEntityObject, 100, ObjectPoolIsFullPolicy.Resize);
            _pool.ItemUsed += OnEntityUsed;
            _pool.ItemReturned += OnEntityReturned;

            _entitiesByName = new Dictionary<string, Entity>();
            _entityTemplatesByName = new Dictionary<string, EntityTemplate>();
            _entitiesByGroup = new Dictionary<string, Bag<Entity>>();

            _entitiesToRefresh = new List<Entity>();

            _componentPools = new List<IComponentPool>();
            _componentTypeEntitiesToComponents = new Bag<Dictionary<Entity, Component>>();
            _componentsToRemove = new List<EntityComponentTypePair>();
        }

        private Entity CreateEntityObject()
        {
            return new Entity(_systemManager.SystemCount, _componentTypes.Count);
        }

        public Entity CreateEntity()
        {
            return _pool.New();
        }

        public Entity CreateEntityFromTemplate(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var entity = _pool.New();
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

        internal void RemoveEntity(Entity entity)
        {
            Debug.Assert(entity != null);

            RemoveEntityFromGroup(entity);
            RemoveComponents(entity);
            entity.Return();
        }

        internal void MarkEntityToBeRefreshed(Entity entity)
        {
            Debug.Assert(entity != null);

            if (entity.NeedsRefresh)
                return;
            entity.NeedsRefresh = true;
            _entitiesToRefresh.Add(entity);
        }

        internal void MarkEntityToBeRemoved(Entity entity)
        {
            Debug.Assert(entity != null);

            MarkEntityToBeRefreshed(entity);
            entity.IsAlive = false;
        }

        internal void RefreshEntity(Entity entity, Bag<System> systems)
        {
            Debug.Assert(entity != null);
            Debug.Assert(systems != null);

            for (var i = systems.Count - 1; i >= 0; --i)
                systems[i].OnEntityChanged(entity);

            entity.NeedsRefresh = false;

            if (!entity.IsAlive)
                 RemoveEntity(entity);
        }

        internal void RefreshMarkedEntitiesWith(Bag<System> systems)
        {
            Debug.Assert(systems != null);

            if (_entitiesToRefresh.Count <= 0)
                return;

            for (var index = _entitiesToRefresh.Count - 1; index >= 0; --index)
            {
                var entity = _entitiesToRefresh[index];
                RefreshEntity(entity, systems);
            }

            _entitiesToRefresh.Clear();
        }

        private void OnEntityUsed(Entity entity)
        {
            entity.Manager = this;
            entity.IsAlive = true;
            MarkEntityToBeRefreshed(entity);
        }

        private void OnEntityReturned(Entity entity)
        {
        }

        internal T AddComponent<T>(Entity entity) where T : Component
        {
            Debug.Assert(entity != null);

            var componentType = GetComponentTypeFrom(typeof(T));
            return (T) AddComponent(entity, componentType);
        }

        internal Component AddComponent(Entity entity, ComponentType componentType)
        {
            Debug.Assert(entity != null);
            Debug.Assert(componentType != null);

            if (componentType.Index >= _componentTypeEntitiesToComponents.Capacity)
                _componentTypeEntitiesToComponents[componentType.Index] = null;
            var components = _componentTypeEntitiesToComponents[componentType.Index];
            if (components == null)
                _componentTypeEntitiesToComponents[componentType.Index] = components = new Dictionary<Entity, Component>();

            Component component;

            if (componentType.IsPooled)
            {
                var componentPool = _componentPools[componentType.Index];
                component = componentPool.New();
                if (component == null)
                    return null;
            }
            else
            {
                component = (Component)Activator.CreateInstance(componentType.Type);
            }

            component.Entity = entity;
            components[entity] = component;

            entity.ComponentBits[componentType.Index] = true;

            MarkEntityToBeRefreshed(entity);

            return component;
        }

        internal T GetComponent<T>(Entity entity) where T : Component
        {
            Debug.Assert(entity != null);

            var componentType = GetComponentTypeFrom(typeof(T));
            return (T)GetComponent(entity, componentType);
        }

        internal Component GetComponent(Entity entity, ComponentType componentType)
        {
            Debug.Assert(entity != null);
            Debug.Assert(componentType != null);
            Debug.Assert(componentType.Index < _componentTypeEntitiesToComponents.Count);

            var components = _componentTypeEntitiesToComponents[componentType.Index];

            Debug.Assert(components != null);

            Component component;
            components.TryGetValue(entity, out component);
            return component;
        }

        internal void MarkComponentToBeRemoved<T>(Entity entity) where T : Component
        {
            Debug.Assert(entity != null);

            MarkComponentToBeRemoved(entity, GetComponentTypeFrom(typeof(T)));
        }

        internal void MarkComponentToBeRemoved(Entity entity, ComponentType componentType)
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

                Component component;
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

                Component component;
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
                var componentType = new ComponentType(type);
                _componentTypes.Add(type, componentType);
            }
        }

        internal ComponentType GetComponentTypeFrom(Type type)
        {
            ComponentType result;
            _componentTypes.TryGetValue(type, out result);
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

        internal void CreateComponentPoolsFrom(List<Tuple<TypeInfo, ComponentPoolAttribute>> componentTypeInfos)
        {
            foreach (var tuple in componentTypeInfos)
            {
                var type = tuple.Item1.AsType();
                var attribute = tuple.Item2;
                var componentType = GetComponentTypeFrom(type);
                CreateComponentPool(componentType, attribute.InitialSize, attribute.IsFullPolicy);
            }
        }

        private void CreateComponentPool(ComponentType componentType, int initialSize, ObjectPoolIsFullPolicy isFullPolicy)
        {
            Debug.Assert(componentType != null);
            Debug.Assert(initialSize > 0);
            Debug.Assert(componentType.IsPooled == false);

            componentType.IsPooled = true;
            var poolType = typeof(ComponentPool<>).MakeGenericType(componentType.Type);
            var componentPool = (IComponentPool)Activator.CreateInstance(poolType, initialSize, isFullPolicy);
            _componentPools[componentType.Index] = componentPool;
        }

        internal struct EntityComponentTypePair
        {
            public Entity Entity;
            public ComponentType ComponentType;

            public EntityComponentTypePair(Entity entity, ComponentType componentType)
            {
                Entity = entity;
                ComponentType = componentType;
            }
        }
    }
}
