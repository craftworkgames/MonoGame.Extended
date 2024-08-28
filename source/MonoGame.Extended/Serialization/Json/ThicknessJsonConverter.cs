using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Serialization.Json;

/// <summary>
/// Converts a <see cref="Thickness"/> value to or from JSON.
/// </summary>
public class ThicknessJsonConverter : JsonConverter<Thickness>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Thickness);

    /// <inheritdoc />
    public override Thickness Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var values = reader.ReadAsMultiDimensional<int>(options);
        return Thickness.FromValues(values);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// Throw if <paramref name="writer"/> is <see langword="null"/>
    /// </exception>
    public override void Write(Utf8JsonWriter writer, Thickness value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStringValue($"{value.Left} {value.Top} {value.Right} {value.Bottom}");
    }
}
