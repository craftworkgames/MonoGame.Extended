using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Content;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps;

/// <summary>
/// Writes a processed tilemap to binary content. Accepts the format-agnostic
/// <see cref="TilemapContentItem"/> produced by <see cref="TilemapProcessor"/>.
/// </summary>
[ContentTypeWriter]
public sealed class TilemapWriter : ContentTypeWriter<TilemapContentItem>
{
    /// <inheritdoc/>
    protected override void Write(ContentWriter writer, TilemapContentItem contentItem)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(contentItem);

        TilemapWriteHelper.WriteMap(writer, contentItem.Data, contentItem);
    }

    /// <inheritdoc/>
    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return typeof(Tilemap).AssemblyQualifiedName;
    }

    /// <inheritdoc/>
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return TilemapReader.NativeAotRegistrationKey;
    }
}
