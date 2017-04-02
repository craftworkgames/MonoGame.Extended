using MonoGame.Extended.Collections;

namespace MonoGame.Extended.Entities
{
    internal interface IComponentPool
    {
        EntityComponent New();
    }

    internal class ComponentPool<T> : ObjectPool<T>, IComponentPool where T : EntityComponent, IPoolable, new()
    {
        public ComponentPool(int intitialSize = 16, ObjectPoolIsFullPolicy isFullPolicy = ObjectPoolIsFullPolicy.ReturnNull) 
            : base(CreateObject, intitialSize, isFullPolicy)
        {
        }

        private static T CreateObject()
        {
            return new T();
        }

        EntityComponent IComponentPool.New()
        {
            return base.New();
        }
    }
}
