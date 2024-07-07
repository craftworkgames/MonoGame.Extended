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

            // TODO: We can probably do better than this without a huge performance penalty by creating our own bit vector that grows after the first 32 bits.
            if (componentTypeId >= 32)
                throw new InvalidOperationException("Component type limit exceeded. We currently only allow 32 component types for performance reasons.");

            var mapperType = typeof(ComponentMapper<>).MakeGenericType(type);
            var mapper = Activator.CreateInstance(mapperType, args: new object[] { componentTypeId, ComponentsChanged });
            _componentMappers[componentTypeId] = (ComponentMapper)mapper;
            return (ComponentMapper)mapper;
        }

        private ComponentMapper<T> CreateMapperForType<T>(int componentTypeId)
            where T : class
        {
            // TODO: We can probably do better than this without a huge performance penalty by creating our own bit vector that grows after the first 32 bits.
            if (componentTypeId >= 32)
                throw new InvalidOperationException("Component type limit exceeded. We currently only allow 32 component types for performance reasons.");

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

        public BitVector32 CreateComponentBits(int entityId)
        {
            var componentBits = new BitVector32();
            var mask = BitVector32.CreateMask();

            for (var componentId = 0; componentId < _componentMappers.Count; componentId++)
            {
                componentBits[mask] = _componentMappers[componentId]?.Has(entityId) ?? false;
                mask = BitVector32.CreateMask(mask);
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
