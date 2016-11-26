using System;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Gui.Serialization
{
    public class GuiJsonConverter : JsonConverter
    {
        private readonly GuiJsonConverterService _converterService;

        public GuiJsonConverter(GuiJsonConverterService converterService)
        {
            _converterService = converterService;
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(TextureRegion2D))
                return true;

            if (objectType == typeof(BitmapFont))
                return true;

            if (objectType == typeof(GuiControl))
                return true;

            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (objectType == typeof(TextureRegion2D))
                return _converterService.GetTextureRegion((string) reader.Value);

            if (objectType == typeof(BitmapFont))
                return _converterService.GetFont((string) reader.Value);

            var jObject = JObject.Load(reader);
            var type = (string) jObject.Property("Type");

            switch (type)
            {
                case "Button":
                    return jObject.ToObject<GuiButton>(serializer);
                default:
                    throw new InvalidOperationException($"Unexpected type {type}");
            }
        }
    }

    public class GuiThicknessConveter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            return GuiThickness.Parse((string) reader.Value);
            ;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GuiThickness);
        }
    }
}