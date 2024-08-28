using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Serialization.Json;

/// <summary>
/// Converts a <see cref="SizeF"/> value to or from JSON.
/// </summary>
public class Size2JsonConverter : JsonConverter<SizeF>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(SizeF);

    /// <inheritdoc />
    /// <exception cref="JsonException">
    /// Thrown if the JSON property does not contain a properly formatted <see cref="SizeF"/> value
    /// </exception>
    public override SizeF Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var values = reader.ReadAsMultiDimensional<float>(options);

        if (values.Length == 2)
        {
            return new SizeF(values[0], values[1]);
        }

        if (values.Length == 1)
        {
            return new SizeF(values[0], values[0]);
        }

        throw new JsonException("Invalid Size2 property value");
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// Throw if <paramref name="writer"/> is <see langword="null"/>.
    /// </exception>
    public override void Write(Utf8JsonWriter writer, SizeF value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStringValue($"{value.Width} {value.Height}");
    }
}
