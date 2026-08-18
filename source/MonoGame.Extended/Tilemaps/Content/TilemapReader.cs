using System;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tilemaps.Content;

/// <summary>
/// Reads a binary tilemap asset produced by the content pipeline into a <see cref="Tilemap"/>.
/// </summary>
public sealed class TilemapReader : ContentTypeReader<Tilemap>
{
    internal static readonly string NativeAotRegistrationKey =
        $"{typeof(TilemapReader).FullName}, {typeof(TilemapReader).Assembly.GetName().Name}";

#if !FNA && !KNI
    /// <summary>
    /// Registers this <see cref="ContentTypeReader"/> with the <see cref="ContentTypeReaderManager"/>
    /// so it is resolved without reflection.
    /// </summary>
    /// <remarks>
    /// Call this method once during application startup when publishing with
    /// <c>PublishAot</c> or <c>PublishTrimmed</c>.
    /// </remarks>
    public static void Register() =>
        ContentTypeReaderManager.AddTypeCreator(
            NativeAotRegistrationKey,
            () => new TilemapReader());
#endif

    /// <inheritdoc/>
    protected override Tilemap Read(ContentReader reader, Tilemap existingInstance)
    {
        ArgumentNullException.ThrowIfNull(reader);
        return TilemapReadHelper.ReadMap(reader);
    }
}
