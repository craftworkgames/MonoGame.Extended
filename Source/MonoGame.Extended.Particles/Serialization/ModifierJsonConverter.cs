using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Serialization;

namespace MonoGame.Extended.Particles.Serialization
{
    public class ModifierJsonConverter : BaseTypeJsonConverter<IModifier>
    {
        public ModifierJsonConverter()
            : base(GetSupportedTypes(), "Modifier")
        {
        }

        private static IEnumerable<TypeInfo> GetSupportedTypes()
        {
            return typeof(IModifier)
                .GetTypeInfo()
                .Assembly
                .DefinedTypes
                .Where(type => typeof(IModifier).GetTypeInfo().IsAssignableFrom(type) && !type.IsAbstract);
        }
    }
}