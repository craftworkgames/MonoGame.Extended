using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities.Systems;

namespace MonoGame.Extended.Entities
{
    public interface IComponentMapperService
    {
        ComponentMapper<T> GetMapper<T>() where T : class;
    }

    public class ComponentManager : UpdateSystem, IComponentMapperService
    {
        public ComponentManager()
        {
            _componentMappers = new Bag<ComponentMapper>();
            _componentTypes = new Dictionary<Type, int>();
        }

        private readonly Bag<ComponentMapper> _componentMappers;
        private readonly Dictionary<Type, int> _componentTypes;

        private ComponentMapper<T> CreateMapperForType<T>(int id)
            where T : class 
        {
            var mapper = new ComponentMapper<T>(id);
            _componentMappers[id] = mapper;
            return mapper;
        }

        public ComponentMapper<T> GetMapper<T>() 
            where T : class
        {
            var id = GetComponentTypeId(typeof(T));

            if (_componentMappers[id] != null)
                return _componentMappers[id] as ComponentMapper<T>;

            return CreateMapperForType<T>(id);
        }

        public int GetComponentTypeId(Type type)
        {
            if (_componentTypes.TryGetValue(type, out var id))
                return id;

            id = _componentTypes.Count;
            _componentTypes.Add(type, id);
            return id;
        }

        public void RefreshComponentBits(int entityId, ref BitArray bits)
        {
            for (var componentId = 0; componentId < _componentMappers.Count; componentId++)
                bits[componentId] = _componentMappers[componentId].Has(entityId);

            for (var i = _componentMappers.Count; i < bits.Length; i++)
                bits[i] = false;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public long GetCompositionIdentity(BitArray bits)
        {
            var array = new int[2];
            bits.CopyTo(array, 0);
            return (uint)array[0] + ((long)(uint)array[1] << 32);
        }
    }
}