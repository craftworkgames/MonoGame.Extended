using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
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

#if KNI || FNA
            // KNI and FNA do not use the new external ref calls from MonoGame's new
            // content builder project
            contentItem.BuildExternalReference<Texture2DContent>(context, contentItem.Data.TexturePath);
#else
            contentItem.BuildExternalReference<TextureContent, Texture2DContent>(
                context,
                contentItem.Data.TexturePath,
                new TextureImporter(),
                new TextureProcessor());
#endif
        }

        foreach (TilemapTileEntryData tile in contentItem.Data.Tiles)
        {
            if (!string.IsNullOrWhiteSpace(tile.ImagePath))
            {
                ContentLogger.Log($"Building tile image '{tile.ImagePath}'");

#if KNI || FNA
                // KNI and FNA do not use the new external ref calls from MonoGame's new
                // content builder project
                contentItem.BuildExternalReference<Texture2DContent>(context, tile.ImagePath);
#else                
                contentItem.BuildExternalReference<TextureContent, Texture2DContent>(
                    context,
                    tile.ImagePath,
                    new TextureImporter(),
                    new TextureProcessor());
#endif
            }
        }

        ContentLogger.Log($"Processed tileset '{contentItem.Data.Name}'");
        return contentItem;
    }
}
