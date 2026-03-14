using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tilemaps.Content;

/// <summary>
/// Reads a binary tilemap world asset produced by the content pipeline into a <see cref="TilemapWorld"/>.
/// </summary>
public sealed class TilemapWorldReader : ContentTypeReader<TilemapWorld>
{
    /// <inheritdoc/>
    protected override TilemapWorld Read(ContentReader reader, TilemapWorld existingInstance)
    {
        ArgumentNullException.ThrowIfNull(reader);

        int count = reader.ReadInt32();
        List<Tilemap> levels = new List<Tilemap>(count);

        for (int i = 0; i < count; i++)
        {
            levels.Add(TilemapReadHelper.ReadMapWithName(reader));
        }

        return new TilemapWorld(levels);
    }
}
