using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MonoGame.Extended.Particles.Profiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Particles.Serialization
{
    public class ProfileJsonConverter : JsonConverter
    {
        private readonly Dictionary<string, Type> _profileTypes;

        public ProfileJsonConverter()
        {
            _profileTypes = typeof(Profile)
                .GetTypeInfo()
                .Assembly
                .DefinedTypes
                .Where(type => type.IsSubclassOf(typeof(Profile)) && !type.IsAbstract)
                .ToDictionary(t => TrimSuffix(t.Name, nameof(Profile), StringComparison.OrdinalIgnoreCase), t => t.AsType(), StringComparer.OrdinalIgnoreCase);
        }

        private static string TrimSuffix(string input, string suffix, StringComparison comparisonType)
        {
            if (input.EndsWith(suffix, comparisonType))
                return input.Substring(0, input.Length - suffix.Length);

            return input;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var type = jObject.GetValue("type").ToObject<string>();
            Type profileType;

            if (_profileTypes.TryGetValue(type, out profileType))
                return jObject.ToObject(profileType, serializer);

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Profile);
        }
    }
}