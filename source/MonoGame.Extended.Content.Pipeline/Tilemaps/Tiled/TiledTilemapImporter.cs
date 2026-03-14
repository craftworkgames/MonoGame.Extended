using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Parsers;
using MonoGame.Extended.Tilemaps.Tiled.Converters;
using MonoGame.Extended.Tilemaps.Tiled.Document;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps.Tiled;

/// <summary>
/// Imports a Tiled map from a TMX file into the content pipeline.
/// </summary>
/// <remarks>
/// Converts the map and registers all dependencies (external tilesets, tileset images, image layer
/// images) so that the map is automatically rebuilt whenever any referenced asset changes.
/// </remarks>
[ContentImporter(".tmx", DefaultProcessor = "TilemapProcessor", DisplayName = "Tiled Tilemap Importer - MonoGame.Extended")]
public sealed class TiledTilemapImporter : ContentImporter<TilemapProjectContentItem>
{
    private static readonly XmlSerializer s_serializer = new XmlSerializer(
        typeof(TiledMapXml),
        new[]
        {
            typeof(TiledTileLayerXml),
            typeof(TiledObjectLayerXml),
            typeof(TiledImageLayerXml),
            typeof(TiledGroupLayerXml)
        });

    /// <inheritdoc/>
    public override TilemapProjectContentItem Import(string filePath, ContentImporterContext context)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(context);

        ContentLogger.Logger = context.Logger;
        ContentLogger.Log($"Importing Tiled map '{filePath}'");

        TiledMapXml map = DeserializeMap(filePath);
        string sourceDirectory = Path.GetDirectoryName(filePath)!;

        if (map.Infinite != 0)
        {
            throw new InvalidContentException(
                $"Map '{Path.GetFileName(filePath)}' uses infinite map mode, which is not supported. " +
                "In Tiled, go to Map > Properties and uncheck 'Infinite'.");
        }

        ResolveTilesetPaths(map, sourceDirectory, filePath, context);
        RegisterLayerDependencies(map.Layers, sourceDirectory, context);

        TilemapData tilemapData;

        try
        {
            tilemapData = TiledTilemapDataConverter.Convert(map);
        }
        catch (TilemapParseException ex)
        {
            throw new InvalidContentException(ex.Message, ex);
        }

        tilemapData.Name = Path.GetFileNameWithoutExtension(filePath);

        ContentLogger.Log($"Imported Tiled map '{filePath}'");
        return new TilemapProjectContentItem(new List<TilemapData> { tilemapData });
    }

    private static TiledMapXml DeserializeMap(string filePath)
    {
        try
        {
            using StreamReader reader = new StreamReader(filePath);
            return (TiledMapXml)s_serializer.Deserialize(reader)!;
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidContentException(
                $"Failed to parse map file '{Path.GetFileName(filePath)}'. " +
                "Ensure the file is a valid Tiled TMX map. " +
                $"Inner error: {ex.InnerException?.Message ?? ex.Message}", ex);
        }
    }

    private static void ResolveTilesetPaths(TiledMapXml map, string sourceDirectory, string mapFilePath, ContentImporterContext context)
    {
        foreach (TiledTilesetRefXml tilesetRef in map.Tilesets)
        {
            if (string.IsNullOrWhiteSpace(tilesetRef.Source))
            {
                // Inline tileset: register the tileset image as a dependency.
                if (tilesetRef.Image != null && !string.IsNullOrWhiteSpace(tilesetRef.Image.Source))
                {
                    string absoluteImagePath = Path.GetFullPath(
                        Path.Combine(sourceDirectory, tilesetRef.Image.Source));
                    ContentLogger.Log($"Adding dependency '{absoluteImagePath}'");
                    context.AddDependency(absoluteImagePath);
                    tilesetRef.Image.Source = absoluteImagePath;
                }
            }
            else
            {
                // External tileset: register the TSX file and its image as dependencies.
                string absoluteTilesetPath = Path.GetFullPath(
                    Path.Combine(sourceDirectory, tilesetRef.Source));

                if (!File.Exists(absoluteTilesetPath))
                {
                    throw new InvalidContentException(
                        $"Map '{Path.GetFileName(mapFilePath)}' references external tileset '{tilesetRef.Source}', " +
                        $"but the file was not found at '{absoluteTilesetPath}'. " +
                        "Ensure the TSX file exists and the path is correct relative to the TMX file.");
                }

                ContentLogger.Log($"Adding dependency '{absoluteTilesetPath}'");
                context.AddDependency(absoluteTilesetPath);
                tilesetRef.Source = absoluteTilesetPath;
            }
        }
    }

    private static void RegisterLayerDependencies(List<TiledLayerXml> layers, string sourceDirectory, ContentImporterContext context)
    {
        foreach (TiledLayerXml layer in layers)
        {
            if (layer is TiledImageLayerXml imageLayer)
            {
                RegisterImageLayerDependency(imageLayer, sourceDirectory, context);
            }
            else if (layer is TiledGroupLayerXml groupLayer)
            {
                RegisterLayerDependencies(groupLayer.Layers, sourceDirectory, context);
            }
        }
    }

    private static void RegisterImageLayerDependency(TiledImageLayerXml imageLayer, string sourceDirectory, ContentImporterContext context)
    {
        if (imageLayer.Image == null || string.IsNullOrWhiteSpace(imageLayer.Image.Source))
        {
            return;
        }

        string absoluteImagePath = Path.GetFullPath(Path.Combine(sourceDirectory, imageLayer.Image.Source));
        ContentLogger.Log($"Adding dependency '{absoluteImagePath}'");
        context.AddDependency(absoluteImagePath);
        imageLayer.Image.Source = absoluteImagePath;
    }
}
