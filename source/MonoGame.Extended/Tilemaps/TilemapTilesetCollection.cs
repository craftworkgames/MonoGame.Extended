using System;
using System.Collections;
using System.Collections.Generic;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// A collection of tilesets used by a tilemap.
/// </summary>
public class TilemapTilesetCollection : IReadOnlyList<TilemapTileset>
{
    private readonly List<TilemapTileset> _tilesets = new List<TilemapTileset>();

    /// <summary>
    /// Gets the tileset at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the tileset to get.</param>
    /// <returns>The tileset at the specified index.</returns>
    public TilemapTileset this[int index]
    {
        get
        {
            return _tilesets[index];
        }
    }

    /// <summary>
    /// Gets the number of tilesets in the collection.
    /// </summary>
    public int Count
    {
        get
        {
            return _tilesets.Count;
        }
    }

    /// <summary>
    /// Adds a tileset to the collection.
    /// </summary>
    /// <param name="tileset">The tileset to add.</param>
    public void Add(TilemapTileset tileset)
    {
        ArgumentNullException.ThrowIfNull(tileset);

        _tilesets.Add(tileset);
    }

    /// <summary>
    /// Removes a tileset from the collection.
    /// </summary>
    /// <param name="tileset">The tileset to remove.</param>
    /// <returns><see langword="true"/> if the tileset was removed; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// This is an O(n) operation.
    /// </remarks>
    public bool Remove(TilemapTileset tileset)
    {
        return _tilesets.Remove(tileset);
    }

    /// <summary>
    /// Removes all tilesets from the collection.
    /// </summary>
    public void Clear()
    {
        _tilesets.Clear();
    }

    /// <summary>
    /// Gets the tileset that contains the specified global tile ID.
    /// </summary>
    /// <param name="globalTileId">The global tile ID.</param>
    /// <returns>
    /// The tileset containing the tile, or <see langword="null"/> if the ID is 0
    /// or no tileset contains it.
    /// </returns>
    public TilemapTileset GetTilesetForGid(int globalTileId)
    {
        if (globalTileId == 0)
        {
            return null;
        }

        for (int i = 0; i < _tilesets.Count; i++)
        {
            if (_tilesets[i].ContainsGlobalId(globalTileId))
            {
                return _tilesets[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the local tile ID within a tileset from a global tile ID.
    /// </summary>
    /// <param name="globalTileId">The global tile ID.</param>
    /// <param name="tileset">
    /// When this method returns, contains the tileset that owns the tile, or
    /// <see langword="null"/> when <paramref name="globalTileId"/> is 0.
    /// </param>
    /// <returns>The local tile ID within the tileset.</returns>
    /// <exception cref="InvalidOperationException">No tileset contains the specified global tile ID.</exception>
    public int GetLocalId(int globalTileId, out TilemapTileset tileset)
    {
        if (globalTileId == 0)
        {
            tileset = null;
            return 0;
        }

        TilemapTileset foundTileset = GetTilesetForGid(globalTileId);

        if (foundTileset == null)
        {
            throw new InvalidOperationException($"No tileset found for global tile ID {globalTileId}.");
        }

        tileset = foundTileset;
        return globalTileId - foundTileset.FirstGlobalId;
    }

    public IEnumerator<TilemapTileset> GetEnumerator()
    {
        return _tilesets.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
