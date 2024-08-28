using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Serialization.Json;

/// <summary>
/// Converts a <see cref="Vector2"/> value to or from JSON.
/// </summary>
public class Vector2JsonConverter : JsonConverter<Vector2>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Vector2);

    /// <inheritdoc />
    /// <exception cref="JsonException">
    /// Thrown if the JSON property does not contain a properly formatted <see cref="Vector2"/> value
    /// </exception>
    public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var values = reader.ReadAsMultiDimensional<float>(options);

        if (values.Length == 2)
        {
            return new Vector2(values[0], values[1]);
        }

        if (values.Length == 1)
        {
            return new Vector2(values[0]);
        }

        throw new JsonException("Invalid Vector2 property value");
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// Throw if <paramref name="writer"/> is <see langword="null"/>.
    /// </exception>
    public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStringValue($"{value.X} {value.Y}");
    }
}
