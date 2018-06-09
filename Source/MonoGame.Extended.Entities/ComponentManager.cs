using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities.Systems;

namespace MonoGame.Extended.Entities
{
    public class ComponentManager : UpdateSystem
    {
        public ComponentManager()
        {
            _mappers = new Bag<ComponentMapper>();
            _componentTypes = new Dictionary<Type, int>();
        }

        private readonly Bag<ComponentMapper> _mappers;
        private readonly Dictionary<Type, int> _componentTypes;

        private ComponentMapper<T> CreateMapperForType<T>()
            where T : class 
        {
            var id = _mappers.Count;
            var mapper = new ComponentMapper<T>(id);
            _mappers[id] = mapper;
            return mapper;
        }

        public ComponentMapper<T> GetMapper<T>() 
            where T : class
        {
            if (_componentTypes.TryGetValue(typeof(T), out var id))
                return _mappers[id] as ComponentMapper<T>;

            var mapper = CreateMapperForType<T>();
            _componentTypes.Add(typeof(T), mapper.Id);
            return mapper;
        }

        public override void Initialize(ComponentManager componentManager)
        {
            // TODO : Okay this is weird.
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}