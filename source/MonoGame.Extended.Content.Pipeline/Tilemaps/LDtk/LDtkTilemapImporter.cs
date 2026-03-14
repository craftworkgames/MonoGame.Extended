using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content.Pipeline;

using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps.LDtk;

/// <summary>
/// Imports an LDtk project from a .ldtk file into the content pipeline.
/// </summary>
/// <remarks>
/// Converts all levels in the project and registers all dependencies (external level files,
/// tileset images) so that the map is automatically rebuilt whenever any referenced asset changes.
/// </remarks>
[ContentImporter(".ldtk", DefaultProcessor = "TilemapProcessor", DisplayName = "LDtk Tilemap Importer - MonoGame.Extended")]
internal sealed class LDtkTilemapImporter : ContentImporter<TilemapProjectContentItem>
{
    /// <inheritdoc/>
    public override TilemapProjectContentItem Import(string filePath, ContentImporterContext context)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(context);

        ContentLogger.Logger = context.Logger;
        ContentLogger.Log($"Importing LDtk project '{filePath}'");

        List<TilemapData> levels = LDtkImportHelper.Import(filePath, context);

        ContentLogger.Log($"Imported {levels.Count} level(s) from LDtk project '{filePath}'");
        return new TilemapProjectContentItem(levels);
    }
}
