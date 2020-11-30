using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace MonoGame.Extended.Sprites
{
    [JsonConverter(typeof(SpriteSheetAnimationFrameJsonConverter))]
    [DebuggerDisplay("{Index} {Duration}")]
    public class SpriteSheetAnimationFrame
    {
        public SpriteSheetAnimationFrame(int index, float duration = 0.2f)
        {
            Index = index;
            Duration = duration;
        }

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
                    return new SpriteSheetAnimationFrame(index);
                }
                case JsonToken.StartObject:
                {
                    var frame = new SpriteSheetAnimationFrame(0);
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