using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities
{
    internal interface IComponentPool
    {
        Component New();
    }

    public class ComponentPool<T> : ObjectPool<T>, IComponentPool where T : Component, IPoolable, new()
    {
        public ComponentPool(int? capacity = null) 
            : base(capacity ?? 10, InstantiationFunction)
        {
        }

        private static T InstantiationFunction(int i)
        {
            return new T();
        }

        public Component New()
        {
            return base.New();
        }
    }
}
