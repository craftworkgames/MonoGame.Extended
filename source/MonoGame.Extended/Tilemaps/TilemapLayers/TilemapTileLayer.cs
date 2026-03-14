using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a layer composed of a grid of tiles.
/// </summary>
public class TilemapTileLayer : TilemapLayer
{
    private readonly TilemapTile?[,] _tiles;

    /// <summary>
    /// Gets the width of the layer in tiles.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the height of the layer in tiles.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets the width of each tile in pixels.
    /// </summary>
    public int TileWidth { get; }

    /// <summary>
    /// Gets the height of each tile in pixels.
    /// </summary>
    public int TileHeight { get; }

    /// <inheritdoc/>
    public override Rectangle Bounds
    {
        get
        {
            return new Rectangle(0, 0, Width * TileWidth, Height * TileHeight);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TilemapTileLayer"/> class.
    /// </summary>
    /// <param name="name">The name of the layer.</param>
    /// <param name="width">The width of the layer in tiles.</param>
    /// <param name="height">The height of the layer in tiles.</param>
    /// <param name="tileWidth">The width of each tile in pixels.</param>
    /// <param name="tileHeight">The height of each tile in pixels.</param>
    public TilemapTileLayer(string name, int width, int height, int tileWidth, int tileHeight)
        : base(name)
    {
        Width = width;
        Height = height;
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        _tiles = new TilemapTile?[width, height];
    }

    /// <summary>
    /// Gets the tile at the specified tile coordinates.
    /// </summary>
    /// <param name="x">The X coordinate of the tile.</param>
    /// <param name="y">The Y coordinate of the tile.</param>
    /// <returns>The tile at the specified coordinates, or <see langword="null"/> if empty.</returns>
    public TilemapTile? GetTile(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return null;
        }

        return _tiles[x, y];
    }

    /// <summary>
    /// Sets the tile at the specified tile coordinates.
    /// </summary>
    /// <param name="x">The X coordinate of the tile.</param>
    /// <param name="y">The Y coordinate of the tile.</param>
    /// <param name="tile">The tile to set, or <see langword="null"/> to clear.</param>
    public void SetTile(int x, int y, TilemapTile? tile)
    {
        if (x < 0 || x >= Width)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }

        if (y < 0 || y >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        _tiles[x, y] = tile;
    }

    /// <summary>
    /// Gets all non-empty tiles in the layer.
    /// </summary>
    /// <returns>An enumerable of tuples containing tile coordinates and tile data.</returns>
    public IEnumerable<TilemapTileEntry> GetTiles()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                TilemapTile? tile = _tiles[x, y];

                if (tile.HasValue)
                {
                    yield return new TilemapTileEntry(x, y, tile.Value);
                }
            }
        }
    }

    /// <summary>
    /// Gets all tiles within the specified rectangular region.
    /// </summary>
    /// <param name="region">The region in tile coordinates.</param>
    /// <returns>An enumerable of tuples containing tile coordinates and tile data.</returns>
    public IEnumerable<TilemapTileEntry> GetTilesInRegion(Rectangle region)
    {
        int startX = Math.Max(0, region.X);
        int startY = Math.Max(0, region.Y);
        int endX = Math.Min(Width, region.X + region.Width);
        int endY = Math.Min(Height, region.Y + region.Height);

        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                TilemapTile? tile = _tiles[x, y];

                if (tile.HasValue)
                {
                    yield return new TilemapTileEntry(x, y, tile.Value);
                }
            }
        }
    }
}
