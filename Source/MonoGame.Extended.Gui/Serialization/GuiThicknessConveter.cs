using System;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiThicknessConveter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var thickness = (GuiThickness)value;
            writer.WriteValue($"{thickness.Left} {thickness.Top} {thickness.Right} {thickness.Bottom}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return GuiThickness.Parse((string)reader.Value);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiThickness);
        }
    }
}