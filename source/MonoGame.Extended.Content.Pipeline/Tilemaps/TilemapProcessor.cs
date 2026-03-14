using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps;

/// <summary>
/// Format-agnostic processor that extracts a single level from an imported tilemap project
/// and produces a <see cref="TilemapContentItem"/> ready for binary serialization.
/// </summary>
/// <remarks>
/// Accepts <see cref="TilemapProjectContentItem"/> produced by any format-specific importer
/// (LDtk, Tiled, Ogmo). Use the <see cref="LevelName"/> property to select a specific level;
/// leave it empty to use the first level in the project.
/// </remarks>
[ContentProcessor(DisplayName = "Tilemap Processor - MonoGame.Extended")]
public sealed class TilemapProcessor : ContentProcessor<TilemapProjectContentItem, TilemapContentItem>
{
    /// <summary>
    /// Gets or sets the name of the level to extract from the project. If empty, the first
    /// level is used. For LDtk this is the level identifier; for Ogmo it is the filename
    /// without extension.
    /// </summary>
    [DisplayName("Level Name")]
    public string LevelName { get; set; } = string.Empty;

    /// <inheritdoc/>
    public override TilemapContentItem Process(TilemapProjectContentItem input, ContentProcessorContext context)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(context);

        ContentLogger.Logger = context.Logger;

        List<TilemapData> levels = input.Data;
        TilemapData level = FindLevel(levels, LevelName);

        ContentLogger.Log($"Processing tilemap level '{level.Name}'");

        TilemapContentItem output = new TilemapContentItem(level);
        TilemapExternalRefHelper.Register(level, output, context);

        ContentLogger.Log($"Tilemap level '{level.Name}' processing complete");
        return output;
    }

    private static TilemapData FindLevel(List<TilemapData> levels, string levelName)
    {
        if (levels == null || levels.Count == 0)
        {
            throw new InvalidContentException("The imported project contains no levels.");
        }

        if (string.IsNullOrEmpty(levelName))
        {
            return levels[0];
        }

        foreach (TilemapData level in levels)
        {
            if (level.Name == levelName)
            {
                return level;
            }
        }

        throw new InvalidContentException(
            $"Level '{levelName}' was not found in the imported project. " +
            "Check the 'Level Name' processor property and the level identifiers in your project file.");
    }
}
