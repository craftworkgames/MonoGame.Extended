// Original code dervied from:
// https://github.com/thelinuxlich/artemis_CSharp/blob/master/Artemis_XNA_INDEPENDENT/Manager/EntityManager.cs

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityManager.cs" company="GAMADU.COM">
//     Copyright � 2013 GAMADU.COM. AllOf rights reserved.
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
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities
{
    public class MissingEntityTemplateException : Exception
    {
        internal MissingEntityTemplateException(string templateName)
            : this(templateName, null)
        {
        }

        internal MissingEntityTemplateException(string templateName, Exception inner)
            : base($"EntityTemplate '{templateName}' was not registered.", inner)
        {
        }
    }

    public sealed partial class EntityComponentSystemManager
    {
        private readonly Dictionary<Guid, Entity> _identifiersToEntities = new Dictionary<Guid, Entity>();
        private int _nextAvailableEntityIndex;
        private readonly Bag<int> _entityIndexPool = new Bag<int>();
        private readonly ObjectPool<Entity> _entityPool = new ObjectPool<Entity>(() => new Entity(), 16, ObjectPoolIsFullPolicy.Resize);

        private readonly Bag<Entity> _entitiesToBeRemoved = new Bag<Entity>();
        private readonly Bag<Entity> _refreshed = new Bag<Entity>();
        private readonly Dictionary<string, EntityTemplate> _entityTemplatesByName = new Dictionary<string, EntityTemplate>();
        private readonly Dictionary<string, Entity> _entitiesByName = new Dictionary<string, Entity>();
        private readonly Dictionary<string, Bag<Entity>> _entitiesByGroup = new Dictionary<string, Bag<Entity>>();

        public Bag<Entity> Entities { get; } = new Bag<Entity>();
        public int EntitiesRequestedCount { get; private set; }
        public int RemovedEntitiesRetention { get; set; } = 100;
        public long TotalEntitiesCreatedCount { get; private set; }
        public long TotalEntitiesRemovedCount { get; private set; }

        public event EntityDelegate EntityAdded;
        public event EntityDelegate EntityRemoved;

        public Entity CreateEntity(Guid? identifier = null) 
        {
            var identifier1 = identifier ?? Guid.NewGuid();

            var entity = _entityPool.New();

            entity.Manager = this;
            entity.Identifier = identifier1;
            entity.Index = _entityIndexPool.IsEmpty
                ? _nextAvailableEntityIndex++
                : _entityIndexPool[_entityIndexPool.Count - 1];

            _identifiersToEntities[entity.Identifier] = entity;
            Entities[entity.Index] = entity;

            ++EntitiesRequestedCount;

            if (TotalEntitiesCreatedCount < long.MaxValue)
            {
                ++TotalEntitiesCreatedCount;
            }

            Refresh(entity);

            EntityAdded?.Invoke(entity);

            return entity;
        }

        public Entity CreateEntityFromTemplate(string templateName)
        {
            return CreateEntityFromTemplate(null, templateName);
        }

        private Entity CreateEntityFromTemplate(Guid? identifier, string templateName)
        {
            if (string.IsNullOrEmpty(templateName))
                throw new ArgumentNullException(nameof(templateName));

            var entity = CreateEntity(identifier);
            EntityTemplate entityTemplate;
            _entityTemplatesByName.TryGetValue(templateName, out entityTemplate);
            if (entityTemplate == null)
                throw new MissingEntityTemplateException(templateName);

            entityTemplate.Build(entity);
            Refresh(entity);
            return entity;
        }

        public void AddEntityTemplate(string tempalteName, EntityTemplate entityTemplate)
        {
            _entityTemplatesByName.Add(tempalteName, entityTemplate);
        }

        //public Bag<Entity> GetEntitiesByGroup(Aspect aspect)
        //{
        //    var entities = aspect.Entities;
        //    if (entities == null)
        //        entities = aspect.Entities = new Bag<Entity>();
        //    else
        //        entities.ReturnAll();

        //    // ReSharper disable once ForCanBeConvertedToForeach
        //    for (var index = 0; index < Entities.Count; ++index)
        //    {
        //        var entity = Entities[index];
        //        if (entity != null && aspect.IsInterestedIn(entity))
        //            entities.Attach(entity);
        //    }

        //    return entities;
        //}

        //public Entity GetEntity(int index)
        //{
        //    return Entities[index];
        //}

        public Entity GetEntity(Guid identifier)
        {
            Entity entity;
            _identifiersToEntities.TryGetValue(identifier, out entity);
            return entity;
        }

        public Entity GetEntity(string name)
        {      
            Entity entity;
            _entitiesByName.TryGetValue(name, out entity);
            if (entity != null && entity.IsActive)
                return entity;
            UnregisterEntityName(name);
            return null;
        }

        public bool IsEntityNameRegistered(string name)
        {
            Entity entity;
            _entitiesByName.TryGetValue(name, out entity);
            if (entity != null && entity.IsActive)
                return true;
            UnregisterEntityName(name);
            return false;
        }

        internal void RegisterEntityName(string name, Entity entity)
        {
            if (entity == null)
                return;
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            _entitiesByName.Add(name, entity);
        }

        internal void UnregisterEntityName(string name)
        {
            _entitiesByName.Remove(name);
        }

        public Bag<Entity> GetEntitiesByGroup(string groupName)
        {
            Bag<Entity> bag;
            return _entitiesByGroup.TryGetValue(groupName, out bag) ? bag : null;
        }

        internal void RemoveEntityFromGroupIfPossible(Entity entity)
        {
            if (string.IsNullOrEmpty(entity?.Group))
                return;

            Bag<Entity> entities;
            if (_entitiesByGroup.TryGetValue(entity.Group, out entities))
                entities.Remove(entity);
        }

        internal void SetGroupForEntity(string group, Entity entity)
        {
            if (string.IsNullOrEmpty(group) || entity == null)
                return;

            RemoveEntityFromGroupIfPossible(entity);

            entity._group = group;

            Bag<Entity> entities;
            if (!_entitiesByGroup.TryGetValue(group, out entities))
            {
                entities = new Bag<Entity>();
                _entitiesByGroup.Add(group, entities);
            }

            entities.Add(entity);
        }

        public void Remove(Entity entity)
        {
            if (entity == null || entity.IsBeingRemoved)
                return;
            entity.IsBeingRemoved = true;
            _entitiesToBeRemoved.Add(entity);
        }

        internal void InternalRemove(Entity entity)
        {
            if (entity == null)
                return;

            RemoveEntityFromGroupIfPossible(entity);

            var swapEntity = Entities[Entities.Count - 1];
            if (swapEntity != null)
                swapEntity.Index = entity.Index;
            Entities[entity.Index] = swapEntity;
            Entities[Entities.Count - 1] = null;

            RemoveComponents(entity);

            --EntitiesRequestedCount;
            if (TotalEntitiesRemovedCount < long.MaxValue)
                ++TotalEntitiesRemovedCount;

            entity.Return();

            EntityRemoved?.Invoke(entity);

            _identifiersToEntities.Remove(entity.Identifier);
        }

        private void RemoveMarkedEntities()
        {
            if (_entitiesToBeRemoved.IsEmpty)
                return;

            for (var index = _entitiesToBeRemoved.Count - 1; index >= 0; --index)
            {
                var entity = _entitiesToBeRemoved[index];
                RemoveEntityFromGroupIfPossible(entity);
                entity.IsBeingRemoved = false;
                InternalRemove(entity);
            }

            _entitiesToBeRemoved.Clear();
        }

        internal void Refresh(Entity entity)
        {
            if (entity == null || entity.IsBeingRefreshed)
                return;
            entity.IsBeingRefreshed = true;
            _refreshed.Add(entity);
        }

        internal void InternalRefresh(Entity entity)
        {
            for (var i = Systems.Count - 1; i >= 0; --i)
                Systems[i].OnEntityChanged(entity);
        }

        private void RefreshMarkedEntities()
        {
            if (_refreshed.IsEmpty)
                return;

            for (var index = _refreshed.Count - 1; index >= 0; --index)
            {
                var entity = _refreshed[index];
                InternalRefresh(entity);
                entity.IsBeingRefreshed = false;
                _refreshed.Remove(index);
            }

            _refreshed.Clear();
        }

        public bool IsActive(Entity entity)
        {
            return Entities[entity.Index] != null;
        }
    }
}
