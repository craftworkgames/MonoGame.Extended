﻿using System;
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
        public abstract bool Has(int entityId);
    }

    public class ComponentMapper<T> : ComponentMapper
        where T : class
    {
        private readonly Action<int> _onCompositionChanged;

        public ComponentMapper(int id, Action<int> onCompositionChanged)
            : base(id, typeof(T))
        {
            _onCompositionChanged = onCompositionChanged;
            Components = new Bag<T>();
        }

        public Bag<T> Components { get; }

        public void Put(int entityId, T component)
        {
            Components[entityId] = component;
            _onCompositionChanged(entityId);
        }

        public T Get(Entity entity)
        {
            return Get(entity.Id);
        }

        public T Get(int entityId)
        {
            return Components[entityId];
        }

        public override bool Has(int entityId)
        {
            if (entityId >= Components.Count)
                return false;

            return Components[entityId] != null;
        }

        public void Delete(int entityId)
        {
            Components[entityId] = null;
            _onCompositionChanged(entityId);
        }
    }
}