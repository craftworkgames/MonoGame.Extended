using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Serialization;

/// <summary>
/// Converts a <see cref="TextureRegion2D"/> value to or from JSON.
/// </summary>
public class TextureRegion2DJsonConverter : JsonConverter<TextureRegion2D>
{
    private readonly ITextureRegionService _textureRegionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureRegion2DJsonConverter"/> class.
    /// </summary>
    /// <param name="textureRegionService">The texture region service to use for retrieving texture regions.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="textureRegionService"/> is <see langword="null"/>.
    /// </exception>
    public TextureRegion2DJsonConverter(ITextureRegionService textureRegionService)
    {
        ArgumentNullException.ThrowIfNull(textureRegionService);
        _textureRegionService = textureRegionService;
    }

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(TextureRegion2D);

    /// <inheritdoc />
    public override TextureRegion2D Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var regionName = reader.GetString();
        return string.IsNullOrEmpty(regionName) ? null : _textureRegionService.GetTextureRegion(regionName);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// Throw if <paramref name="writer"/> is <see langword="null"/>.
    ///
    /// -or-
    ///
    /// Thrown if <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    public override void Write(Utf8JsonWriter writer, TextureRegion2D value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);
        writer.WriteStringValue(value.Name);
    }
}
