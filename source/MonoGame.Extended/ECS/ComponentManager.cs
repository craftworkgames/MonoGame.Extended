using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.ECS.Systems;

namespace MonoGame.Extended.ECS
{
    public interface IComponentMapperService : IEnumerable<ComponentMapper>
    {
        ComponentMapper<T> GetMapper<T>() where T : class;
        ComponentMapper this[Type type] { get; }
    }

    public class ComponentManager : UpdateSystem, IComponentMapperService, IEnumerable<ComponentMapper>
    {
        private const int MAX_COMPONENT_TYPES = 256;

        public ComponentManager()
        {
            _componentMappers = new Bag<ComponentMapper>();
            _componentTypes = new Dictionary<Type, int>();
        }

        internal readonly Bag<ComponentMapper> _componentMappers;
        internal readonly Dictionary<Type, int> _componentTypes;

        public Action<int> ComponentsChanged;

        private ComponentMapper CreateMapperForType(Type type, int componentTypeId)
        {
            if (!type.IsClass)
                throw new ArgumentException("Type must be a class type.", nameof(type));

            if (componentTypeId >= MAX_COMPONENT_TYPES)
                throw new InvalidOperationException($"Component type limit exceeded. Maximum of {MAX_COMPONENT_TYPES} component types are supported.");

            var mapperType = typeof(ComponentMapper<>).MakeGenericType(type);
            var mapper = Activator.CreateInstance(mapperType, args: new object[] { componentTypeId, ComponentsChanged });
            _componentMappers[componentTypeId] = (ComponentMapper)mapper;
            return (ComponentMapper)mapper;
        }

        private ComponentMapper<T> CreateMapperForType<T>(int componentTypeId)
            where T : class
        {
            if (componentTypeId >= MAX_COMPONENT_TYPES)
                throw new InvalidOperationException($"Component type limit exceeded. Maximum of {MAX_COMPONENT_TYPES} component types are supported.");

            var mapper = new ComponentMapper<T>(componentTypeId, ComponentsChanged);
            _componentMappers[componentTypeId] = mapper;
            return mapper;
        }

        public ComponentMapper GetMapper(int componentTypeId)
        {
            return _componentMappers[componentTypeId];
        }

        public ComponentMapper<T> GetMapper<T>()
            where T : class
        {
            var componentTypeId = GetComponentTypeId(typeof(T));

            if (_componentMappers[componentTypeId] != null)
                return _componentMappers[componentTypeId] as ComponentMapper<T>;

            return CreateMapperForType<T>(componentTypeId);
        }

        public int GetComponentTypeId(Type type)
        {
            if (_componentTypes.TryGetValue(type, out var id))
                return id;

            id = _componentTypes.Count;
            _componentTypes.Add(type, id);
            return id;
        }

        public ComponentBits CreateComponentBits(int entityId)
        {
            var componentBits = new ComponentBits();

            for (var componentId = 0; componentId < _componentMappers.Count; componentId++)
            {
                componentBits[componentId] = _componentMappers[componentId]?.Has(entityId) ?? false;
            }

            return componentBits;
        }

        public void Destroy(int entityId)
        {
            foreach (var componentMapper in _componentMappers)
                componentMapper?.Delete(entityId);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public ComponentMapper this[Type type]
        {
            get
            {
                var componentTypeId = GetComponentTypeId(type);

                if (_componentMappers[componentTypeId] != null)
                    return _componentMappers[componentTypeId];

                return CreateMapperForType(type, componentTypeId);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<ComponentMapper> GetEnumerator() => new ComponentMapperEnumerator(this);
        
        public struct ComponentMapperEnumerator : IEnumerator<ComponentMapper>
        {
            private readonly ComponentManager _componentManager;
            private IEnumerator<ComponentMapper> _enumerator;

            internal ComponentMapperEnumerator(ComponentManager componentManager)
            {
                _componentManager = componentManager;
                _enumerator = _componentManager._componentMappers.GetEnumerator();
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }

            object IEnumerator.Current => _enumerator.Current;
            public ComponentMapper Current => _enumerator.Current;

            public void Dispose()
            {
                _enumerator?.Dispose();
            }
        }
    }
}
