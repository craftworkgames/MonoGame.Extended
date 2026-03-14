namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a tile instance at a specific position within a tile layer.
/// </summary>
public readonly record struct TilemapTileEntry(int X, int Y, TilemapTile Tile);
