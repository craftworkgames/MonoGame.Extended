using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Graphics;


namespace MonoGame.Extended.Graphics
{
    [JsonConverter(typeof(SpriteSheetAnimationFrameJsonConverter))]
    [DebuggerDisplay("{Index} {Duration}")]
    public class SpriteSheetAnimationFrame
    {
        public int FrameIndex { get; }
        public TimeSpan Duration { get; }

        public Texture2DRegion TextureRegion { get; }

        public SpriteSheetAnimationFrame(int index, Texture2DRegion region, TimeSpan duration)
        {
            ArgumentNullException.ThrowIfNull(region);
            if (region.Texture.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(region), $"The source {nameof(Texture2D)} of {nameof(region)} was disposed prior.");
            }

            FrameIndex = index;

            Duration = duration;
        }


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
                    return new SpriteSheetAnimationFrame(index, TimeSpan.FromSeconds(0.2f));

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
