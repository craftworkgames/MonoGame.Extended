using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Ogmo.Converters;
using MonoGame.Extended.Tilemaps.Ogmo.Document;
using MonoGame.Extended.Tilemaps.Parsers;
using MonoGame.Extended.Tilemaps.Tiled.Converters;
using MonoGame.Extended.Tilemaps.Tiled.Document;
using MonoGame.Extended.Content.Pipeline.Tilemaps.Tiled;

using LDtkHelpers = MonoGame.Extended.Content.Pipeline.Tilemaps.LDtk.LDtkImportHelper;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps;

/// <summary>
/// Imports a tilemap world from a .ldtk, .world, or .tilemapworld file into the content pipeline.
/// </summary>
/// <remarks>
/// Dispatches to the appropriate format-specific loading path based on the file extension.
/// All levels in the world are converted and returned for processing by
/// <see cref="TilemapWorldProcessor"/>.
/// </remarks>
[ContentImporter(".ldtk", ".world", ".tilemapworld", DefaultProcessor = "TilemapWorldProcessor", DisplayName = "Tilemap World Importer - MonoGame.Extended")]
public sealed class TilemapWorldImporter : ContentImporter<TilemapProjectContentItem>
{
    private static readonly JsonSerializerOptions s_jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private static readonly XmlSerializer s_tiledSerializer =
        new XmlSerializer(
            typeof(TiledMapXml),
            new[]
            {
                typeof(TiledTileLayerXml),
                typeof(TiledObjectLayerXml),
                typeof(TiledImageLayerXml),
                typeof(TiledGroupLayerXml)
            }
        );

    /// <inheritdoc/>
    public override TilemapProjectContentItem Import(string filePath, ContentImporterContext context)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(context);

        ContentLogger.Logger = context.Logger;

        string extension = Path.GetExtension(filePath).ToLowerInvariant();

