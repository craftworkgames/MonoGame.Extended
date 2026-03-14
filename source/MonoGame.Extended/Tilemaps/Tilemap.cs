using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Tilemaps;

/// <summary>
/// Represents a tilemap loaded from any supported format.
/// </summary>
public class Tilemap
{
    /// <summary>
    /// Gets or sets the name of the tilemap.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets the width of the tilemap in tiles.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the height of the tilemap in tiles.
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

    /// <summary>
    /// Gets the orientation of the tilemap.
    /// </summary>
    public TilemapOrientation Orientation { get; }

    /// <summary>
    /// Gets the side length of hex tiles in pixels. Only meaningful for hexagonal maps.
    /// </summary>
    public int HexSideLength { get; internal set; }

    /// <summary>
    /// Gets the stagger axis. Only meaningful for staggered and hexagonal maps.
    /// </summary>
    public TilemapStaggerAxis StaggerAxis { get; internal set; }

    /// <summary>
    /// Gets the stagger index. Only meaningful for staggered and hexagonal maps.
    /// </summary>
    public TilemapStaggerIndex StaggerIndex { get; internal set; }

    /// <summary>
    /// Gets or sets the background color of the tilemap.
    /// </summary>
    public Color? BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the parallax origin in world pixels.
    /// </summary>
    public Vector2 ParallaxOrigin { get; set; }

    /// <summary>
    /// Gets or sets the position of this map in world space.
    /// </summary>
    public Vector2 WorldPosition { get; set; }

    /// <summary>
    /// Gets or sets the depth layer of this map.
    /// Positive values represent layers above the surface; negative values represent
    /// layers below the surface.
    /// </summary>
    public int WorldDepth { get; set; }

    /// <summary>
    /// Gets the custom properties of the tilemap.
    /// </summary>
    public TilemapProperties Properties { get; }

    /// <summary>
    /// Gets the collection of tilesets used by this tilemap.
    /// </summary>
    public TilemapTilesetCollection Tilesets { get; }

    /// <summary>
    /// Gets the collection of layers in this tilemap.
    /// </summary>
    public TilemapLayerCollection Layers { get; }

