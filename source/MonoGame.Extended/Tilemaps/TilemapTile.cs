using System;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a tile instance in a tile layer.
/// </summary>
/// <remarks>
/// A tile references a tileset and local tile ID through its global ID,
/// and may include flip flags for horizontal, vertical, or diagonal transformations.
/// </remarks>
public readonly struct TilemapTile
{
    /// <summary>
    /// Gets the global tile ID.
    /// </summary>
    /// <remarks>
    /// The global ID uniquely identifies a tile across all tilesets in the tilemap.
    /// It combines the tileset's first global ID with the local tile ID.
    /// </remarks>
    public int GlobalId { get; }

    /// <summary>
    /// Gets the flip transformation flags applied to this tile.
    /// </summary>
    public TilemapTileFlipFlags FlipFlags { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapTile"/> struct.
    /// </summary>
    /// <param name="globalId">The global tile ID.</param>
    /// <param name="flipFlags">The flip transformation flags.</param>
    public TilemapTile(int globalId, TilemapTileFlipFlags flipFlags = TilemapTileFlipFlags.None)
    {
        GlobalId = globalId;
        FlipFlags = flipFlags;
    }

    /// <summary>
    /// Gets the tileset that contains this tile.
    /// </summary>
    /// <param name="tilesets">The collection of tilesets.</param>
    /// <returns>The tileset containing this tile, or <see langword="null"/> if not found.</returns>
    public TilemapTileset GetTileset(TilemapTilesetCollection tilesets)
    {
        return tilesets.GetTilesetForGid(GlobalId);
    }

    /// <summary>
    /// Gets the local tile ID within the tileset.
    /// </summary>
    /// <param name="tilesets">The collection of tilesets.</param>
    /// <returns>The local tile ID.</returns>
    public int GetLocalId(TilemapTilesetCollection tilesets)
    {
        return tilesets.GetLocalId(GlobalId, out _);
    }

    /// <summary>
    /// Gets the local tile ID within the specified tileset.
    /// </summary>
    /// <param name="tilesets">The collection of tilesets.</param>
    /// <param name="tileset">When this method returns, contains the tileset that owns this tile.</param>
    /// <returns>The local tile ID within the tileset.</returns>
    public int GetLocalId(TilemapTilesetCollection tilesets, out TilemapTileset tileset)
    {
        return tilesets.GetLocalId(GlobalId, out tileset);
    }

    /// <summary>
    /// Gets the tile data for this tile from the tileset that contains it.
    /// </summary>
    /// <param name="tilesets">The collection of tilesets to search.</param>
    /// <returns>The tile data, or <see langword="null"/> if not found or the tile has no data defined.</returns>
    public TilemapTileData? GetTileData(TilemapTilesetCollection tilesets)
    {
        TilemapTileset tileset = GetTileset(tilesets);
        if (tileset == null)
        {
            return null;
        }

        int localId = GlobalId - tileset.FirstGlobalId;
        return tileset.GetTileData(localId);
    }
}
