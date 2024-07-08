using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Serialization.Json;

/// <summary>
/// Converts a <see cref="HslColor"/> value to or from JSON.
/// </summary>
public class HslColorJsonConverter : JsonConverter<HslColor>
{
    private readonly ColorJsonConverter _colorConverter = new ColorJsonConverter();

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(HslColor);

    /// <inheritdoc />
    public override HslColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var color = _colorConverter.Read(ref reader, typeToConvert, options);
        return HslColor.FromRgb(color);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// Throw if <paramref name="writer"/> is <see langword="null"/>.
    /// </exception>
    public override void Write(Utf8JsonWriter writer, HslColor value, JsonSerializerOptions options)
    {
        var color = ((HslColor)value).ToRgb();
        _colorConverter.Write(writer, color, options);
    }
}
