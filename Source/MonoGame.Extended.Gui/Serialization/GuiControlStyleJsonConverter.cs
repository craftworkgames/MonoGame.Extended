using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Gui.Controls;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiControlStyleJsonConverter : JsonConverter
    {
        private readonly Dictionary<string, Type> _controlTypes;
        private const string _typeProperty = "Type";
        private const string _nameProperty = "Name";

        public GuiControlStyleJsonConverter()
        {
            _controlTypes = typeof(GuiControl)
                .GetTypeInfo()
                .Assembly
                .ExportedTypes
                .Where(t => t.GetTypeInfo().IsSubclassOf(typeof(GuiControl)))
                .ToDictionary(t => t.Name);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var style = (GuiControlStyle)value;
            var dictionary = new Dictionary<string, object> { [_typeProperty] = style.TargetType.Name };

            foreach (var keyValuePair in style)
                dictionary.Add(keyValuePair.Key, keyValuePair.Value);

            serializer.Serialize(writer, dictionary);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var dictionary = serializer.Deserialize<Dictionary<string, object>>(reader);
            var name = dictionary.GetValueOrDefault(_nameProperty) as string;
            var typeName = dictionary.GetValueOrDefault(_typeProperty) as string;
            var targetType =  typeName != null ? _controlTypes[typeName] : typeof(GuiControl);
            var properties = targetType
                .GetRuntimeProperties()
                .ToDictionary(p => p.Name);
            var style = new GuiControlStyle(name, targetType);

            foreach (var keyValuePair in dictionary.Where(i => i.Key != _typeProperty))
            {
                var propertyName = keyValuePair.Key;
                var rawValue = keyValuePair.Value;

                PropertyInfo propertyInfo;
                var value = properties.TryGetValue(propertyName, out propertyInfo)
                    ? DeserializeValueAs(serializer, rawValue, propertyInfo.PropertyType)
                    : DeserializeValueAs(serializer, rawValue, typeof(object));

                style.Add(propertyName, value);
            }

            return style;
        }

        private static object DeserializeValueAs(JsonSerializer serializer, object value, Type type)
        {
            var json = JsonConvert.SerializeObject(value);

            using (var textReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(textReader))
            {
                return serializer.Deserialize(jsonReader, type);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiControlStyle);
        }
    }
}