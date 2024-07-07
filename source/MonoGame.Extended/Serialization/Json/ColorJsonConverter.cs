using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Serialization.Json;

/// <summary>
/// Converts a <see cref="Color"/> value to or from JSON.
/// </summary>
public class ColorJsonConverter : JsonConverter<Color>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Color);

    /// <inheritdoc />
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value[0] == '#' ? ColorHelper.FromHex(value) : ColorHelper.FromName(value);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// Throw if <paramref name="writer"/> is <see langword="null"/>.
    /// </exception>
    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        var hexValue = ColorHelper.ToHex(value);
        writer.WriteStringValue(hexValue);
    }
}

