using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Content;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps.Tiled;

/// <summary>
/// Writes a processed Tiled tileset to binary content.
/// </summary>
[ContentTypeWriter]
public sealed class TilemapTilesetWriter : ContentTypeWriter<TilemapTilesetContentItem>
{
    /// <inheritdoc/>
    protected override void Write(ContentWriter writer, TilemapTilesetContentItem contentItem)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(contentItem);

        WriteTileset(writer, contentItem.Data, contentItem);
    }

    /// <summary>
    /// Writes tileset data from a <see cref="TilemapTilesetData"/> DTO. Called from both this
    /// writer (for standalone .tsx assets) and <see cref="TilemapWriter"/> for inline tilesets.
    /// </summary>
    internal static void WriteTileset(ContentWriter writer, TilemapTilesetData tileset, IExternalReferenceRepository references)
    {
        writer.Write(tileset.Name ?? string.Empty);

        ExternalReference<Texture2DContent> textureRef =
            references.GetExternalReference<Texture2DContent>(tileset.TexturePath ?? string.Empty);
        writer.WriteExternalReference(textureRef);

        writer.Write(tileset.TileWidth);
        writer.Write(tileset.TileHeight);
        writer.Write(tileset.TileCount);
        writer.Write(tileset.Columns);
        writer.Write(tileset.Spacing);
        writer.Write(tileset.Margin);
        writer.Write(tileset.DrawOffsetX);
        writer.Write(tileset.DrawOffsetY);

        TilemapWriteHelper.WriteProperties(writer, tileset.Properties);
        TilemapWriteHelper.WriteTileEntries(writer, tileset.Tiles, references);
    }

    /// <inheritdoc/>
    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return typeof(TilemapTileset).AssemblyQualifiedName;
    }

    /// <inheritdoc/>
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return TilemapTilesetReader.NativeAotRegistrationKey;
    }
}
