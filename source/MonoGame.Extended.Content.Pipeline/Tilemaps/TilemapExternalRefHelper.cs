using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps;

/// <summary>
/// Shared helper for registering external asset references on a content item during processing.
/// </summary>
internal static class TilemapExternalRefHelper
{
    /// <summary>
    /// Registers all external references (textures, external tilesets) required by a single
    /// tilemap level on the given content item.
    /// </summary>
    internal static void Register<T>(TilemapData data, ContentItem<T> item, ContentProcessorContext ctx)
    {
        RegisterTilesetRefs(data.Tilesets, item, ctx);
        RegisterLayerRefs(data.Layers, item, ctx);
    }

    private static void RegisterTilesetRefs<T>(IReadOnlyList<TilemapTilesetEntry> tilesets, ContentItem<T> item, ContentProcessorContext ctx)
    {
        foreach (TilemapTilesetEntry entry in tilesets)
        {
            if (entry.IsExternal && !string.IsNullOrEmpty(entry.ExternalPath))
            {
                // Only Tiled uses external .tsx tileset references.
                BuildTilesetIfAbsent(item, ctx, entry.ExternalPath);
            }
            else if (entry.InlineData != null && !string.IsNullOrEmpty(entry.InlineData.TexturePath))
            {
                BuildTextureIfAbsent(item, ctx, entry.InlineData.TexturePath);
            }
        }
    }

    private static void RegisterLayerRefs<T>(List<TilemapLayerData> layers, ContentItem<T> item, ContentProcessorContext ctx)
    {
        foreach (TilemapLayerData layer in layers)
        {
            if (layer is TilemapImageLayerData imgLayer && !string.IsNullOrEmpty(imgLayer.TexturePath))
            {
                BuildTextureIfAbsent(item, ctx, imgLayer.TexturePath);
            }
            else if (layer is TilemapGroupLayerData groupLayer)
            {
                RegisterLayerRefs(groupLayer.Layers, item, ctx);
            }
        }
    }

    private static void BuildTextureIfAbsent<T>(ContentItem<T> item, ContentProcessorContext ctx, string path)
    {
        if (item.GetExternalReference<Texture2DContent>(path) == null)
        {
            ContentLogger.Log($"Building texture reference '{path}'");
            item.BuildExternalReference<Texture2DContent>(ctx, path);
        }
    }

    private static void BuildTilesetIfAbsent<T>(ContentItem<T> item, ContentProcessorContext ctx, string path)
    {
        if (item.GetExternalReference<TilemapTilesetData>(path) == null)
        {
            ContentLogger.Log($"Building external tileset reference '{path}'");
            item.BuildExternalReference<TilemapTilesetData>(ctx, path);
        }
    }
}
