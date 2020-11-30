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
        private static readonly Dictionary<Type, Func<string, object>> _stringParsers = new Dictionary<Type, Func<string, object>>
        {
            {typeof(int), s => int.Parse(s, CultureInfo.InvariantCulture.NumberFormat)},
            {typeof(float), s => float.Parse(s, CultureInfo.InvariantCulture.NumberFormat)},
            {typeof(HslColor), s => ColorExtensions.FromHex(s).ToHsl() }
        };

        public static T[] ReadAsMultiDimensional<T>(this JsonReader reader)
        {
            var tokenType = reader.TokenType;

            switch (tokenType)
            {
                case JsonToken.StartArray:
                    return reader.ReadAsJArray<T>();

                case JsonToken.String:
                    return reader.ReadAsDelimitedString<T>();

                case JsonToken.Integer:
                case JsonToken.Float:
                    return reader.ReadAsSingleValue<T>();

                default:
                    throw new NotSupportedException($"{tokenType} is not currently supported in the multi dimensional parser");
            }
        }

        private static T[] ReadAsSingleValue<T>(this JsonReader reader)
        {
            return new[] { JToken.Load(reader).ToObject<T>() };
        }

        private static T[] ReadAsJArray<T>(this JsonReader reader)
        {
            var jArray = JArray.Load(reader);
            var items = new List<T>();

            foreach (var token in jArray)
            {
                if (token.Type == JTokenType.String)
                {
                    var stringParser = _stringParsers[typeof(T)];
                    var s = token.Value<string>();
                    items.Add((T)stringParser(s));
                }
                else
                {
                    items.Add(token.Value<T>());
                }
            }

            return items.ToArray();
        }

        private static T[] ReadAsDelimitedString<T>(this JsonReader reader)
        {
            var value = (string)reader.Value;
            var parser = _stringParsers[typeof(T)];
            return value.Split(' ')
                .Select(i => parser(i))
                .Cast<T>()
                .ToArray();
        }
    }
}