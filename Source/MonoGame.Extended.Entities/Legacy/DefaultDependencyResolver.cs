using System;

namespace MonoGame.Extended.Entities.Legacy
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