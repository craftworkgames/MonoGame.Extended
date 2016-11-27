using System;
using System.Reflection;
using MonoGame.Extended.Gui.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiControlStyleConveter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var targetType = typeof(GuiControl);
            var style = new GuiControlStyle(targetType);

            foreach (var property in jObject.Properties())
            {
                var propertyType = targetType.GetRuntimeProperty(property.Name);
                var value = property.Value.ToObject(propertyType.PropertyType, serializer);

                style.Setters.Add(property.Name, value);
            }

            return style;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiControlStyle);
        }
    }
}