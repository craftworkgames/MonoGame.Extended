using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Serialization.Json;

/// <summary>
/// Converts a <see cref="Range{T}"/> value to or from JSON.
/// </summary>
public class RangeJsonConverter<T> : JsonConverter<Range<T>> where T : IComparable<T>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Range<T>);

    /// <inheritdoc />
    public override Range<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Span<T> values = reader.ReadAsMultiDimensional<T>(options);

        if (values.Length == 2)
        {
            if (values[0].CompareTo(values[1]) < 0)
            {
                return new Range<T>(values[0], values[1]);
            }

            return new Range<T>(values[1], values[0]);
        }

        if (values.Length == 1)
        {
            return new Range<T>(values[0], values[0]);
        }

        throw new InvalidOperationException("Invalid range");
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// Throw if <paramref name="writer"/> is <see langword="null"/>.
    /// </exception>
    public override void Write(Utf8JsonWriter writer, Range<T> value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStartArray();
        JsonSerializer.Serialize(writer, value.Min, options);
        JsonSerializer.Serialize(writer, value.Max, options);
        writer.WriteEndArray();
    }
}
