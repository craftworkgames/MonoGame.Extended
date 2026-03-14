using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Tests;

public sealed class TilemapTileTests
{
    private Texture2D CreateDummyTexture()
    {
        // TODO: setup with xvfb later
        return null;
    }

    private TilemapTilesetCollection CreateTestTilesets()
    {
        TilemapTilesetCollection collection = new TilemapTilesetCollection();

        TilemapTileset tileset1 = new TilemapTileset("Tileset1", CreateDummyTexture(), 32, 32, 100, 10)
        {
            FirstGlobalId = 1
        };

        TilemapTileset tileset2 = new TilemapTileset("Tileset2", CreateDummyTexture(), 32, 32, 50, 5)
        {
            FirstGlobalId = 101
        };

        collection.Add(tileset1);
        collection.Add(tileset2);

        return collection;
    }

    [Fact]
    public void GetTileset_WithValidGid_ReturnsTileset()
    {
        TilemapTilesetCollection tilesets = CreateTestTilesets();
        TilemapTile tile = new TilemapTile(50);

        TilemapTileset tileset = tile.GetTileset(tilesets);

        Assert.NotNull(tileset);
        Assert.Equal("Tileset1", tileset.Name);
    }

    [Fact]
    public void GetTileset_WithInvalidGid_ReturnsNull()
    {
        TilemapTilesetCollection tilesets = CreateTestTilesets();

        // Beyond any tileset
        TilemapTile tile = new TilemapTile(999);

        TilemapTileset tileset = tile.GetTileset(tilesets);

        Assert.Null(tileset);
    }

    [Fact]
    public void GetLocalId_WithOutParameter_ReturnsTilesetAndLocalId()
    {
        TilemapTilesetCollection tilesets = CreateTestTilesets();
        TilemapTile tile = new TilemapTile(125);

        var localId = tile.GetLocalId(tilesets, out TilemapTileset tileset);

        Assert.Equal(24, localId);
        Assert.NotNull(tileset);
        Assert.Equal("Tileset2", tileset.Name);
    }

    [Fact]
    public void GetTileData_WithValidGid_ReturnsTileData()
    {
        TilemapTilesetCollection tilesets = CreateTestTilesets();
        TilemapTileData tileData = new TilemapTileData(49) { Class = "Water" };
        tilesets[0].AddTileData(tileData);

        // Tileset1 FirstGlobalId=1, so localId=49
        TilemapTile tile = new TilemapTile(50);

        TilemapTileData retrieved = tile.GetTileData(tilesets);

        Assert.NotNull(retrieved);
        Assert.Equal("Water", retrieved.Class);
    }

    [Fact]
    public void GetTileData_WithNoTileData_ReturnsNull()
    {
        TilemapTilesetCollection tilesets = CreateTestTilesets();
        TilemapTile tile = new TilemapTile(50);

        TilemapTileData retrieved = tile.GetTileData(tilesets);

        Assert.Null(retrieved);
    }

    [Fact]
    public void GetTileData_WithInvalidGid_ReturnsNull()
    {
        TilemapTilesetCollection tilesets = CreateTestTilesets();
        TilemapTile tile = new TilemapTile(999);

        TilemapTileData retrieved = tile.GetTileData(tilesets);

        Assert.Null(retrieved);
    }

}
