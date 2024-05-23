using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Serialization;

/// <summary>
/// Converts a <see cref="Size2"/> value to or from JSON.
/// </summary>
public class Size2JsonConverter : JsonConverter<Size2>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Size2);

    /// <inheritdoc />
    /// <exception cref="JsonException">
    /// Thrown if the JSON property does not contain a properly formatted <see cref="Size2"/> value
    /// </exception>
    public override Size2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var values = reader.ReadAsMultiDimensional<float>();

        if (values.Length == 2)
        {
            return new Size2(values[0], values[1]);
        }

        if (values.Length == 1)
        {
            return new Size2(values[0], values[0]);
        }

        throw new JsonException("Invalid Size2 property value");
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// Throw if <paramref name="writer"/> is <see langword="null"/>.
    /// </exception>
    public override void Write(Utf8JsonWriter writer, Size2 value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStringValue($"{value.Width} {value.Height}");
    }
}
