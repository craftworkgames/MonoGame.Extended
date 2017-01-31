using System;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    public class TextureRegion2DConveter : JsonConverter
    {
        private readonly ITextureRegionService _textureRegionService;

        public TextureRegion2DConveter(ITextureRegionService textureRegionService)
        {
            _textureRegionService = textureRegionService;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var region = (TextureRegion2D)value;
            writer.WriteValue(region.Name);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var regionName = reader.Value as string;
            return regionName == null ? null : _textureRegionService.GetTextureRegion(regionName);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TextureRegion2D);
        }
    }
}