using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Tilemaps;

namespace MonoGame.Extended.Tests;

public sealed class TilemapLayerTests
{
    [Fact]
    public void GetLayer_WithExistingLayer_ReturnsLayer()
    {
        Tilemap tilemap = new Tilemap("test", 10, 10, 32, 32, TilemapOrientation.Orthogonal);
        TilemapTileLayer layer = new TilemapTileLayer("TestLayer", 10, 10, 32, 32);
        tilemap.Layers.Add(layer);

        TilemapLayer result = tilemap.Layers["TestLayer"];

        Assert.Equal(layer, result);
    }

    [Fact]
    public void GetLayer_WithNonExistingLayer_ReturnsNull()
    {
        Tilemap tilemap = new Tilemap("test", 10, 10, 32, 32, TilemapOrientation.Orthogonal);
        bool hasLayer = tilemap.Layers.TryGetValue("NonExistent", out TilemapLayer result);
        Assert.False(hasLayer);
        Assert.Null(result);
    }

    [Fact]
    public void GetLayer_Generic_WithCorrectType_ReturnsLayer()
    {
        Tilemap tilemap = new Tilemap("test", 10, 10, 32, 32, TilemapOrientation.Orthogonal);
        TilemapTileLayer tileLayer = new TilemapTileLayer("TileLayer", 10, 10, 32, 32);
        tilemap.Layers.Add(tileLayer);

        TilemapTileLayer result = tilemap.Layers.GetLayer<TilemapTileLayer>("TileLayer");

        Assert.Equal(tileLayer, result);
    }

    [Fact]
    public void GetLayer_Generic_WithWrongType_ReturnsNull()
    {
        Tilemap tilemap = new Tilemap("test", 10, 10, 32, 32, TilemapOrientation.Orthogonal);
        TilemapTileLayer tileLayer = new TilemapTileLayer("TileLayer", 10, 10, 32, 32);
        tilemap.Layers.Add(tileLayer);

        TilemapObjectLayer result = tilemap.Layers.GetLayer<TilemapObjectLayer>("TileLayer");

        Assert.Null(result);
    }

    [Fact]
    public void GetLayers_Generic_ReturnsLayersOfType()
    {
        Tilemap tilemap = new Tilemap("test", 10, 10, 32, 32, TilemapOrientation.Orthogonal);
        TilemapTileLayer tileLayer1 = new TilemapTileLayer("Tiles1", 10, 10, 32, 32);
        TilemapTileLayer tileLayer2 = new TilemapTileLayer("Tiles2", 10, 10, 32, 32);
        TilemapObjectLayer objectLayer = new TilemapObjectLayer("Objects");

        tilemap.Layers.Add(tileLayer1);
        tilemap.Layers.Add(objectLayer);
        tilemap.Layers.Add(tileLayer2);

        List<TilemapTileLayer> tileLayers = tilemap.Layers.GetLayers<TilemapTileLayer>().ToList();

        Assert.Equal(2, tileLayers.Count);
        Assert.Contains(tileLayer1, tileLayers);
        Assert.Contains(tileLayer2, tileLayers);
    }

    [Fact]
    public void GetLayers_Generic_WithNoMatchingLayers_ReturnsEmpty()
    {
        Tilemap tilemap = new Tilemap("test", 10, 10, 32, 32, TilemapOrientation.Orthogonal);
        TilemapTileLayer tileLayer = new TilemapTileLayer("Tiles", 10, 10, 32, 32);
        tilemap.Layers.Add(tileLayer);

        List<TilemapObjectLayer> objectLayers = tilemap.Layers.GetLayers<TilemapObjectLayer>().ToList();

        Assert.Empty(objectLayers);
    }
}
