using System;
using MonoGame.Extended.TextureAtlases;
using Newtonsoft.Json;

namespace MonoGame.Extended.Gui.Serialization
{
    public class TextureRegion2DConveter : JsonConverter
    {
        private readonly TextureAtlas _textureAtlas;

        public TextureRegion2DConveter(TextureAtlas textureAtlas)
        {
            _textureAtlas = textureAtlas;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var regionName = reader.Value.ToString();
            return _textureAtlas[regionName];
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TextureRegion2D);
        }
    }
}