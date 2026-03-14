using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps;

/// <summary>
/// Writes a processed tilemap world to binary content. Accepts the
/// <see cref="TilemapWorldContentItem"/> produced by <see cref="TilemapWorldProcessor"/>.
/// </summary>
[ContentTypeWriter]
internal sealed class TilemapWorldWriter : ContentTypeWriter<TilemapWorldContentItem>
{
    /// <inheritdoc/>
    protected override void Write(ContentWriter writer, TilemapWorldContentItem contentItem)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(contentItem);

        List<TilemapData> levels = contentItem.Data;
        writer.Write(levels.Count);

        foreach (TilemapData level in levels)
        {
            // Write name explicitly so the reader can restore it without relying on AssetName.
            writer.Write(level.Name ?? string.Empty);
            TilemapWriteHelper.WriteMap(writer, level, contentItem);
        }
    }

    /// <inheritdoc/>
    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return typeof(TilemapWorld).AssemblyQualifiedName;
    }

    /// <inheritdoc/>
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return "MonoGame.Extended.Tilemaps.Content.TilemapWorldReader, MonoGame.Extended";
    }
}
