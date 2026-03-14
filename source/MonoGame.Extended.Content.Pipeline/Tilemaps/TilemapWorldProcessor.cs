using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps;

/// <summary>
/// Format-agnostic processor that takes all levels from an imported tilemap project and produces
/// a <see cref="TilemapWorldContentItem"/> containing the full world for binary serialization.
/// </summary>
/// <remarks>
/// Accepts <see cref="TilemapProjectContentItem"/> produced by <see cref="TilemapWorldImporter"/>
/// (which handles .ldtk, .world, and .tilemapworld source files).
/// </remarks>
[ContentProcessor(DisplayName = "Tilemap World Processor - MonoGame.Extended")]
public sealed class TilemapWorldProcessor : ContentProcessor<TilemapProjectContentItem, TilemapWorldContentItem>
{
    /// <inheritdoc/>
    public override TilemapWorldContentItem Process(TilemapProjectContentItem input, ContentProcessorContext context)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(context);

        ContentLogger.Logger = context.Logger;

        List<TilemapData> levels = input.Data;

        if (levels == null || levels.Count == 0)
        {
            throw new InvalidContentException("The imported project contains no levels.");
        }

        ContentLogger.Log($"Processing tilemap world with {levels.Count} level(s)");

        TilemapWorldContentItem output = new TilemapWorldContentItem(levels);

        foreach (TilemapData level in levels)
        {
            TilemapExternalRefHelper.Register(level, output, context);
        }

        ContentLogger.Log("Tilemap world processing complete");
        return output;
    }
}
