using System;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tilemaps.Content;

/// <summary>
/// Reads a binary tilemap asset produced by the content pipeline into a <see cref="Tilemap"/>.
/// </summary>
public sealed class TilemapReader : ContentTypeReader<Tilemap>
{
    /// <inheritdoc/>
    protected override Tilemap Read(ContentReader reader, Tilemap existingInstance)
    {
        ArgumentNullException.ThrowIfNull(reader);
        return TilemapReadHelper.ReadMap(reader);
    }
}
