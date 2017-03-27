using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities
{
    internal interface IComponentPool
    {
        Component New();
    }

    internal class ComponentPool<T> : ObjectPool<T>, IComponentPool where T : Component, IPoolable, new()
    {
        public ComponentPool(int intitialSize = 16, ObjectPoolIsFullPolicy isFullPolicy = ObjectPoolIsFullPolicy.ReturnNull) 
            : base(CreateObject, intitialSize, isFullPolicy)
        {
        }

        private static T CreateObject()
        {
            return new T();
        }

        Component IComponentPool.New()
        {
            return base.New();
        }
    }
}
