using System;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MonoGame.Extended.Gui.Serialization
{
    public class NinePatchRegion2DConveter : JsonConverter
    {
        private readonly ITextureRegionService _textureRegionService;

        public NinePatchRegion2DConveter(ITextureRegionService textureRegionService)
        {
            _textureRegionService = textureRegionService;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var region = (NinePatchRegion2D)value;
            var jsonObject = new
            {
                TextureRegion = region.Name,
                Padding = $"{region.LeftPadding} {region.TopPadding} {region.RightPadding} {region.BottomPadding}"
            };
            serializer.Serialize(writer, jsonObject);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = serializer.Deserialize<JObject>(reader);
            var paddingAsString = jsonObject.Value<string>("Padding");
            var thickness = GuiThickness.Parse(paddingAsString);
            var regionName = jsonObject.Value<string>("TextureRegion");
            var region = _textureRegionService.GetTextureRegion(regionName);
            return new NinePatchRegion2D(region, thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(NinePatchRegion2D);
        }
    }
}