using System;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    public class TextureRegion2DConveter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var region = (TextureRegion2D)value;
            writer.WriteValue(region.Name);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TextureRegion2D);
        }
    }

    public class NinePatchRegion2DConveter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var region = (NinePatchRegion2D)value;
            var jsonObject = new { Name = region.Name, Padding = $"{region.LeftPadding} {region.TopPadding} {region.RightPadding} {region.BottomPadding}" };
            serializer.Serialize(writer, jsonObject);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(NinePatchRegion2D);
        }
    }
}