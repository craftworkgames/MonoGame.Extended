using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Serialization
{
    public static class JsonReaderExtensions
    {
        private static readonly Dictionary<Type, Func<string,  object>> _stringParsers = new Dictionary<Type, Func<string, object>>
        {
            {typeof(int), s => int.Parse(s, CultureInfo.InvariantCulture.NumberFormat)},
            {typeof(float), s => float.Parse(s, CultureInfo.InvariantCulture.NumberFormat)},
        };

        public static T[] ReadAsMultiDimensional<T>(this JsonReader reader)
        {
            var tokenType = reader.TokenType;

            switch (tokenType)
            {
                case JsonToken.StartArray:
                    var jArray = JArray.Load(reader);
                    return jArray
                        .Select(i => i.Value<T>())
                        .ToArray();

                case JsonToken.String:
                    var value = (string)reader.Value;
                    var parser = _stringParsers[typeof(T)];
                    return value.Split(' ')
                        .Select(i => parser(i))
                        .Cast<T>()
                        .ToArray();

                case JsonToken.Integer:
                case JsonToken.Float:
                    return new []{ JToken.Load(reader).ToObject<T>() };

                default:
                    throw new NotSupportedException($"{tokenType} is not currently supported in the multi dimensional parser");
            }
        }
    }
}