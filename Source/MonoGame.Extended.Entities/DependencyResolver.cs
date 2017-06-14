using System;

namespace MonoGame.Extended.Entities
{
    public abstract class DependencyResolver
    {
        public abstract object Resolve(Type type, params object[] args);

        public T Resolve<T>(Type type, params object[] args)
        {
            return (T)Resolve(type, args);
        }
    }
}