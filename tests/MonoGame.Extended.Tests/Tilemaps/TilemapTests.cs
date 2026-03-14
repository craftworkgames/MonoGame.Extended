using Microsoft.Xna.Framework;
using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Tests;

public sealed class TilemapTests
{
    #region Orthogonal Coordinate Tests

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(1, 0, 32, 0)]
    [InlineData(0, 1, 0, 32)]
    [InlineData(1, 1, 32, 32)]
    [InlineData(5, 3, 160, 96)]
    [InlineData(10, 10, 320, 320)]
    public void TileToWorldPosition_Orthogonal_VariousCoordinates_ReturnsExpectedPositions(int tileX, int tileY, int expectedWorldX, int expectedWorldY)
    {
        Tilemap tilemap = new Tilemap("test", 20, 20, 32, 32, TilemapOrientation.Orthogonal);
        Point worldPos = tilemap.TileToWorldPosition(tileX, tileY);
        Assert.Equal(new Point(expectedWorldX, expectedWorldY), worldPos);
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(32, 0, 1, 0)]
    [InlineData(0, 32, 0, 1)]
    [InlineData(32, 32, 1, 1)]
    [InlineData(160, 96, 5, 3)]
    [InlineData(320, 320, 10, 10)]
    public void WorldToTilePosition_Orthogonal_VariousPositions_ReturnsExpectedCoordinates(float worldX, float worldY, int expectedTileX, int expectedTileY)
    {
        Tilemap tilemap = new Tilemap("test", 20, 20, 32, 32, TilemapOrientation.Orthogonal);
        Point tilePos = tilemap.WorldToTilePosition(new Vector2(worldX, worldY));
        Assert.Equal(new Point(expectedTileX, expectedTileY), tilePos);
    }

    [Fact]
    public void CoordinateTransformation_Orthogonal_RoundTrip_PreservesCoordinates()
    {
        Tilemap tilemap = new Tilemap("test", 20, 20, 32, 32, TilemapOrientation.Orthogonal);
        Point originalTile = new Point(5, 7);

        Point worldPos = tilemap.TileToWorldPosition(originalTile.X, originalTile.Y);
        Point backToTile = tilemap.WorldToTilePosition(new Vector2(worldPos.X, worldPos.Y));

        Assert.Equal(originalTile, backToTile);
    }

    [Fact]
    public void WorldBounds_Orthogonal_ReturnsCorrectBounds()
    {
        Tilemap tilemap = new Tilemap("test", 10, 8, 32, 32, TilemapOrientation.Orthogonal);
        Rectangle bounds = tilemap.WorldBounds;
        Assert.Equal(new Rectangle(0, 0, 320, 256), bounds);
    }

    #endregion

    #region Isometric Coordinate Tests

    [Fact]
    public void TileToWorldPosition_Isometric_Origin_ReturnsZero()
    {
        Tilemap tilemap = new Tilemap("test", 10, 10, 64, 32, TilemapOrientation.Isometric);
        Point worldPos = tilemap.TileToWorldPosition(0, 0);
        Assert.Equal(Point.Zero, worldPos);
    }

    [Theory]
    [InlineData(1, 0, 32, 16)]   // (1-0)*(64/2), (1+0)*(32/2)
    [InlineData(0, 1, -32, 16)]  // (0-1)*(64/2), (0+1)*(32/2)
    [InlineData(1, 1, 0, 32)]    // (1-1)*(64/2), (1+1)*(32/2)
    [InlineData(2, 1, 32, 48)]   // (2-1)*(64/2), (2+1)*(32/2)
    public void TileToWorldPosition_Isometric_VariousCoordinates_ReturnsExpectedPositions(int tileX, int tileY, int expectedWorldX, int expectedWorldY)
    {
        Tilemap tilemap = new Tilemap("test", 10, 10, 64, 32, TilemapOrientation.Isometric);
        Point worldPos = tilemap.TileToWorldPosition(tileX, tileY);
        Assert.Equal(new Point(expectedWorldX, expectedWorldY), worldPos);
    }

    [Fact]
    public void WorldToTilePosition_Isometric_Origin_ReturnsZero()
    {
        Tilemap tilemap = new Tilemap("test", 10, 10, 64, 32, TilemapOrientation.Isometric);
        Point tilePos = tilemap.WorldToTilePosition(new Vector2(0, 0));
        Assert.Equal(Point.Zero, tilePos);
    }

    [Fact]
    public void CoordinateTransformation_Isometric_RoundTrip_PreservesCoordinates()
    {
        Tilemap tilemap = new Tilemap("test", 10, 10, 64, 32, TilemapOrientation.Isometric);
        Point originalTile = new Point(5, 3);

        Point worldPos = tilemap.TileToWorldPosition(originalTile.X, originalTile.Y);
        Point backToTile = tilemap.WorldToTilePosition(new Vector2(worldPos.X, worldPos.Y));

        Assert.Equal(originalTile, backToTile);
    }

    [Fact]
    public void WorldBounds_Isometric_ReturnsCorrectBounds()
    {
        Tilemap tilemap = new Tilemap("test", 10, 8, 64, 32, TilemapOrientation.Isometric);
        Rectangle bounds = tilemap.WorldBounds;

        // (10 + 8) * (64/2) = 18 * 32 = 576 width
        // (10 + 8) * (32/2) = 18 * 16 = 288 height
        Assert.Equal(new Rectangle(0, 0, 576, 288), bounds);
    }

    #endregion

    #region Different Tile Sizes

    [Theory]
    [InlineData(16, 16)]
    [InlineData(32, 32)]
    [InlineData(64, 64)]
    [InlineData(48, 24)]
    public void TileToWorldPosition_Orthogonal_DifferentTileSizes_WorksCorrectly(int tileWidth, int tileHeight)
    {
        Tilemap tilemap = new Tilemap("test", 10, 10, tileWidth, tileHeight, TilemapOrientation.Orthogonal);
        Point worldPos = tilemap.TileToWorldPosition(3, 2);
        Assert.Equal(new Point(3 * tileWidth, 2 * tileHeight), worldPos);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void TileToWorldPosition_NegativeCoordinates_HandlesCorrectly()
    {
        Tilemap tilemap = new Tilemap("test", 10, 10, 32, 32, TilemapOrientation.Orthogonal);
        Point worldPos = tilemap.TileToWorldPosition(-1, -2);
        Assert.Equal(new Point(-32, -64), worldPos);
    }

    [Fact]
    public void WorldToTilePosition_NegativeWorldPosition_HandlesCorrectly()
    {
        Tilemap tilemap = new Tilemap("test", 10, 10, 32, 32, TilemapOrientation.Orthogonal);
        Point tilePos = tilemap.WorldToTilePosition(new Vector2(-32, -64));
        Assert.Equal(new Point(-1, -2), tilePos);
    }

    [Fact]
    public void WorldToTilePosition_FractionalWorldPosition_TruncatesDown()
    {
        Tilemap tilemap = new Tilemap("test", 10, 10, 32, 32, TilemapOrientation.Orthogonal);
        Point tilePos = tilemap.WorldToTilePosition(new Vector2(50.7f, 70.9f));

        // 50/32 = 1.5 -> 1
        // 70/32 = 2.1 -> 2
        Assert.Equal(new Point(1, 2), tilePos);
    }

    #endregion

    #region Staggered Coordinate Tests

    // Staggered Y-axis, Odd index: odd rows are offset right by TileWidth/2.
    // worldX = x * TileWidth + (oddRow ? TileWidth/2 : 0)
    // worldY = y * (TileHeight / 2)
    [Theory]
    [InlineData(0, 0,  0,  0)]  // even row, no offset
    [InlineData(2, 0, 32,  0)]  // even row, no offset
    [InlineData(0, 1,  8,  8)]  // odd row, +8 on X
    [InlineData(2, 1, 40,  8)]  // odd row, +8 on X
    [InlineData(0, 2,  0, 16)]  // even row, no offset
    [InlineData(0, 3,  8, 24)]  // odd row, +8 on X
    public void TileToWorldPosition_StaggeredY_Odd_ReturnsExpectedPositions(int tileX, int tileY, int expectedWorldX, int expectedWorldY)
    {
        Tilemap tilemap = new Tilemap("test", 20, 20, 16, 16, TilemapOrientation.Staggered);
        tilemap.StaggerAxis = TilemapStaggerAxis.Y;
        tilemap.StaggerIndex = TilemapStaggerIndex.Odd;
        Assert.Equal(new Point(expectedWorldX, expectedWorldY), tilemap.TileToWorldPosition(tileX, tileY));
    }

    // Staggered Y-axis, Even index: even rows are offset right by TileWidth/2.
    [Theory]
    [InlineData(0, 0,  8,  0)]  // even row, +8 on X
    [InlineData(2, 0, 40,  0)]  // even row, +8 on X
    [InlineData(0, 1,  0,  8)]  // odd row, no offset
    [InlineData(2, 1, 32,  8)]  // odd row, no offset
    [InlineData(0, 2,  8, 16)]  // even row, +8 on X
    [InlineData(0, 3,  0, 24)]  // odd row, no offset
    public void TileToWorldPosition_StaggeredY_Even_ReturnsExpectedPositions(int tileX, int tileY, int expectedWorldX, int expectedWorldY)
    {
        Tilemap tilemap = new Tilemap("test", 20, 20, 16, 16, TilemapOrientation.Staggered);
        tilemap.StaggerAxis = TilemapStaggerAxis.Y;
        tilemap.StaggerIndex = TilemapStaggerIndex.Even;
        Assert.Equal(new Point(expectedWorldX, expectedWorldY), tilemap.TileToWorldPosition(tileX, tileY));
    }

    // Staggered X-axis, Odd index: odd columns are offset down by TileHeight/2.
    // worldX = x * (TileWidth / 2)
    // worldY = y * TileHeight + (oddCol ? TileHeight/2 : 0)
    [Theory]
    [InlineData(0, 0,  0,  0)]  // even col, no offset
    [InlineData(0, 2,  0, 32)]  // even col, no offset
    [InlineData(1, 0,  8,  8)]  // odd col, +8 on Y
    [InlineData(1, 2,  8, 40)]  // odd col, +8 on Y
    [InlineData(2, 0, 16,  0)]  // even col, no offset
    [InlineData(3, 0, 24,  8)]  // odd col, +8 on Y
    public void TileToWorldPosition_StaggeredX_Odd_ReturnsExpectedPositions(int tileX, int tileY, int expectedWorldX, int expectedWorldY)
    {
        Tilemap tilemap = new Tilemap("test", 20, 20, 16, 16, TilemapOrientation.Staggered);
        tilemap.StaggerAxis = TilemapStaggerAxis.X;
        tilemap.StaggerIndex = TilemapStaggerIndex.Odd;
        Assert.Equal(new Point(expectedWorldX, expectedWorldY), tilemap.TileToWorldPosition(tileX, tileY));
    }

    // Staggered X-axis, Even index: even columns are offset down by TileHeight/2.
    [Theory]
    [InlineData(0, 0,  0,  8)]  // even col, +8 on Y
    [InlineData(0, 2,  0, 40)]  // even col, +8 on Y
    [InlineData(1, 0,  8,  0)]  // odd col, no offset
    [InlineData(1, 2,  8, 32)]  // odd col, no offset
    [InlineData(2, 0, 16,  8)]  // even col, +8 on Y
    [InlineData(3, 0, 24,  0)]  // odd col, no offset
    public void TileToWorldPosition_StaggeredX_Even_ReturnsExpectedPositions(int tileX, int tileY, int expectedWorldX, int expectedWorldY)
    {
        Tilemap tilemap = new Tilemap("test", 20, 20, 16, 16, TilemapOrientation.Staggered);
        tilemap.StaggerAxis = TilemapStaggerAxis.X;
        tilemap.StaggerIndex = TilemapStaggerIndex.Even;
        Assert.Equal(new Point(expectedWorldX, expectedWorldY), tilemap.TileToWorldPosition(tileX, tileY));
    }

    [Theory]
    [InlineData(TilemapStaggerAxis.Y, TilemapStaggerIndex.Odd)]
    [InlineData(TilemapStaggerAxis.Y, TilemapStaggerIndex.Even)]
    [InlineData(TilemapStaggerAxis.X, TilemapStaggerIndex.Odd)]
    [InlineData(TilemapStaggerAxis.X, TilemapStaggerIndex.Even)]
    public void CoordinateTransformation_Staggered_RoundTrip_PreservesCoordinates(TilemapStaggerAxis axis, TilemapStaggerIndex index)
    {
        Tilemap tilemap = new Tilemap("test", 20, 20, 16, 16, TilemapOrientation.Staggered);
        tilemap.StaggerAxis = axis;
        tilemap.StaggerIndex = index;
        Point originalTile = new Point(5, 7);

        Point worldPos = tilemap.TileToWorldPosition(originalTile.X, originalTile.Y);
        Point backToTile = tilemap.WorldToTilePosition(new Vector2(worldPos.X, worldPos.Y));

        Assert.Equal(originalTile, backToTile);
    }

    #endregion

    #region Hexagonal Coordinate Tests

    // Hexagonal Y-axis, Odd index: odd rows are offset right by TileWidth/2.
    // Vertical step = (TileHeight + HexSideLength) / 2
    // worldX = x * TileWidth + (oddRow ? TileWidth/2 : 0)
    // worldY = y * (TileHeight + HexSideLength) / 2
    // Using 14x12 tiles with hexSideLength=6: rowStep=(12+6)/2=9, halfX=7
    [Theory]
    [InlineData(0, 0,  0,  0)]  // even row, no X offset, worldY=0
    [InlineData(2, 0, 28,  0)]  // even row, no X offset
    [InlineData(0, 1,  7,  9)]  // odd row, +7 on X, worldY=9
    [InlineData(2, 1, 35,  9)]  // odd row, +7 on X
    [InlineData(0, 2,  0, 18)]  // even row, worldY=18
    [InlineData(0, 3,  7, 27)]  // odd row, +7 on X, worldY=27
    public void TileToWorldPosition_HexagonalY_Odd_ReturnsExpectedPositions(int tileX, int tileY, int expectedWorldX, int expectedWorldY)
    {
        Tilemap tilemap = new Tilemap("test", 20, 20, 14, 12, TilemapOrientation.Hexagonal);
        tilemap.StaggerAxis = TilemapStaggerAxis.Y;
        tilemap.StaggerIndex = TilemapStaggerIndex.Odd;
        tilemap.HexSideLength = 6;
        Assert.Equal(new Point(expectedWorldX, expectedWorldY), tilemap.TileToWorldPosition(tileX, tileY));
    }

    // Hexagonal Y-axis, Even index: even rows are offset right by TileWidth/2.
    [Theory]
    [InlineData(0, 0,  7,  0)]  // even row, +7 on X
    [InlineData(2, 0, 35,  0)]  // even row, +7 on X
    [InlineData(0, 1,  0,  9)]  // odd row, no X offset
    [InlineData(2, 1, 28,  9)]  // odd row, no X offset
    [InlineData(0, 2,  7, 18)]  // even row, +7 on X
    [InlineData(0, 3,  0, 27)]  // odd row, no X offset
    public void TileToWorldPosition_HexagonalY_Even_ReturnsExpectedPositions(int tileX, int tileY, int expectedWorldX, int expectedWorldY)
    {
        Tilemap tilemap = new Tilemap("test", 20, 20, 14, 12, TilemapOrientation.Hexagonal);
        tilemap.StaggerAxis = TilemapStaggerAxis.Y;
        tilemap.StaggerIndex = TilemapStaggerIndex.Even;
        tilemap.HexSideLength = 6;
        Assert.Equal(new Point(expectedWorldX, expectedWorldY), tilemap.TileToWorldPosition(tileX, tileY));
    }

    // Hexagonal X-axis, Odd index: odd columns are offset down by TileHeight/2.
    // Horizontal step = (TileWidth + HexSideLength) / 2
    // worldX = x * (TileWidth + HexSideLength) / 2
    // worldY = y * TileHeight + (oddCol ? TileHeight/2 : 0)
    // Using 12x14 tiles with hexSideLength=6: colStep=(12+6)/2=9, halfY=7
    [Theory]
    [InlineData(0, 0,  0,  0)]  // even col, no Y offset
    [InlineData(0, 2,  0, 28)]  // even col, no Y offset
    [InlineData(1, 0,  9,  7)]  // odd col, +7 on Y
    [InlineData(1, 2,  9, 35)]  // odd col, +7 on Y
    [InlineData(2, 0, 18,  0)]  // even col, no Y offset
    [InlineData(3, 0, 27,  7)]  // odd col, +7 on Y
    public void TileToWorldPosition_HexagonalX_Odd_ReturnsExpectedPositions(int tileX, int tileY, int expectedWorldX, int expectedWorldY)
    {
        Tilemap tilemap = new Tilemap("test", 20, 20, 12, 14, TilemapOrientation.Hexagonal);
        tilemap.StaggerAxis = TilemapStaggerAxis.X;
        tilemap.StaggerIndex = TilemapStaggerIndex.Odd;
        tilemap.HexSideLength = 6;
        Assert.Equal(new Point(expectedWorldX, expectedWorldY), tilemap.TileToWorldPosition(tileX, tileY));
    }

    // Hexagonal X-axis, Even index: even columns are offset down by TileHeight/2.
    [Theory]
    [InlineData(0, 0,  0,  7)]  // even col, +7 on Y
    [InlineData(0, 2,  0, 35)]  // even col, +7 on Y
    [InlineData(1, 0,  9,  0)]  // odd col, no Y offset
    [InlineData(1, 2,  9, 28)]  // odd col, no Y offset
    [InlineData(2, 0, 18,  7)]  // even col, +7 on Y
    [InlineData(3, 0, 27,  0)]  // odd col, no Y offset
    public void TileToWorldPosition_HexagonalX_Even_ReturnsExpectedPositions(int tileX, int tileY, int expectedWorldX, int expectedWorldY)
    {
        Tilemap tilemap = new Tilemap("test", 20, 20, 12, 14, TilemapOrientation.Hexagonal);
        tilemap.StaggerAxis = TilemapStaggerAxis.X;
        tilemap.StaggerIndex = TilemapStaggerIndex.Even;
        tilemap.HexSideLength = 6;
        Assert.Equal(new Point(expectedWorldX, expectedWorldY), tilemap.TileToWorldPosition(tileX, tileY));
    }

    [Theory]
    [InlineData(TilemapStaggerAxis.Y, TilemapStaggerIndex.Odd)]
    [InlineData(TilemapStaggerAxis.Y, TilemapStaggerIndex.Even)]
    [InlineData(TilemapStaggerAxis.X, TilemapStaggerIndex.Odd)]
    [InlineData(TilemapStaggerAxis.X, TilemapStaggerIndex.Even)]
    public void CoordinateTransformation_Hexagonal_RoundTrip_PreservesCoordinates(TilemapStaggerAxis axis, TilemapStaggerIndex index)
    {
        // Use asymmetric tile sizes to verify both axes are handled independently.
        int tileWidth = axis == TilemapStaggerAxis.Y ? 14 : 12;
        int tileHeight = axis == TilemapStaggerAxis.Y ? 12 : 14;

        Tilemap tilemap = new Tilemap("test", 20, 20, tileWidth, tileHeight, TilemapOrientation.Hexagonal);
        tilemap.StaggerAxis = axis;
        tilemap.StaggerIndex = index;
        tilemap.HexSideLength = 6;
        Point originalTile = new Point(5, 7);

        Point worldPos = tilemap.TileToWorldPosition(originalTile.X, originalTile.Y);
        Point backToTile = tilemap.WorldToTilePosition(new Vector2(worldPos.X, worldPos.Y));

        Assert.Equal(originalTile, backToTile);
    }

    #endregion
}