        switch (extension)
        {
            case ".ldtk":
                return ImportLDtk(filePath, context);

            case ".world":
                return ImportTiledWorld(filePath, context);

            case ".tilemapworld":
                return ImportTilemapWorld(filePath, context);

            default:
                throw new InvalidContentException(
                    $"Unsupported world file extension '{extension}'. " +
                    "Expected .ldtk, .world, or .tilemapworld.");
        }
    }

    #region LDtk (.ldtk)

    private static TilemapProjectContentItem ImportLDtk(string filePath, ContentImporterContext context)
    {
        ContentLogger.Log($"Importing LDtk world '{filePath}'");

        List<TilemapData> levels = LDtkHelpers.Import(filePath, context);

        ContentLogger.Log($"Imported {levels.Count} level(s) from LDtk world '{filePath}'");
        return new TilemapProjectContentItem(levels);
    }

    #endregion

    #region Tiled world (.world)

    private static TilemapProjectContentItem ImportTiledWorld(string filePath, ContentImporterContext context)
    {
        ContentLogger.Log($"Importing Tiled world '{filePath}'");

        TiledWorldDocument worldDoc = DeserializeTiledWorld(filePath);
        string worldDirectory = Path.GetDirectoryName(filePath)!;

        if (worldDoc.Maps == null || worldDoc.Maps.Count == 0)
        {
            throw new InvalidContentException(
                $"Tiled world '{Path.GetFileName(filePath)}' contains no map entries.");
        }

        List<TilemapData> levels = new List<TilemapData>();

        foreach (TiledWorldMapEntry entry in worldDoc.Maps)
        {
            if (string.IsNullOrWhiteSpace(entry.FileName))
            {
                continue;
            }

            string mapFilePath = Path.GetFullPath(Path.Combine(worldDirectory, entry.FileName));

            if (!File.Exists(mapFilePath))
            {
                throw new InvalidContentException(
                    $"Tiled world '{Path.GetFileName(filePath)}' references map '{entry.FileName}', " +
                    $"but the file was not found at '{mapFilePath}'.");
            }

            TilemapData data = LoadAndConvertTiledMap(mapFilePath, worldDirectory, context);
            data.WorldX = entry.X;
            data.WorldY = entry.Y;
            levels.Add(data);
        }

        ContentLogger.Log($"Imported {levels.Count} level(s) from Tiled world '{filePath}'");
        return new TilemapProjectContentItem(levels);
    }

    private static TiledWorldDocument DeserializeTiledWorld(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            TiledWorldDocument doc = JsonSerializer.Deserialize<TiledWorldDocument>(json, s_jsonOptions);

            if (doc == null)
            {
                throw new InvalidContentException(
                    $"Failed to parse Tiled world '{Path.GetFileName(filePath)}': deserialization returned null.");
            }

            return doc;
        }
        catch (JsonException ex)
        {
            throw new InvalidContentException(
                $"Failed to parse Tiled world '{Path.GetFileName(filePath)}'. " +
                "Ensure the file is a valid Tiled .world JSON file. " +
                $"Inner error: {ex.Message}", ex);
        }
    }

    private static TilemapData LoadAndConvertTiledMap(string mapFilePath, string baseDirectory, ContentImporterContext context)
    {
        TiledMapXml map = DeserializeTiledMap(mapFilePath);

        if (map.Infinite != 0)
        {
            throw new InvalidContentException(
                $"Map '{Path.GetFileName(mapFilePath)}' uses infinite map mode, which is not supported. " +
                "In Tiled, go to Map > Properties and uncheck 'Infinite'.");
        }

        ResolveTiledTilesetPaths(map, baseDirectory, mapFilePath, context);
        RegisterTiledLayerDependencies(map.Layers, baseDirectory, context);

        try
        {
            TilemapData data = TiledTilemapDataConverter.Convert(map);
            data.Name = Path.GetFileNameWithoutExtension(mapFilePath);
            return data;
        }
        catch (TilemapParseException ex)
        {
            throw new InvalidContentException(ex.Message, ex);
        }
    }

    private static TiledMapXml DeserializeTiledMap(string filePath)
    {
        try
        {
            using StreamReader reader = new StreamReader(filePath);
            return (TiledMapXml)s_tiledSerializer.Deserialize(reader)!;
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidContentException(
                $"Failed to parse map file '{Path.GetFileName(filePath)}'. " +
                "Ensure the file is a valid Tiled TMX map. " +
                $"Inner error: {ex.InnerException?.Message ?? ex.Message}", ex);
        }
    }

    private static void ResolveTiledTilesetPaths(TiledMapXml map, string baseDirectory, string mapFilePath, ContentImporterContext context)
    {
        foreach (TiledTilesetRefXml tilesetRef in map.Tilesets)
        {
            if (string.IsNullOrWhiteSpace(tilesetRef.Source))
            {
                if (tilesetRef.Image != null && !string.IsNullOrWhiteSpace(tilesetRef.Image.Source))
                {
                    string absoluteImagePath = Path.GetFullPath(Path.Combine(baseDirectory, tilesetRef.Image.Source));
                    ContentLogger.Log($"Adding dependency '{absoluteImagePath}'");
                    context.AddDependency(absoluteImagePath);
                    tilesetRef.Image.Source = absoluteImagePath;
                }
            }
            else
            {
                string absoluteTilesetPath = Path.GetFullPath(
                    Path.Combine(baseDirectory, tilesetRef.Source));

                if (!File.Exists(absoluteTilesetPath))
                {
                    throw new InvalidContentException(
                        $"Map '{Path.GetFileName(mapFilePath)}' references external tileset '{tilesetRef.Source}', " +
                        $"but the file was not found at '{absoluteTilesetPath}'.");
                }

                ContentLogger.Log($"Adding dependency '{absoluteTilesetPath}'");
                context.AddDependency(absoluteTilesetPath);
                tilesetRef.Source = absoluteTilesetPath;
            }
        }
    }

    private static void RegisterTiledLayerDependencies(List<TiledLayerXml> layers, string baseDirectory, ContentImporterContext context)
    {
        foreach (TiledLayerXml layer in layers)
        {
            if (layer is TiledImageLayerXml imageLayer)
            {
                if (imageLayer.Image != null && !string.IsNullOrWhiteSpace(imageLayer.Image.Source))
                {
                    string absoluteImagePath = Path.GetFullPath(
                        Path.Combine(baseDirectory, imageLayer.Image.Source));
                    ContentLogger.Log($"Adding dependency '{absoluteImagePath}'");
                    context.AddDependency(absoluteImagePath);
                    imageLayer.Image.Source = absoluteImagePath;
                }
            }
            else if (layer is TiledGroupLayerXml groupLayer)
            {
                RegisterTiledLayerDependencies(groupLayer.Layers, baseDirectory, context);
            }
        }
    }

    #endregion

    #region Generic world (.tilemapworld)

    private static TilemapProjectContentItem ImportTilemapWorld(string filePath, ContentImporterContext context)
    {
        ContentLogger.Log($"Importing tilemap world '{filePath}'");

        TilemapWorldDefinition definition = DeserializeTilemapWorldDefinition(filePath);
        string worldDirectory = Path.GetDirectoryName(filePath)!;

        if (string.IsNullOrWhiteSpace(definition.Format))
        {
            throw new InvalidContentException(
                $"Tilemap world '{Path.GetFileName(filePath)}' is missing the required 'format' field.");
        }

        if (definition.Maps == null || definition.Maps.Count == 0)
        {
            throw new InvalidContentException(
                $"Tilemap world '{Path.GetFileName(filePath)}' contains no map entries.");
        }

        string format = definition.Format.ToLowerInvariant();

        switch (format)
        {
            case "tiled":
                return ImportTilemapWorldTiled(filePath, definition, worldDirectory, context);

            case "ogmo":
                return ImportTilemapWorldOgmo(filePath, definition, worldDirectory, context);

            default:
                throw new InvalidContentException(
                    $"Tilemap world '{Path.GetFileName(filePath)}' uses unsupported format '{definition.Format}'. " +
                    "Supported formats: 'tiled', 'ogmo'.");
        }
    }

    private static TilemapWorldDefinition DeserializeTilemapWorldDefinition(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            TilemapWorldDefinition def = JsonSerializer.Deserialize<TilemapWorldDefinition>(json, s_jsonOptions);

            if (def == null)
            {
                throw new InvalidContentException(
                    $"Failed to parse tilemap world '{Path.GetFileName(filePath)}': deserialization returned null.");
            }

            return def;
        }
        catch (JsonException ex)
        {
            throw new InvalidContentException(
                $"Failed to parse tilemap world '{Path.GetFileName(filePath)}'. " +
                "Ensure the file is a valid .tilemapworld JSON file. " +
                $"Inner error: {ex.Message}", ex);
        }
    }

    private static TilemapProjectContentItem ImportTilemapWorldTiled(string worldFilePath, TilemapWorldDefinition definition, string worldDirectory, ContentImporterContext context)
    {
        List<TilemapData> levels = new List<TilemapData>();

        foreach (TilemapWorldDefinitionMap entry in definition.Maps)
        {
            if (string.IsNullOrWhiteSpace(entry.Source))
            {
                continue;
            }

            string mapFilePath = Path.GetFullPath(Path.Combine(worldDirectory, entry.Source));

            if (!File.Exists(mapFilePath))
            {
                throw new InvalidContentException(
                    $"Tilemap world '{Path.GetFileName(worldFilePath)}' references map '{entry.Source}', " +
                    $"but the file was not found at '{mapFilePath}'.");
            }

            TilemapData data = LoadAndConvertTiledMap(mapFilePath, worldDirectory, context);
            data.WorldX = entry.X;
            data.WorldY = entry.Y;
            data.WorldDepth = entry.Depth;
            levels.Add(data);
        }

        ContentLogger.Log($"Imported {levels.Count} Tiled level(s) from tilemap world '{worldFilePath}'");
        return new TilemapProjectContentItem(levels);
    }

    private static TilemapProjectContentItem ImportTilemapWorldOgmo(string worldFilePath, TilemapWorldDefinition definition, string worldDirectory, ContentImporterContext context)
    {
        if (string.IsNullOrWhiteSpace(definition.Project))
        {
            throw new InvalidContentException(
                $"Tilemap world '{Path.GetFileName(worldFilePath)}' with format 'ogmo' " +
                "requires a 'project' field pointing to the .ogmo project file.");
        }

        string projectFilePath = Path.GetFullPath(Path.Combine(worldDirectory, definition.Project));

        if (!File.Exists(projectFilePath))
        {
            throw new InvalidContentException(
                $"Tilemap world '{Path.GetFileName(worldFilePath)}' references Ogmo project '{definition.Project}', " +
                $"but the file was not found at '{projectFilePath}'.");
        }

        OgmoProject project = DeserializeOgmoProject(projectFilePath);
        string projectDirectory = Path.GetDirectoryName(projectFilePath)!;

        RegisterOgmoTilesetDependencies(project, projectDirectory, context);

        List<TilemapData> levels = new List<TilemapData>();

        foreach (TilemapWorldDefinitionMap entry in definition.Maps)
        {
            if (string.IsNullOrWhiteSpace(entry.Source))
            {
                continue;
            }

            string levelFilePath = Path.GetFullPath(Path.Combine(worldDirectory, entry.Source));

            if (!File.Exists(levelFilePath))
            {
                throw new InvalidContentException(
                    $"Tilemap world '{Path.GetFileName(worldFilePath)}' references level '{entry.Source}', " +
                    $"but the file was not found at '{levelFilePath}'.");
            }

            ContentLogger.Log($"Adding level dependency '{levelFilePath}'");
            context.AddDependency(levelFilePath);

            OgmoLevel level = LoadOgmoLevel(levelFilePath);

            try
            {
                TilemapData data = OgmoTilemapDataConverter.Convert(level, project);
                data.Name = Path.GetFileNameWithoutExtension(levelFilePath);
                data.WorldX = entry.X;
                data.WorldY = entry.Y;
                data.WorldDepth = entry.Depth;
                levels.Add(data);
            }
            catch (TilemapParseException ex)
            {
                throw new InvalidContentException(ex.Message, ex);
            }
        }

        ContentLogger.Log($"Imported {levels.Count} Ogmo level(s) from tilemap world '{worldFilePath}'");
        return new TilemapProjectContentItem(levels);
    }

    private static OgmoProject DeserializeOgmoProject(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            OgmoProject project = JsonSerializer.Deserialize<OgmoProject>(json, s_jsonOptions);

            if (project == null)
            {
                throw new InvalidContentException(
                    $"Failed to parse Ogmo project '{Path.GetFileName(filePath)}': deserialization returned null.");
            }

            return project;
        }
        catch (JsonException ex)
        {
            throw new InvalidContentException(
                $"Failed to parse Ogmo project '{Path.GetFileName(filePath)}'. " +
                "Ensure the file is a valid Ogmo Editor .ogmo JSON file. " +
                $"Inner error: {ex.Message}", ex);
        }
    }

    private static void RegisterOgmoTilesetDependencies(OgmoProject project, string projectDirectory, ContentImporterContext context)
    {
        if (project.Tilesets == null)
        {
            return;
        }

        foreach (OgmoTilesetTemplate tilesetTemplate in project.Tilesets)
        {
            string imagePath = tilesetTemplate.Image ?? tilesetTemplate.Path;
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                continue;
            }

            if (imagePath.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
            {
                // Embedded images are already extracted to disk by OgmoTilemapImporter if the
                // .ogmo file was imported separately. Skip dependency registration for data URIs.
                continue;
            }

            string absoluteImagePath = Path.GetFullPath(Path.Combine(projectDirectory, imagePath));
            tilesetTemplate.Image = absoluteImagePath;
            ContentLogger.Log($"Adding tileset dependency '{absoluteImagePath}'");
            context.AddDependency(absoluteImagePath);
        }
    }

    private static OgmoLevel LoadOgmoLevel(string levelFilePath)
    {
        try
        {
            string json = File.ReadAllText(levelFilePath);
            return JsonSerializer.Deserialize<OgmoLevel>(json, s_jsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidContentException(
                $"Failed to parse Ogmo level file '{Path.GetFileName(levelFilePath)}'. " +
                $"Inner error: {ex.Message}", ex);
        }
    }

    #endregion
}
