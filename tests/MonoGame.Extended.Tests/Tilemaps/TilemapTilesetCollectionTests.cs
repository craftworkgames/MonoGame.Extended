using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Tests;

public sealed class TilemapTilesetCollectionTests
{
    private Texture2D CreateDummyTexture()
    {
        // TODO: setup with xvfb later
        return null;
    }

    [Fact]
    public void GetTilesetForGid_WithSingleTileset_ReturnsCorrectTileset()
    {
        TilemapTilesetCollection collection = new TilemapTilesetCollection();
        TilemapTileset tileset = new TilemapTileset("Tileset", CreateDummyTexture(), 32, 32, 100, 10)
        {
            FirstGlobalId = 1
        };
        collection.Add(tileset);

        TilemapTileset result = collection.GetTilesetForGid(50);

        Assert.Equal(tileset, result);
    }

    [Fact]
    public void GetTilesetForGid_WithMultipleTilesets_ReturnsCorrectTileset()
    {
        TilemapTilesetCollection collection = new TilemapTilesetCollection();

        TilemapTileset tileset1 = new TilemapTileset("Tileset1", CreateDummyTexture(), 32, 32, 100, 10)
        {
            FirstGlobalId = 1
        };

        TilemapTileset tileset2 = new TilemapTileset("Tileset2", CreateDummyTexture(), 32, 32, 50, 5)
        {
            // After tileset1
            FirstGlobalId = 101
        };

        TilemapTileset tileset3 = new TilemapTileset("Tileset3", CreateDummyTexture(), 32, 32, 25, 5)
        {
            // After tileset2
            FirstGlobalId = 151
        };

        collection.Add(tileset1);
        collection.Add(tileset2);
        collection.Add(tileset3);

        // Fist tile of tileset1
        Assert.Equal(tileset1, collection.GetTilesetForGid(1));
        // Mid tileset 1
        Assert.Equal(tileset1, collection.GetTilesetForGid(50));
        // Last tile of tileset1
        Assert.Equal(tileset1, collection.GetTilesetForGid(100));

        // First tile of tileset2
        Assert.Equal(tileset2, collection.GetTilesetForGid(101));
        // Mid of tileset2
        Assert.Equal(tileset2, collection.GetTilesetForGid(125));
        // Last tile of tileset2
        Assert.Equal(tileset2, collection.GetTilesetForGid(150));

        // First tile of tileset3
        Assert.Equal(tileset3, collection.GetTilesetForGid(151));
        // Last tile of tileset3
        Assert.Equal(tileset3, collection.GetTilesetForGid(175));
    }

    [Fact]
    public void GetTilesetForGid_WithGidBeforeFirstTileset_ReturnsNull()
    {
        TilemapTilesetCollection collection = new TilemapTilesetCollection();
        TilemapTileset tileset = new TilemapTileset("Tileset", CreateDummyTexture(), 32, 32, 100, 10)
        {
            FirstGlobalId = 10
        };
        collection.Add(tileset);

        TilemapTileset result = collection.GetTilesetForGid(5);

        Assert.Null(result);
    }

    [Fact]
    public void GetTilesetForGid_WithEmptyCollection_ReturnsNull()
    {
        TilemapTilesetCollection collection = new TilemapTilesetCollection();
        TilemapTileset result = collection.GetTilesetForGid(1);
        Assert.Null(result);
    }

    [Fact]
    public void GetLocalId_WithValidGid_ReturnsCorrectLocalId()
    {
        TilemapTilesetCollection collection = new TilemapTilesetCollection();
        TilemapTileset tileset = new TilemapTileset("Tileset", CreateDummyTexture(), 32, 32, 100, 10)
        {
            FirstGlobalId = 1
        };
        collection.Add(tileset);

        int localId = collection.GetLocalId(50, out TilemapTileset outTileset);

        // 50 - 1 = 49
        Assert.Equal(49, localId);
        Assert.Equal(tileset, outTileset);
    }

    [Fact]
    public void GetLocalId_WithFirstGid_ReturnsZero()
    {
        TilemapTilesetCollection collection = new TilemapTilesetCollection();
        TilemapTileset tileset = new TilemapTileset("Tileset", CreateDummyTexture(), 32, 32, 100, 10)
        {
            FirstGlobalId = 100
        };
        collection.Add(tileset);

        int localId = collection.GetLocalId(100, out TilemapTileset outTileset);

        Assert.Equal(0, localId);
        Assert.Equal(tileset, outTileset);
    }

    [Fact]
    public void GetLocalId_WithMultipleTilesets_ReturnsCorrectLocalId()
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

        int localId1 = collection.GetLocalId(50, out TilemapTileset outTileset1);
        int localId2 = collection.GetLocalId(125, out TilemapTileset outTileset2);

        Assert.Equal(49, localId1);
        Assert.Equal(tileset1, outTileset1);

        Assert.Equal(24, localId2);
        Assert.Equal(tileset2, outTileset2);
    }

    [Fact]
    public void GetLocalId_WithInvalidGid_ThrowsInvalidOperationException()
    {
        TilemapTilesetCollection collection = new TilemapTilesetCollection();
        TilemapTileset tileset = new TilemapTileset("Tileset", CreateDummyTexture(), 32, 32, 100, 10)
        {
            FirstGlobalId = 10
        };
        collection.Add(tileset);

        Assert.Throws<InvalidOperationException>(() => collection.GetLocalId(5, out _));
    }

    [Fact]
    public void GetTilesetForGid_WithCollectionTilesetNonSequentialIds_ReturnsCorrectTileset()
    {
        // Regression test: collection tilesets assign arbitrary tile IDs that can be much larger
        // than tileCount, so the lookup must use the tracked max local ID, not tileCount.
        // The data for this test mirrors a real world case that was given by a user on discord.
        TilemapTilesetCollection collection = new TilemapTilesetCollection();
        TilemapTileset tileset = new TilemapTileset("Props", CreateDummyTexture(), 56, 63, 35, 0)
        {
            FirstGlobalId = 6106
        };
        tileset.AddTileData(new TilemapTileData(86));
        tileset.AddTileData(new TilemapTileData(87));
        tileset.AddTileData(new TilemapTileData(187));
        collection.Add(tileset);

        // GlobalId 6192 = firstGid(6106) + localId(86)
        TilemapTileset foundTileset = collection.GetTilesetForGid(6192);
        int localId = collection.GetLocalId(6192, out TilemapTileset outTileset);

        Assert.Equal(tileset, foundTileset);
        Assert.Equal(86, localId);
        Assert.Equal(tileset, outTileset);
    }

}
