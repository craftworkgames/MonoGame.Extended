using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Gui.Controls;

namespace MonoGame.Extended.Gui.Serialization
{
    public class ControlStyleJsonConverter : JsonConverter<ControlStyle>
    {
        private readonly Dictionary<string, Type> _controlTypes;
        private const string _typeProperty = "Type";
        private const string _nameProperty = "Name";

        public ControlStyleJsonConverter(params Type[] customControlTypes)
        {
            _controlTypes = typeof(Control)
                .GetTypeInfo()
                .Assembly
                .ExportedTypes
                .Concat(customControlTypes)
                .Where(t => t.GetTypeInfo().IsSubclassOf(typeof(Control)))
                .ToDictionary(t => t.Name);
        }

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(ControlStyle);

        /// <inheritdoc />
        public override ControlStyle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options);
            var name = dictionary.GetValueOrDefault(_nameProperty) as string;
            var typeName = dictionary.GetValueOrDefault(_typeProperty) as string;

            if (!_controlTypes.TryGetValue(typeName, out Type controlType))
                throw new FormatException("invalid control type: " + typeName);

            var targetType = typeName != null ? controlType : typeof(Control);
            var properties = targetType
                .GetRuntimeProperties()
                .ToDictionary(p => p.Name);
            var style = new ControlStyle(name, targetType);

            foreach (var keyValuePair in dictionary.Where(i => i.Key != _typeProperty))
            {
                var propertyName = keyValuePair.Key;
                var rawValue = keyValuePair.Value;

                PropertyInfo propertyInfo;
                var value = properties.TryGetValue(propertyName, out propertyInfo)
                    ? DeserializeValueAs(rawValue, propertyInfo.PropertyType)
                    : DeserializeValueAs(rawValue, typeof(object));

                style.Add(propertyName, value);
            }

            return style;
        }

        private static object DeserializeValueAs(object value, Type type)
        {
            var json = JsonSerializer.Serialize(value, type);
            return JsonSerializer.Deserialize(json, type);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, ControlStyle value, JsonSerializerOptions options)
        {
            var style = (ControlStyle)value;
            var dictionary = new Dictionary<string, object> { [_typeProperty] = style.TargetType.Name };

            foreach (var keyValuePair in style)
                dictionary.Add(keyValuePair.Key, keyValuePair.Value);

            JsonSerializer.Serialize(writer, dictionary);
        }






    }
}