    /// <summary>
    /// Gets the world-space bounds of the tilemap.
    /// </summary>
    public Rectangle WorldBounds
    {
        get
        {
            return Orientation switch
            {
                TilemapOrientation.Orthogonal => new Rectangle(0, 0, Width * TileWidth, Height * TileHeight),
                TilemapOrientation.Isometric => CalculateIsometricBounds(),
                TilemapOrientation.Staggered => new Rectangle(0, 0, Width * TileWidth, Height * TileHeight),
                TilemapOrientation.Hexagonal => new Rectangle(0, 0, Width * TileWidth, Height * TileHeight),
                _ => new Rectangle(0, 0, Width * TileWidth, Height * TileHeight)
            };
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tilemap"/> class.
    /// </summary>
    /// <param name="name">The name of the tilemap.</param>
    /// <param name="width">The width of the tilemap in tiles.</param>
    /// <param name="height">The height of the tilemap in tiles.</param>
    /// <param name="tileWidth">The width of each tile in pixels.</param>
    /// <param name="tileHeight">The height of each tile in pixels.</param>
    /// <param name="orientation">The orientation of the tilemap.</param>
    public Tilemap(string name, int width, int height, int tileWidth, int tileHeight, TilemapOrientation orientation)
    {
        Name = name;
        Width = width;
        Height = height;
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        Orientation = orientation;
        Properties = new TilemapProperties();
        Tilesets = new TilemapTilesetCollection();
        Layers = new TilemapLayerCollection();
    }

    /// <summary>
    /// Converts tile coordinates to world-space position.
    /// </summary>
    /// <param name="x">The tile X coordinate.</param>
    /// <param name="y">The tile Y coordinate.</param>
    /// <returns>The position in world space, where (0,0) represents the top-left corner of the map.</returns>
    public Point TileToWorldPosition(int x, int y)
    {
        return Orientation switch
        {
            TilemapOrientation.Orthogonal => TileToWorldOrthogonal(x, y),
            TilemapOrientation.Isometric => TileToWorldIsometric(x, y),
            TilemapOrientation.Staggered => TileToWorldStaggered(x, y),
            TilemapOrientation.Hexagonal => TileToWorldHexagonal(x, y),
            _ => TileToWorldOrthogonal(x, y)
        };
    }

    /// <summary>
    /// Converts world-space position to tile coordinates.
    /// </summary>
    /// <param name="worldPosition">The position in world space.</param>
    /// <returns>The tile coordinates.</returns>
    public Point WorldToTilePosition(Vector2 worldPosition)
    {
        return Orientation switch
        {
            TilemapOrientation.Orthogonal => WorldToTileOrthogonal(worldPosition),
            TilemapOrientation.Isometric => WorldToTileIsometric(worldPosition),
            TilemapOrientation.Staggered => WorldToTileStaggered(worldPosition),
            TilemapOrientation.Hexagonal => WorldToTileHexagonal(worldPosition),
            _ => WorldToTileOrthogonal(worldPosition),
        };
    }

    #region Helper Methods

    private Rectangle CalculateIsometricBounds()
    {
        int worldWidth = (Width + Height) * (TileWidth / 2);
        int worldHeight = (Width + Height) * (TileHeight / 2);

        return new Rectangle(0, 0, worldWidth, worldHeight);
    }

    private Point TileToWorldOrthogonal(int x, int y)
    {
        return new Point(x * TileWidth, y * TileHeight);
    }

    private Point TileToWorldIsometric(int x, int y)
    {
        int worldX = (x - y) * (TileWidth / 2);
        int worldY = (x + y) * (TileHeight / 2);

        return new Point(worldX, worldY);
    }

    private Point TileToWorldStaggered(int x, int y)
    {
        if (StaggerAxis == TilemapStaggerAxis.Y)
        {
            bool isStaggered = StaggerIndex == TilemapStaggerIndex.Odd ? (y % 2 != 0) : (y % 2 == 0);
            int worldX = x * TileWidth + (isStaggered ? TileWidth / 2 : 0);
            int worldY = y * (TileHeight / 2);
            return new Point(worldX, worldY);
        }
        else
        {
            bool isStaggered = StaggerIndex == TilemapStaggerIndex.Odd ? (x % 2 != 0) : (x % 2 == 0);
            int worldX = x * (TileWidth / 2);
            int worldY = y * TileHeight + (isStaggered ? TileHeight / 2 : 0);
            return new Point(worldX, worldY);
        }
    }

    private Point TileToWorldHexagonal(int x, int y)
    {
        if (StaggerAxis == TilemapStaggerAxis.Y)
        {
            // Rows are staggered. Vertical step = (tileHeight + hexSideLength) / 2.
            bool isStaggered = StaggerIndex == TilemapStaggerIndex.Odd ? (y % 2 != 0) : (y % 2 == 0);
            int worldX = x * TileWidth + (isStaggered ? TileWidth / 2 : 0);
            int worldY = y * (TileHeight + HexSideLength) / 2;
            return new Point(worldX, worldY);
        }
        else
        {
            // Columns are staggered. Horizontal step = (tileWidth + hexSideLength) / 2.
            bool isStaggered = StaggerIndex == TilemapStaggerIndex.Odd ? (x % 2 != 0) : (x % 2 == 0);
            int worldX = x * (TileWidth + HexSideLength) / 2;
            int worldY = y * TileHeight + (isStaggered ? TileHeight / 2 : 0);
            return new Point(worldX, worldY);
        }
    }

    private Point WorldToTileOrthogonal(Vector2 worldPosition)
    {
        int tileX = (int)(worldPosition.X / TileWidth);
        int tileY = (int)(worldPosition.Y / TileHeight);

        return new Point(tileX, tileY);
    }

    private Point WorldToTileIsometric(Vector2 worldPosition)
    {
        float halfTileWidth = TileWidth * 0.5f;
        float halfTileHeight = TileHeight * 0.5f;

        float normalizedX = worldPosition.X / halfTileWidth;
        float normalizedY = worldPosition.Y / halfTileHeight;

        int tileX = (int)((normalizedX + normalizedY) * 0.5f);
        int tileY = (int)((normalizedY - normalizedX) * 0.5f);

        return new Point(tileX, tileY);
    }

    private Point WorldToTileStaggered(Vector2 worldPosition)
    {
        if (StaggerAxis == TilemapStaggerAxis.Y)
        {
            int rowStep = TileHeight / 2;
            int row = (int)(worldPosition.Y / rowStep);
            bool isStaggered = StaggerIndex == TilemapStaggerIndex.Odd ? (row % 2 != 0) : (row % 2 == 0);
            int col = (int)((worldPosition.X - (isStaggered ? TileWidth / 2f : 0f)) / TileWidth);
            return new Point(col, row);
        }
        else
        {
            int colStep = TileWidth / 2;
            int col = (int)(worldPosition.X / colStep);
            bool isStaggered = StaggerIndex == TilemapStaggerIndex.Odd ? (col % 2 != 0) : (col % 2 == 0);
            int row = (int)((worldPosition.Y - (isStaggered ? TileHeight / 2f : 0f)) / TileHeight);
            return new Point(col, row);
        }
    }

    private Point WorldToTileHexagonal(Vector2 worldPosition)
    {
        if (StaggerAxis == TilemapStaggerAxis.Y)
        {
            int rowStep = (TileHeight + HexSideLength) / 2;
            int row = (int)(worldPosition.Y / rowStep);
            bool isStaggered = StaggerIndex == TilemapStaggerIndex.Odd ? (row % 2 != 0) : (row % 2 == 0);
            int col = (int)((worldPosition.X - (isStaggered ? TileWidth / 2f : 0f)) / TileWidth);
            return new Point(col, row);
        }
        else
        {
            int colStep = (TileWidth + HexSideLength) / 2;
            int col = (int)(worldPosition.X / colStep);
            bool isStaggered = StaggerIndex == TilemapStaggerIndex.Odd ? (col % 2 != 0) : (col % 2 == 0);
            int row = (int)((worldPosition.Y - (isStaggered ? TileHeight / 2f : 0f)) / TileHeight);
            return new Point(col, row);
        }
    }

    #endregion
}
