using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Ogmo;
using MonoGame.Extended.Tilemaps.Ogmo.Converters;
using MonoGame.Extended.Tilemaps.Ogmo.Document;
using MonoGame.Extended.Tilemaps.Parsers;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps.Ogmo;

/// <summary>
/// Imports an Ogmo Editor 3 project from a .ogmo file into the content pipeline.
/// </summary>
/// <remarks>
/// Discovers all level files in the project's configured level paths, converts them, and registers
/// dependencies so the map is rebuilt whenever any referenced asset changes.
/// Set the 'Level Name' processor property to the filename (without extension) of the level to
/// build. If the property is empty, the first discovered level is used.
/// When a tileset image is embedded as a base64 data URI in the project file, the importer
/// extracts and writes it to a PNG file alongside the .ogmo file so the content pipeline
/// can reference it as a texture asset.
/// </remarks>
[ContentImporter(".ogmo", DefaultProcessor = "TilemapProcessor", DisplayName = "Ogmo Tilemap Importer - MonoGame.Extended")]
internal sealed class OgmoTilemapImporter : ContentImporter<TilemapProjectContentItem>
{
    /// <inheritdoc/>
    public override TilemapProjectContentItem Import(string filePath, ContentImporterContext context)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(context);

        ContentLogger.Logger = context.Logger;
        ContentLogger.Log($"Importing Ogmo project '{filePath}'");

        OgmoProject project = DeserializeProject(filePath);
        string sourceDirectory = Path.GetDirectoryName(filePath)!;

        RegisterTilesetDependencies(project, sourceDirectory, context);

        Dictionary<string, OgmoLevel> discoveredLevels = new Dictionary<string, OgmoLevel>();
        DiscoverAndRegisterLevelFiles(project, sourceDirectory, context, discoveredLevels);

        List<TilemapData> levels = ConvertLevels(project, discoveredLevels);

        ContentLogger.Log($"Imported {levels.Count} level(s) from Ogmo project '{filePath}'");
        return new TilemapProjectContentItem(levels);
    }

    private static OgmoProject DeserializeProject(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            OgmoProject project = JsonSerializer.Deserialize<OgmoProject>(json, OgmoJsonSerializerContext.Default.OgmoProject);

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

    private static void RegisterTilesetDependencies(OgmoProject project, string sourceDirectory, ContentImporterContext context)
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

            string absoluteImagePath;

            if (imagePath.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
            {
                // Ogmo embeds tileset images as base64 data URIs. Extract to a real file
                // so the content pipeline can reference and process the texture normally.
                absoluteImagePath = ExtractEmbeddedImage(imagePath, tilesetTemplate.Label, sourceDirectory);
                tilesetTemplate.Image = absoluteImagePath;
            }
            else
            {
                absoluteImagePath = Path.GetFullPath(
                    Path.Combine(sourceDirectory, imagePath));
                tilesetTemplate.Image = absoluteImagePath;
            }

            ContentLogger.Log($"Adding tileset dependency '{absoluteImagePath}'");
            context.AddDependency(absoluteImagePath);
        }
    }

    private static string ExtractEmbeddedImage(string dataUri, string label, string directory)
    {
        int commaIndex = dataUri.IndexOf(',');
        if (commaIndex < 0)
        {
            throw new InvalidContentException(
                $"Tileset '{label}' has an invalid embedded image data URI: missing comma separator.");
        }

        byte[] imageBytes;
        try
        {
            string base64Data = dataUri.Substring(commaIndex + 1);
            imageBytes = Convert.FromBase64String(base64Data);
        }
        catch (FormatException ex)
        {
            throw new InvalidContentException(
                $"Tileset '{label}' has an invalid embedded image data URI: {ex.Message}", ex);
        }

        string safeName = string.IsNullOrEmpty(label)
            ? "tileset_embedded"
            : string.Concat(label.Split(Path.GetInvalidFileNameChars()));

        string outputPath = Path.Combine(directory, safeName + ".png");
        File.WriteAllBytes(outputPath, imageBytes);
        ContentLogger.Log($"Extracted embedded tileset image '{label}' to '{outputPath}'");
        return outputPath;
    }

    private static void DiscoverAndRegisterLevelFiles(OgmoProject project, string sourceDirectory, ContentImporterContext context, Dictionary<string, OgmoLevel> discoveredLevels)
    {
        if (project.LevelPaths == null || project.LevelPaths.Count == 0)
        {
            ContentLogger.Log("Ogmo project has no LevelPaths configured; no level files will be discovered.");
            return;
        }

        int discoveredCount = 0;

        foreach (string levelPath in project.LevelPaths)
        {
            string absoluteDir = Path.GetFullPath(
                Path.Combine(sourceDirectory, levelPath));

            if (!Directory.Exists(absoluteDir))
            {
                ContentLogger.Log($"Level directory '{absoluteDir}' does not exist; skipping.");
                continue;
            }

            foreach (string levelFile in Directory.GetFiles(absoluteDir, "*.json"))
            {
                ContentLogger.Log($"Adding level dependency '{levelFile}'");
                context.AddDependency(levelFile);

                OgmoLevel level = LoadLevel(levelFile);
                if (level != null)
                {
                    string levelName = Path.GetFileNameWithoutExtension(levelFile);
                    discoveredLevels[levelName] = level;
                    discoveredCount++;
                }
            }
        }

        if (discoveredCount == 0)
        {
            ContentLogger.Log("No Ogmo level files (.json) were found in the configured LevelPaths.");
        }
    }

    private static OgmoLevel LoadLevel(string levelFilePath)
    {
        try
        {
            string json = File.ReadAllText(levelFilePath);
            return JsonSerializer.Deserialize<OgmoLevel>(json, OgmoJsonSerializerContext.Default.OgmoLevel);
        }
        catch (JsonException ex)
        {
            throw new InvalidContentException(
                $"Failed to parse Ogmo level file '{Path.GetFileName(levelFilePath)}'. " +
                $"Inner error: {ex.Message}", ex);
        }
    }

    private static List<TilemapData> ConvertLevels(OgmoProject project, Dictionary<string, OgmoLevel> discoveredLevels)
    {
        List<TilemapData> result = new List<TilemapData>();

        foreach (KeyValuePair<string, OgmoLevel> kvp in discoveredLevels)
        {
            try
            {
                TilemapData data = OgmoTilemapDataConverter.Convert(kvp.Value, project);

                // Override the name with the actual level filename; the converter uses OgmoVersion instead.
                data.Name = kvp.Key;
                result.Add(data);
            }
            catch (TilemapParseException ex)
            {
                throw new InvalidContentException(ex.Message, ex);
            }
        }

        return result;
    }
}
