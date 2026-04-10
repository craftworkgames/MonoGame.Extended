using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Tiled.Converters;
using MonoGame.Extended.Tilemaps.Tiled.Document;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps.Tiled;

/// <summary>
/// Imports a Tiled tileset from a TSX file into the content pipeline.
/// </summary>
[ContentImporter(".tsx", DefaultProcessor = "TilemapTilesetProcessor", DisplayName = "Tilemap Tileset Importer - MonoGame.Extended")]
public sealed class TilemapTilesetImporter : ContentImporter<TilemapTilesetContentItem>
{
    private static readonly XmlSerializer s_serializer = new XmlSerializer(typeof(TiledTilesetXml));

    /// <inheritdoc/>
    public override TilemapTilesetContentItem Import(string filePath, ContentImporterContext context)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(context);

        ContentLogger.Logger = context.Logger;
        ContentLogger.Log($"Importing tileset '{filePath}'");

        TiledTilesetXml tileset = DeserializeTileset(filePath);
        string sourceDirectory = Path.GetDirectoryName(filePath)!;

        RegisterImageDependency(tileset, sourceDirectory, filePath, context);

        TilemapTilesetData tilesetData = TiledTilemapDataConverter.ConvertTilesetData(tileset);

        ContentLogger.Log($"Imported tileset '{filePath}'");
        return new TilemapTilesetContentItem(tilesetData, sourceDirectory);
    }

    private static TiledTilesetXml DeserializeTileset(string filePath)
    {
        try
        {
            using StreamReader reader = new StreamReader(filePath);
            return (TiledTilesetXml)s_serializer.Deserialize(reader)!;
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidContentException(
                $"Failed to parse tileset file '{Path.GetFileName(filePath)}'. " +
                "Ensure the file is a valid Tiled TSX tileset. " +
                $"Inner error: {ex.InnerException?.Message ?? ex.Message}", ex);
        }
    }

    private static void RegisterImageDependency(TiledTilesetXml tileset, string sourceDirectory, string filePath, ContentImporterContext context)
    {
        if (tileset.Image == null)
        {
            RegisterCollectionTileImageDependencies(tileset, sourceDirectory, filePath, context);
            return;
        }

        if (string.IsNullOrWhiteSpace(tileset.Image.Source))
        {
            throw new InvalidContentException(
                $"Tileset '{Path.GetFileName(filePath)}' has an image element but no source path. " +
                "Open the tileset in Tiled and verify the image is correctly referenced.");
        }

        string absoluteImagePath = Path.GetFullPath(Path.Combine(sourceDirectory, tileset.Image.Source));
        ContentLogger.Log($"Adding dependency '{absoluteImagePath}'");
        context.AddDependency(absoluteImagePath);

        // Store the absolute path so ConvertTilesetData picks it up directly.
        tileset.Image.Source = absoluteImagePath;
    }

    private static void RegisterCollectionTileImageDependencies(TiledTilesetXml tileset, string sourceDirectory, string filePath, ContentImporterContext context)
    {
        if (tileset.Tiles == null)
        {
            return;
        }

        foreach (TiledTileXml tile in tileset.Tiles)
        {
            if (tile.Image == null || string.IsNullOrWhiteSpace(tile.Image.Source))
            {
                continue;
            }

            string absoluteTileImagePath = Path.GetFullPath(Path.Combine(sourceDirectory, tile.Image.Source));
            ContentLogger.Log($"Adding dependency '{absoluteTileImagePath}'");
            context.AddDependency(absoluteTileImagePath);

            // Store the absolute path so ConvertTileEntries picks it up directly.
            tile.Image.Source = absoluteTileImagePath;
        }
    }
}
