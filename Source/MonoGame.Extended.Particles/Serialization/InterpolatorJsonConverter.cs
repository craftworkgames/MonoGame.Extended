using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Serialization;

namespace MonoGame.Extended.Particles.Serialization
{
    public class InterpolatorJsonConverter : BaseTypeJsonConverter<IInterpolator>
    {
        public InterpolatorJsonConverter() 
            : base(GetSupportedTypes(), "Interpolator")
        {
        }

        private static IEnumerable<TypeInfo> GetSupportedTypes()
        {
            return typeof(IInterpolator)
                .GetTypeInfo()
                .Assembly
                .DefinedTypes
                .Where(type => typeof(IInterpolator).GetTypeInfo().IsAssignableFrom(type) && !type.IsAbstract);
        }
    }
}