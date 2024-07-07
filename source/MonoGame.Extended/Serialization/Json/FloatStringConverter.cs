using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Serialization.Json;

public class FloatStringConverter : JsonConverter<float>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(float) || typeToConvert == typeof(string);

    /// <inheritdoc />
    public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (float.TryParse(reader.GetString(), out float value))
                return value;
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetSingle();
        }

        throw new JsonException($"Unable to convert value of type {reader.TokenType} to {typeof(float)}");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteNumberValue(value);
    }
}
