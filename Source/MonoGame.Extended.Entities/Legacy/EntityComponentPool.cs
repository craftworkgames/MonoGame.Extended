using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities.Legacy
{
    internal interface IComponentPool
    {
        object New();
    }

    internal class ComponentPool<T> : ObjectPool<T>, IComponentPool where T : class, IPoolable, new()
    {
        public ComponentPool(int intitialSize = 16, ObjectPoolIsFullPolicy isFullPolicy = ObjectPoolIsFullPolicy.ReturnNull) 
            : base(CreateObject, intitialSize, isFullPolicy)
        {
        }

        private static T CreateObject()
        {
            return new T();
        }

        object IComponentPool.New()
        {
            return base.New();
        }
    }
}
