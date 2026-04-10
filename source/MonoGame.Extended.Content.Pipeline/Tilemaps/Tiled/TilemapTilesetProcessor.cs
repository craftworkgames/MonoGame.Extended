using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps.Tiled;

/// <summary>
/// Processes an imported Tiled tileset, building the tileset texture as an external pipeline asset.
/// </summary>
[ContentProcessor(DisplayName = "Tilemap Tileset Processor - MonoGame.Extended")]
public sealed class TilemapTilesetProcessor : ContentProcessor<TilemapTilesetContentItem, TilemapTilesetContentItem>
{
    /// <inheritdoc/>
    public override TilemapTilesetContentItem Process(
        TilemapTilesetContentItem contentItem,
        ContentProcessorContext context)
    {
        ArgumentNullException.ThrowIfNull(contentItem);
        ArgumentNullException.ThrowIfNull(context);

        ContentLogger.Logger = context.Logger;
        ContentLogger.Log($"Processing tileset '{contentItem.Data.Name}'");

        if (!string.IsNullOrWhiteSpace(contentItem.Data.TexturePath))
        {
            ContentLogger.Log($"Building texture '{contentItem.Data.TexturePath}'");
            contentItem.BuildExternalReference<Texture2DContent>(context, contentItem.Data.TexturePath);
        }

        foreach (TilemapTileEntryData tile in contentItem.Data.Tiles)
        {
            if (!string.IsNullOrWhiteSpace(tile.ImagePath))
            {
                ContentLogger.Log($"Building tile image '{tile.ImagePath}'");
                contentItem.BuildExternalReference<Texture2DContent>(context, tile.ImagePath);
            }
        }

        ContentLogger.Log($"Processed tileset '{contentItem.Data.Name}'");
        return contentItem;
    }
}
