using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Serialization.Json;

/// <summary>
/// Loads content from a JSON file into the <see cref="ContentManager"/> using the asset name
/// </summary>
/// <typeparam name="T">The type of content to load</typeparam>
public class ContentManagerJsonConverter<T> : JsonConverter<T>
{
    private readonly ContentManager _contentManager;
    private readonly Func<T, string> _getAssetName;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentManagerJsonConverter{T}"/> class.
    /// </summary>
    /// <param name="contentManager">The <see cref="ContentManager"/> used to load content.</param>
    /// <param name="getAssetName">A function that returns the asset name for a given instance of <typeparamref name="T"/>.</param>
    public ContentManagerJsonConverter(ContentManager contentManager, Func<T, string> getAssetName)
    {
        _contentManager = contentManager;
        _getAssetName = getAssetName;
    }

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(T);

    /// <inheritdoc />
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var assetName = reader.GetString();
        return _contentManager.Load<T>(assetName);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// Throw if <paramref name="writer"/> is <see langword="null"/>.
    /// </exception>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        var asset = value;
        var assetName = _getAssetName(asset);
        writer.WriteStringValue(assetName);
    }
}
