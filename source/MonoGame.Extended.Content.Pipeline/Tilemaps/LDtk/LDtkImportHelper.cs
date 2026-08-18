using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.LDtk;
using MonoGame.Extended.Tilemaps.LDtk.Converters;
using MonoGame.Extended.Tilemaps.LDtk.Document;
using MonoGame.Extended.Tilemaps.Parsers;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps.LDtk;

/// <summary>
/// Shared LDtk import logic used by both the single-map and world importers.
/// </summary>
internal static class LDtkImportHelper
{
    /// <summary>
    /// Loads an LDtk project, registers all asset dependencies, and converts all levels to
    /// format-agnostic <see cref="TilemapData"/>.
    /// </summary>
    internal static List<TilemapData> Import(string filePath, ContentImporterContext context)
    {
        LDtkProject project = DeserializeProject(filePath);
        string sourceDirectory = Path.GetDirectoryName(filePath)!;

        RegisterTilesetDependencies(project, sourceDirectory, context);

        Dictionary<string, LDtkLevel> resolvedExternalLevels = new Dictionary<string, LDtkLevel>();

        if (project.ExternalLevels)
        {
            LoadExternalLevels(project, sourceDirectory, context, resolvedExternalLevels);
        }

        return ConvertLevels(project, resolvedExternalLevels);
    }

    internal static LDtkProject DeserializeProject(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            LDtkProject project = JsonSerializer.Deserialize(json, LDtkJsonSerializerContext.Default.LDtkProject);

            if (project == null)
            {
                throw new InvalidContentException(
                    $"Failed to parse LDtk project '{Path.GetFileName(filePath)}': deserialization returned null.");
            }

            return project;
        }
        catch (JsonException ex)
        {
            throw new InvalidContentException(
                $"Failed to parse LDtk project '{Path.GetFileName(filePath)}'. " +
                "Ensure the file is a valid LDtk .ldtk JSON file. " +
                $"Inner error: {ex.Message}", ex);
        }
    }

    internal static void RegisterTilesetDependencies(LDtkProject project, string sourceDirectory, ContentImporterContext context)
    {
        if (project.Defs?.Tilesets == null)
        {
            return;
        }

        foreach (LDtkTilesetDefinition tilesetDef in project.Defs.Tilesets)
        {
            if (string.IsNullOrWhiteSpace(tilesetDef.RelPath))
            {
                continue;
            }

            string absoluteImagePath = Path.GetFullPath(
                Path.Combine(sourceDirectory, tilesetDef.RelPath));
            ContentLogger.Log($"Adding tileset dependency '{absoluteImagePath}'");
            context.AddDependency(absoluteImagePath);
            tilesetDef.RelPath = absoluteImagePath;
        }
    }

    internal static void LoadExternalLevels(LDtkProject project, string sourceDirectory, ContentImporterContext context, Dictionary<string, LDtkLevel> resolvedLevels)
    {
        foreach (LDtkLevel level in project.Levels)
        {
            if (string.IsNullOrWhiteSpace(level.ExternalRelPath))
            {
                continue;
            }

            string absoluteLevelPath = Path.GetFullPath(
                Path.Combine(sourceDirectory, level.ExternalRelPath));

            if (!File.Exists(absoluteLevelPath))
            {
                throw new InvalidContentException(
                    $"LDtk project references external level '{level.ExternalRelPath}', " +
                    $"but the file was not found at '{absoluteLevelPath}'.");
            }

            ContentLogger.Log($"Adding external level dependency '{absoluteLevelPath}'");
            context.AddDependency(absoluteLevelPath);

            string levelJson = File.ReadAllText(absoluteLevelPath);
            LDtkLevel resolved = JsonSerializer.Deserialize(levelJson, LDtkJsonSerializerContext.Default.LDtkLevel);

            if (resolved != null)
            {
                resolvedLevels[absoluteLevelPath] = resolved;
            }

            // Update to absolute path so ConvertLevels can match by the same key.
            level.ExternalRelPath = absoluteLevelPath;
        }
    }

    internal static List<TilemapData> ConvertLevels(LDtkProject project, Dictionary<string, LDtkLevel> resolvedExternalLevels)
    {
        List<TilemapData> result = new List<TilemapData>();

        if (project.Levels == null || project.Levels.Count == 0)
        {
            return result;
        }

        foreach (LDtkLevel level in project.Levels)
        {
            LDtkLevel effectiveLevel = level;

            if (!string.IsNullOrEmpty(level.ExternalRelPath) &&
                resolvedExternalLevels.TryGetValue(level.ExternalRelPath, out LDtkLevel resolved))
            {
                effectiveLevel = resolved;
            }

            try
            {
                TilemapData data = LDtkTilemapDataConverter.Convert(effectiveLevel, project);
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
