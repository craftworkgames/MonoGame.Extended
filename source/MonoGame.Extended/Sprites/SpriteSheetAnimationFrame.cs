using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;


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
        /// <inheritdoc />
        public override SpriteSheetAnimationFrame Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    var index = reader.GetInt32();
                    return new SpriteSheetAnimationFrame(index);

                case JsonTokenType.StartObject:
                    var frame = JsonSerializer.Deserialize<SpriteSheetAnimationFrame>(ref reader, options);
                    return frame;

                case JsonTokenType.Null:
                    return null;

                default:
                    throw new JsonException();
            }
        }

        public override void Write(Utf8JsonWriter writer, SpriteSheetAnimationFrame value, JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(writer);
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
