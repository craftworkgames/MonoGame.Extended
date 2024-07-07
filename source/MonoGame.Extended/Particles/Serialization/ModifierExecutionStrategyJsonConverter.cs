using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGame.Extended.Particles.Serialization;

/// <summary>
/// Converts a <see cref="ParticleModifierExecutionStrategy"/> value to or from JSON.
/// </summary>
public class ModifierExecutionStrategyJsonConverter : JsonConverter<ParticleModifierExecutionStrategy>
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert == typeof(ParticleModifierExecutionStrategy) ||
        typeToConvert.GetTypeInfo().BaseType == typeof(ParticleModifierExecutionStrategy);

    /// <inheritdoc />
    public override ParticleModifierExecutionStrategy Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = JsonSerializer.Deserialize<string>(ref reader, options);
        return ParticleModifierExecutionStrategy.Parse(value);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// Throw if <paramref name="writer"/> is <see langword="null"/>.
    /// </exception>
    public override void Write(Utf8JsonWriter writer, ParticleModifierExecutionStrategy value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStringValue(value.ToString());
    }
}
