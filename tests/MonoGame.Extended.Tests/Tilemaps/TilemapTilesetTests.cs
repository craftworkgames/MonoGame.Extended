using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tests.Fixtures;
using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Tests;

[Collection("GraphicsTest")]
public sealed class TilemapTilesetTests
{
    private readonly GraphicsTestFixture _graphicsFixture;

    public TilemapTilesetTests(GraphicsTestFixture graphicsTestFixture)
    {
        _graphicsFixture = graphicsTestFixture;
    }

    [Theory]
    [InlineData(0, 0, 0)]      // First tile
    [InlineData(1, 32, 0)]     // Second tile (column 1)
    [InlineData(9, 288, 0)]    // Last tile in first row (column 9)
    [InlineData(10, 0, 32)]    // First tile in second row
    [InlineData(11, 32, 32)]   // Second tile in second row
    [InlineData(25, 160, 64)]  // Tile at column 5, row 2
    public void GetSourceRectangle_CalculatesCorrectPosition(int localId, int expectedX, int expectedY)
    {
        Texture2D texture = _graphicsFixture.CreatePixelTexture();

        try
        {
            TilemapTileset tileset = new TilemapTileset("Test", texture, 32, 32, 100, 10);

            Rectangle rect = tileset.GetTileRegion(localId);

            Assert.Equal(expectedX, rect.X);
            Assert.Equal(expectedY, rect.Y);
            Assert.Equal(32, rect.Width);
            Assert.Equal(32, rect.Height);
        }
        finally
        {
            texture.Dispose();
        }
    }

    [Fact]
    public void GetSourceRectangle_WithDifferentTileSize_CalculatesCorrectly()
    {
        Texture2D texture = _graphicsFixture.CreatePixelTexture();

        try
        {
            TilemapTileset tileset = new TilemapTileset("Test", texture, 16, 24, 100, 8);

            // Column 2, Row 1
            Rectangle rect = tileset.GetTileRegion(10);

            Assert.Equal(32, rect.X);
            Assert.Equal(24, rect.Y);
            Assert.Equal(16, rect.Width);
            Assert.Equal(24, rect.Height);
        }
        finally
        {
            texture.Dispose();
        }
    }

    [Fact]
    public void GetTileData_WithNoData_ReturnsNull()
    {
        Texture2D texture = _graphicsFixture.CreatePixelTexture();

        try
        {
            TilemapTileset tileset = new TilemapTileset("Test", texture, 32, 32, 100, 10);

            TilemapTileData data = tileset.GetTileData(5);

            Assert.Null(data);
        }
        finally
        {
            texture.Dispose();
        }
    }

    [Fact]
    public void AddTileData_AndGetTileData_ReturnsData()
    {
        Texture2D texture = _graphicsFixture.CreatePixelTexture();

        try
        {
            TilemapTileset tileset = new TilemapTileset("Test", texture, 32, 32, 100, 10);
            TilemapTileData tileData = new TilemapTileData(5);
            tileData.Class = "Water";

            tileset.AddTileData(tileData);
            TilemapTileData retrieved = tileset.GetTileData(5);

            Assert.NotNull(retrieved);
            Assert.Equal(5, retrieved.LocalId);
            Assert.Equal("Water", retrieved.Class);
        }
        finally
        {
            texture.Dispose();
        }
    }

    [Fact]
    public void AddTileData_WithMultipleTiles_CanRetrieveEach()
    {
        Texture2D texture = _graphicsFixture.CreatePixelTexture();

        try
        {
            TilemapTileset tileset = new TilemapTileset("Test", texture, 32, 32, 100, 10);
            TilemapTileData data1 = new TilemapTileData(1) { Class = "Grass" };
            TilemapTileData data2 = new TilemapTileData(2) { Class = "Stone" };
            TilemapTileData data3 = new TilemapTileData(3) { Class = "Water" };

            tileset.AddTileData(data1);
            tileset.AddTileData(data2);
            tileset.AddTileData(data3);

            Assert.Equal("Grass", tileset.GetTileData(1)?.Class);
            Assert.Equal("Stone", tileset.GetTileData(2)?.Class);
            Assert.Equal("Water", tileset.GetTileData(3)?.Class);
            Assert.Null(tileset.GetTileData(4));
        }
        finally
        {
            texture.Dispose();
        }
    }

    [Fact]
    public void AddTileData_WithSameLocalId_OverwritesPrevious()
    {
        Texture2D texture = _graphicsFixture.CreatePixelTexture();

        try
        {
            TilemapTileset tileset = new TilemapTileset("Test", texture, 32, 32, 100, 10);
            TilemapTileData data1 = new TilemapTileData(5) { Class = "Old" };
            TilemapTileData data2 = new TilemapTileData(5) { Class = "New" };

            tileset.AddTileData(data1);
            tileset.AddTileData(data2);
            TilemapTileData retrieved = tileset.GetTileData(5);

            Assert.NotNull(retrieved);
            Assert.Equal("New", retrieved.Class);
        }
        finally
        {
            texture.Dispose();
        }
    }

    [Fact]
    public void GetTileRegion_OnCollectionTileset_ThrowsInvalidOperationException()
    {
        // Regression test: collection tilesets have Columns=0, which caused a DivideByZeroException.
        TilemapTileset tileset = new TilemapTileset("Collection", null, 128, 108, 7, 0);

        Assert.Throws<InvalidOperationException>(() => tileset.GetTileRegion(0));
    }
}

