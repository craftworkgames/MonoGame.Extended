using System;
using System.Linq;
using Autofac;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Legacy;

namespace Platformer
{
    public class AutofacDependencyResolver : DefaultDependencyResolver
    {
        private readonly IContainer _container;

        public AutofacDependencyResolver(IContainer container)
        {
            _container = container;
        }

        public override object Resolve(Type type, params object[] args)
        {
            if (_container.IsRegistered(type))
            {
                var instance = _container.Resolve(type, args.Select((a, i) => new PositionalParameter(i, a)));

                if (instance != null)
                    return instance;
            }

            return base.Resolve(type, args);
        }
    }
}