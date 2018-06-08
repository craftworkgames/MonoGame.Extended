using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities
{
    public class ComponentManager
    {
        public ComponentManager()
        {
            _mappers = new Bag<ComponentMapper>();
        }

        private readonly Bag<ComponentMapper> _mappers;

        public void RegisterComponentType<T>()
            where T : class 
        {
            var index = _mappers.Count;
            var componentType = new ComponentType(typeof(T), index);
            var mapper = new ComponentMapper<T>();
            _mappers[index] = mapper;
        }
    }
}