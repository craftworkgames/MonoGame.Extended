using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Gui.Serialization;

public class HorizontalAlignmentConverter : JsonConverter<HorizontalAlignment>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(HorizontalAlignment);

    /// <inheritdoc />
    public override HorizontalAlignment Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (value.Equals("Center", StringComparison.OrdinalIgnoreCase) || value.Equals("Centre", StringComparison.OrdinalIgnoreCase))
        {
            return HorizontalAlignment.Centre;
        }

        if (Enum.TryParse<HorizontalAlignment>(value, true, out var alignment))
        {
            return alignment;
        }

        throw new InvalidOperationException($"Invalid value for '{nameof(HorizontalAlignment)}'");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, HorizontalAlignment value, JsonSerializerOptions options) { }
}
