using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Content.Pipeline.Tilemaps.Tiled;

/// <summary>
/// Pipeline content item wrapping a converted Tiled tileset.
/// </summary>
public sealed class TilemapTilesetContentItem : ContentItem<TilemapTilesetData>
{
    /// <summary>
    /// Gets the directory containing the source TSX file, used to resolve relative asset paths.
    /// </summary>
    public string SourceDirectory { get; }

    /// <summary>
    /// Gets or sets the first global tile ID assigned by the map that references this tileset.
    /// </summary>
    /// <remarks>
    /// Only meaningful when this tileset is embedded inline in a map rather than loaded standalone.
    /// </remarks>
    public int FirstGlobalId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapTilesetContentItem"/> class.
    /// </summary>
    /// <param name="tilesetData">The converted tileset data.</param>
    /// <param name="sourceDirectory">The directory of the source TSX file.</param>
    public TilemapTilesetContentItem(TilemapTilesetData tilesetData, string sourceDirectory)
        : base(tilesetData)
    {
        SourceDirectory = sourceDirectory;
    }
}
