using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace MonoGame.Extended.Animations.SpriteSheets
{
    [JsonConverter(typeof(SpriteSheetAnimationFrameJsonConverter))]
    [DebuggerDisplay("{Index} {Duration}")]
    public class SpriteSheetAnimationFrame
    {
        public int Index { get; set; }
        public float Duration { get; set; }
    }

    public class SpriteSheetAnimationFrameJsonConverter : JsonConverter<SpriteSheetAnimationFrame>
    {
        public override void WriteJson(JsonWriter writer, SpriteSheetAnimationFrame value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override SpriteSheetAnimationFrame ReadJson(JsonReader reader, Type objectType, SpriteSheetAnimationFrame existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                {
                    var index = serializer.Deserialize<int>(reader);
                    return new SpriteSheetAnimationFrame {Index = index };
                }
                case JsonToken.StartObject:
                {
                    var frame = new SpriteSheetAnimationFrame();
                    serializer.Populate(reader, frame);
                    return frame;
                }
                case JsonToken.Null:
                    return null;
                default:
                    throw new JsonSerializationException();
            }
        }
    }
}