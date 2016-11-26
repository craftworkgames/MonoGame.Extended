using System;
using MonoGame.Extended.Gui.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiControlConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var type = (string) jObject.Property("Type");

            switch (type)
            {
                case "Button":
                    return jObject.ToObject<GuiButton>(serializer);
                case "Label":
                    return jObject.ToObject<GuiLabel>(serializer);
                case "ToggleButton":
                    return jObject.ToObject<GuiToggleButton>(serializer);
                default:
                    throw new InvalidOperationException($"Unexpected control type {type}");
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiControl);
        }
    }
}