using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MonoGame.Extended.Gui.Controls;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiControlStyleJsonConverter : JsonConverter
    {
        private readonly Dictionary<string, Type> _controlTypes;
        private const string _typeProperty = "Type";

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
            var dictionary = new Dictionary<string, object>
            {
                [_typeProperty] = style.TargetType.Name
            };

            foreach (var keyValuePair in style)
                dictionary.Add(keyValuePair.Key, keyValuePair.Value);

            serializer.Serialize(writer, dictionary);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var dictionary = serializer.Deserialize<Dictionary<string, object>>(reader);
            var typeName = (string)dictionary[_typeProperty];
            var targetType = _controlTypes[typeName];
            var properties = targetType
                .GetRuntimeProperties()
                .ToDictionary(p => p.Name);
            var style = new GuiControlStyle(targetType);

            foreach (var keyValuePair in dictionary.Where(i => i.Key != _typeProperty))
            {
                var propertyName = keyValuePair.Key;

                if (properties.TryGetValue(propertyName, out var propertyInfo))
                {
                    var json = JsonConvert.SerializeObject(keyValuePair.Value);

                    using (var textReader = new StringReader(json))
                    using (var jsonReader = new JsonTextReader(textReader))
                    {
                        var value = serializer.Deserialize(jsonReader, propertyInfo.PropertyType);
                        style.Add(propertyName, value);
                    }
                }
            }

            return style;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiControlStyle);
        }
    }
}