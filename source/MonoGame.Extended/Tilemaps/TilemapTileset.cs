using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a collection of tiles from a single texture atlas.
/// </summary>
public class TilemapTileset
{
    private readonly Dictionary<int, TilemapTileData> _tileData = new Dictionary<int, TilemapTileData>();
    private readonly List<TilemapTileData> _animatedTileData = new List<TilemapTileData>();
    private int _maxLocalId;

    /// <summary>
    /// Gets the name of the tileset.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the width of each tile in pixels.
    /// </summary>
    public int TileWidth { get; }

    /// <summary>
    /// Gets the height of each tile in pixels.
    /// </summary>
    public int TileHeight { get; }

    /// <summary>
    /// Gets the total number of tiles in the tileset.
    /// </summary>
    public int TileCount { get; }

    /// <summary>
    /// Gets the number of columns in the tileset.
    /// </summary>
    public int Columns { get; }

    /// <summary>
    /// Gets the spacing between tiles in pixels.
    /// </summary>
    public int Spacing { get; }

    /// <summary>
    /// Gets the margin around the tileset in pixels.
    /// </summary>
    public int Margin { get; }

    /// <summary>
    /// Gets the first global tile ID for tiles in this tileset.
    /// </summary>
    /// <remarks>
    /// This value determines the range of global IDs this tileset covers.
    /// Tiles in this tileset have IDs from FirstGlobalId to FirstGlobalId + TileCount - 1.
    /// </remarks>
    public int FirstGlobalId { get; set; }

    /// <summary>
    /// Gets the texture atlas.
    /// </summary>
    public Texture2D Texture { get; }

    /// <summary>
    /// Gets or sets the offset applied when rendering tiles from this tileset.
    /// </summary>
    public Vector2 TileOffset { get; set; }

    /// <summary>
    /// Gets the custom properties of the tileset.
    /// </summary>
    public TilemapProperties Properties { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapTileset"/> class.
    /// </summary>
    /// <param name="name">The name of the tileset.</param>
    /// <param name="texture">The texture atlas.</param>
    /// <param name="tileWidth">The width of each tile in pixels.</param>
    /// <param name="tileHeight">The height of each tile in pixels.</param>
    /// <param name="tileCount">The total number of tiles.</param>
    /// <param name="columns">The number of columns in the tileset.</param>
    /// <param name="spacing">The spacing between tiles in pixels.</param>
    /// <param name="margin">The margin around the tileset in pixels.</param>
    public TilemapTileset(string name, Texture2D texture, int tileWidth, int tileHeight, int tileCount, int columns, int spacing = 0, int margin = 0)
    {
        Name = name;
        Texture = texture;
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        TileCount = tileCount;
        Columns = columns;
        Spacing = spacing;
        Margin = margin;
        Properties = new TilemapProperties();

        // For atlas tilesets tile IDs are sequential 0..tileCount-1, so the max is known upfront.
        // For collection tilesets tile IDs are arbitrary; AddTileData updates _maxLocalId as tiles are registered.
        _maxLocalId = columns > 0 ? tileCount - 1 : -1;
    }

    /// <summary>
    /// Adds tile data for a specified local tile ID.
    /// </summary>
    /// <param name="tileData">The tile data to add.</param>
    public void AddTileData(TilemapTileData tileData)
    {
        _tileData[tileData.LocalId] = tileData;
        if (tileData.Animation != null && tileData.Animation.Frames.Length > 0)
        {
            _animatedTileData.Add(tileData);
        }

        if (tileData.LocalId > _maxLocalId)
        {
            _maxLocalId = tileData.LocalId;
        }
    }

    /// <summary>
    /// Gets the tile data for a specific local tile ID.
    /// </summary>
    /// <param name="localId">The local tile ID within this tileset.</param>
    /// <returns>The tile data, or <see langword="null"/> if not found.</returns>
    public TilemapTileData GetTileData(int localId)
    {
        if (_tileData.TryGetValue(localId, out TilemapTileData data))
        {
            return data;
        }

        return null;
    }

    /// <summary>
    /// Determines whether this tileset contains the specified global tile ID.
    /// </summary>
    /// <param name="globalTileId">The global tile ID to check.</param>
    /// <returns>
    /// <see langword="true"/> if the ID is within this tileset's range; otherwise, <see langword="false"/>.
    /// </returns>
    public bool ContainsGlobalId(int globalTileId)
    {
        int localId = globalTileId - FirstGlobalId;
        return localId >= 0 && localId <= _maxLocalId;
    }

    /// <summary>
    /// Returns all tile data entries that have an animation defined.
    /// </summary>
    /// <returns>A cached list of animated tile data entries. Empty if no animated tiles exist.</returns>
    public IReadOnlyList<TilemapTileData> GetAnimatedTiles() => _animatedTileData;

    /// <summary>
    /// Resolves the texture and source rectangle to use when rendering a tile.
    /// </summary>
    /// <param name="localId">The local tile ID within this tileset.</param>
    /// <param name="texture">The texture to draw from.</param>
    /// <param name="sourceRect">The source rectangle within the texture.</param>
    /// <remarks>
    /// For image collection tilesets each tile has its own image; the resolved texture will
    /// differ per tile and the source rectangle will cover the full image. For atlas-based
    /// tilesets the shared atlas texture and a sub-rectangle are returned. Current animation
    /// frame advancement is also applied automatically.
    /// </remarks>
    public void GetRenderSource(int localId, out Texture2D texture, out Rectangle sourceRect)
    {
        TilemapTileData data = GetTileData(localId);
        int renderLocalId = localId;

        if (data?.Animation != null)
        {
            renderLocalId = data.Animation.CurrentFrame.TileId;
            data = GetTileData(renderLocalId);
        }

        if (data?.CustomImage != null)
        {
            texture = data.CustomImage;
            sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
            return;
        }

        texture = Texture;
        sourceRect = GetTileRegion(renderLocalId);
    }

    /// <summary>
    /// Gets the source rectangle for a tile within the texture atlas.
    /// </summary>
    /// <param name="localId">The local tile ID within this tileset.</param>
    /// <returns>The source rectangle for the tile.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this tileset is an image collection tileset. Use <see cref="GetRenderSource"/> instead.
    /// </exception>
    /// <remarks>
    /// This method accounts for spacing and margin when calculating the tile position.
    /// </remarks>
    public Rectangle GetTileRegion(int localId)
    {
        if (Columns == 0)
        {
            throw new InvalidOperationException("GetTileRegion is not valid for image collection tilesets. Use GetRenderSource instead.");
        }

        int column = localId % Columns;
        int row = localId / Columns;

        int x = Margin + column * (TileWidth + Spacing);
        int y = Margin + row * (TileHeight + Spacing);

        return new Rectangle(x, y, TileWidth, TileHeight);
    }
}
