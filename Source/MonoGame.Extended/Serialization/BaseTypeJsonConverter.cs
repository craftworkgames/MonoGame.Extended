using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Serialization
{
    public abstract class BaseTypeJsonConverter<T> : JsonConverter
    {
        private readonly string _suffix;
        private readonly Dictionary<string, Type> _baseTypes;

        protected BaseTypeJsonConverter(IEnumerable<TypeInfo> supportedTypes, string suffix)
        {
            _suffix = suffix;
            _baseTypes = supportedTypes
                .ToDictionary(t => TrimSuffix(t.Name, suffix), t => t.AsType(), StringComparer.OrdinalIgnoreCase);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var key = jObject.GetValue("type", StringComparison.OrdinalIgnoreCase).ToObject<string>();
            Type type;

            if (_baseTypes.TryGetValue(key, out type))
                return jObject.ToObject(type, serializer);

            throw new InvalidOperationException($"Unknown {_suffix} type '{key}'");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        private static string TrimSuffix(string input, string suffix)
        {
            if (input.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                return input.Substring(0, input.Length - suffix.Length);

            return input;
        }
    }
}