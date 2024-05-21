using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Gui.Serialization;

public class VerticalAlignmentConverter : JsonConverter<VerticalAlignment>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(VerticalAlignment);

    /// <inheritdoc />
    public override VerticalAlignment Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (value.Equals("Center", StringComparison.OrdinalIgnoreCase) || value.Equals("Centre", StringComparison.OrdinalIgnoreCase))
        {
            return VerticalAlignment.Centre;
        }

        if (Enum.TryParse<VerticalAlignment>(value, true, out var alignment))
        {
            return alignment;
        }

        throw new InvalidOperationException($"Invalid value for '{nameof(VerticalAlignment)}'");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, VerticalAlignment value, JsonSerializerOptions options) { }
}
