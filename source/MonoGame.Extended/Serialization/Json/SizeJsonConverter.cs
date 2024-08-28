using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Serialization.Json;

/// <summary>
/// Converts a <see cref="Size"/> value to or from JSON.
/// </summary>
public class SizeJsonConverter : JsonConverter<Size>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Size);

    /// <inheritdoc />
    /// <exception cref="JsonException">
    /// Thrown if the JSON property does not contain a properly formatted <see cref="Size"/> value
    /// </exception>
    public override Size Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var values = reader.ReadAsMultiDimensional<int>(options);

        if (values.Length == 2)
        {
            return new Size(values[0], values[1]);
        }

        if (values.Length == 1)
        {
            return new Size(values[0], values[0]);
        }

        throw new JsonException("Invalid Size property value");
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// Throw if <paramref name="writer"/> is <see langword="null"/>.
    /// </exception>
    public override void Write(Utf8JsonWriter writer, Size value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStringValue($"{value.Width} {value.Height}");
    }
}
