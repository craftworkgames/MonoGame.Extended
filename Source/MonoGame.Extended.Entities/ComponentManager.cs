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
            : base(Aspect.All())
        {
            _mappers = new Bag<ComponentMapper>();
            _componentTypes = new Dictionary<Type, int>();
        }

        private readonly Bag<ComponentMapper> _mappers;
        private readonly Dictionary<Type, int> _componentTypes;

        private ComponentMapper<T> CreateMapperForType<T>(int id)
            where T : class 
        {
            var mapper = new ComponentMapper<T>(id);
            _mappers[id] = mapper;
            return mapper;
        }

        public ComponentMapper<T> GetMapper<T>() 
            where T : class
        {
            var id = GetComponentTypeId(typeof(T));

            if (_mappers[id] != null)
                return _mappers[id] as ComponentMapper<T>;

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

        public override void Initialize(ComponentManager componentManager)
        {
            // TODO : Okay this is weird.
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}