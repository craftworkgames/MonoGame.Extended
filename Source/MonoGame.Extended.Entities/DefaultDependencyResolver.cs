using System;

namespace MonoGame.Extended.Entities
{
    public class DefaultDependencyResolver : DependencyResolver
    {
        public DefaultDependencyResolver()
        {
        }

        public override object Resolve(Type type, params object[] args)
        {
            return Activator.CreateInstance(type, args);
        }
    }
}