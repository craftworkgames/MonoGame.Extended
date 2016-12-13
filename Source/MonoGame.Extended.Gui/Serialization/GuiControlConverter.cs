using System;
using MonoGame.Extended.Gui.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiControlConverter : JsonConverter
    {
        public GuiControlConverter()
        {
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiControl);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(GuiControl))
            {
                var jObject = JObject.Load(reader);
                var type = (string) jObject.Property("Type");

                switch (type)
                {
                    case "GuiPanel":
                        return jObject.ToObject<GuiPanel>(serializer);
                    case "GuiButton":
                        return jObject.ToObject<GuiButton>(serializer);
                    default:
                        throw new InvalidOperationException($"Unexpected type {type}");
                }
            }

            return ReadJson(reader, objectType, existingValue, serializer);
        }
    }
}