using System;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    public interface ITextureRegionService
    {
        TextureRegion2D GetTextureRegion(string name);
    }

    public class TextureRegionService : ITextureRegionService
    {
        public TextureRegionService()
        {
            TextureAtlases = new List<TextureAtlas>();
        }

        public List<TextureAtlas> TextureAtlases { get; }

        public TextureRegion2D GetTextureRegion(string name)
        {
            return TextureAtlases
                .Select(textureAtlas => textureAtlas.GetRegion(name))
                .FirstOrDefault(region => region != null);
        }
    }

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