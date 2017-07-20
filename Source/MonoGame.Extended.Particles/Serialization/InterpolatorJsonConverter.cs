using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Serialization;

namespace MonoGame.Extended.Particles.Serialization
{
    public class InterpolatorJsonConverter : BaseTypeJsonConverter<Interpolator>
    {
        public InterpolatorJsonConverter() 
            : base(GetSupportedTypes(), "Interpolator")
        {
        }

        private static IEnumerable<TypeInfo> GetSupportedTypes()
        {
            return typeof(Interpolator)
                .GetTypeInfo()
                .Assembly
                .DefinedTypes
                .Where(type => typeof(Interpolator).GetTypeInfo().IsAssignableFrom(type) && !type.IsAbstract);
        }
    }
}