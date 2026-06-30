using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tests.Fixtures;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;

namespace MonoGame.Extended.Tests.Tilemaps.Rendering;

[Collection("GraphicsTest")]
public class TilemapSpriteBatchRendererTests
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;

    public TilemapSpriteBatchRendererTests(GraphicsTestFixture fixture)
    {
        _graphicsDevice = fixture.GraphicsDevice;
        _spriteBatch = fixture.SpriteBatch;
    }

    // ---- LoadTilemap ----

    [Fact]
    public void LoadTilemap_WithNullTilemap_ThrowsArgumentNullException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();

        Assert.Throws<ArgumentNullException>(() => renderer.LoadTilemap(null));
    }

    // ---- Update ----

    [Fact]
    public void Update_WithNullGameTime_ThrowsArgumentNullException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();

        Assert.Throws<ArgumentNullException>(() => renderer.Update(null));
    }

    [Fact]
    public void Update_WithAnimatedTiles_AdvancesFrame()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateTilemapWithAnimatedTile(frameDurationSeconds: 0.1f);
        renderer.LoadTilemap(tilemap);

        TilemapTileData animatedTile = tilemap.Tilesets[0].GetAnimatedTiles()[0];
        int initialFrame = animatedTile.Animation.CurrentFrameIndex;

        // Advance far enough to cross at least one frame boundary
        GameTime gameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.15));
        renderer.Update(gameTime);

        Assert.NotEqual(initialFrame, animatedTile.Animation.CurrentFrameIndex);
    }

    // ---- Draw ----

    [Fact]
    public void Draw_WithNullSpriteBatch_ThrowsArgumentNullException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<ArgumentNullException>(() => renderer.Draw(null, camera));
    }

    [Fact]
    public void Draw_WithNullCamera_ThrowsArgumentNullException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);

        Assert.Throws<ArgumentNullException>(() => renderer.Draw(_spriteBatch, null));
    }

    [Fact]
    public void Draw_WithoutLoadedTilemap_ThrowsInvalidOperationException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<InvalidOperationException>(() => renderer.Draw(_spriteBatch, camera));
    }

    // ---- DrawLayer by name ----

    [Fact]
    public void DrawLayer_ByName_WithNullSpriteBatch_ThrowsArgumentNullException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<ArgumentNullException>(() => renderer.DrawLayer(null, camera, "TestLayer"));
    }

    [Fact]
    public void DrawLayer_ByName_WithNullCamera_ThrowsArgumentNullException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);

        Assert.Throws<ArgumentNullException>(() => renderer.DrawLayer(_spriteBatch, null, "TestLayer"));
    }

    [Fact]
    public void DrawLayer_ByName_WithNullLayerName_ThrowsArgumentNullException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<ArgumentNullException>(() => renderer.DrawLayer(_spriteBatch, camera, (string)null));
    }

    [Fact]
    public void DrawLayer_ByName_WithInvalidName_ThrowsKeyNotFoundException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<KeyNotFoundException>(() => renderer.DrawLayer(_spriteBatch, camera, "DoesNotExist"));
    }

    [Fact]
    public void DrawLayer_ByName_WithoutLoadedTilemap_ThrowsInvalidOperationException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<InvalidOperationException>(() => renderer.DrawLayer(_spriteBatch, camera, "TestLayer"));
    }

    // ---- DrawLayer by index ----

    [Fact]
    public void DrawLayer_ByIndex_WithNullSpriteBatch_ThrowsArgumentNullException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<ArgumentNullException>(() => renderer.DrawLayer(null, camera, 0));
    }

    [Fact]
    public void DrawLayer_ByIndex_WithNullCamera_ThrowsArgumentNullException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);

        Assert.Throws<ArgumentNullException>(() => renderer.DrawLayer(_spriteBatch, null, 0));
    }

    [Fact]
    public void DrawLayer_ByIndex_WithNegativeIndex_ThrowsArgumentOutOfRangeException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<ArgumentOutOfRangeException>(() => renderer.DrawLayer(_spriteBatch, camera, -1));
    }

    [Fact]
    public void DrawLayer_ByIndex_WithIndexBeyondEnd_ThrowsArgumentOutOfRangeException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<ArgumentOutOfRangeException>(() => renderer.DrawLayer(_spriteBatch, camera, 999));
    }

    [Fact]
    public void DrawLayer_ByIndex_WithoutLoadedTilemap_ThrowsInvalidOperationException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<InvalidOperationException>(() => renderer.DrawLayer(_spriteBatch, camera, 0));
    }

    // ---- DrawLayers ----

    [Fact]
    public void DrawLayers_WithNullSpriteBatch_ThrowsArgumentNullException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<ArgumentNullException>(() => renderer.DrawLayers(null, camera, "Layer1", "Layer2"));
    }

    [Fact]
    public void DrawLayers_WithNullCamera_ThrowsArgumentNullException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);

        Assert.Throws<ArgumentNullException>(() => renderer.DrawLayers(_spriteBatch, null, "Layer1", "Layer2"));
    }

    [Fact]
    public void DrawLayers_WithNullLayerNames_ThrowsArgumentNullException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<ArgumentNullException>(() => renderer.DrawLayers(_spriteBatch, camera, (string[])null));
    }

    [Fact]
    public void DrawLayers_WithoutLoadedTilemap_ThrowsInvalidOperationException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<InvalidOperationException>(() => renderer.DrawLayers(_spriteBatch, camera, "Layer1"));
    }

    [Fact]
    public void DrawLayers_WithInvalidLayerName_ThrowsKeyNotFoundException()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<KeyNotFoundException>(() => renderer.DrawLayers(_spriteBatch, camera, "DoesNotExist"));
    }

    // ---- Helper methods ----

    private Tilemap CreateSimpleTilemap()
    {
        Tilemap tilemap = new Tilemap(
            name: "TestMap",
            width: 10,
            height: 10,
            tileWidth: 32,
            tileHeight: 32,
            orientation: TilemapOrientation.Orthogonal);

        TilemapTileLayer layer = new TilemapTileLayer(
            name: "TestLayer",
            width: 10,
            height: 10,
            tileWidth: 32,
            tileHeight: 32);

        tilemap.Layers.Add(layer);

        Texture2D dummyTexture = new Texture2D(_graphicsDevice, 32, 32);
        TilemapTileset tileset = new TilemapTileset(
            name: "TestTileset",
            texture: dummyTexture,
            tileWidth: 32,
            tileHeight: 32,
            tileCount: 10,
            columns: 10,
            spacing: 0,
            margin: 0)
        {
            FirstGlobalId = 1
        };

        tilemap.Tilesets.Add(tileset);

        return tilemap;
    }

    private Tilemap CreateMultiLayerTilemap()
    {
        Tilemap tilemap = new Tilemap(
            name: "MultiLayerMap",
            width: 5,
            height: 5,
            tileWidth: 32,
            tileHeight: 32,
            orientation: TilemapOrientation.Orthogonal);

        Texture2D texture = new Texture2D(_graphicsDevice, 64, 64);
        TilemapTileset tileset = new TilemapTileset(
            name: "TestTileset",
            texture: texture,
            tileWidth: 32,
            tileHeight: 32,
            tileCount: 4,
            columns: 2,
            spacing: 0,
            margin: 0)
        {
            FirstGlobalId = 1
        };

        tilemap.Tilesets.Add(tileset);

        for (int i = 1; i <= 4; i++)
        {
            TilemapTileLayer layer = new TilemapTileLayer(
                name: $"Layer{i}",
                width: 5,
                height: 5,
                tileWidth: 32,
                tileHeight: 32);

            tilemap.Layers.Add(layer);
        }

        return tilemap;
    }

    private Tilemap CreateTilemapWithAnimatedTile(float frameDurationSeconds)
    {
        Tilemap tilemap = new Tilemap(
            name: "AnimatedMap",
            width: 5,
            height: 5,
            tileWidth: 32,
            tileHeight: 32,
            orientation: TilemapOrientation.Orthogonal);

        Texture2D texture = new Texture2D(_graphicsDevice, 64, 32);
        TilemapTileset tileset = new TilemapTileset(
            name: "AnimatedTileset",
            texture: texture,
            tileWidth: 32,
            tileHeight: 32,
            tileCount: 2,
            columns: 2,
            spacing: 0,
            margin: 0)
        {
            FirstGlobalId = 1
        };

        TilemapTileData tileData = new TilemapTileData(localId: 0);
        tileData.Animation = new TilemapTileAnimation(new[]
        {
            new TilemapTileAnimationFrame(tileId: 0, duration: frameDurationSeconds),
            new TilemapTileAnimationFrame(tileId: 1, duration: frameDurationSeconds),
        });

        tileset.AddTileData(tileData);
        tilemap.Tilesets.Add(tileset);

        TilemapTileLayer layer = new TilemapTileLayer(
            name: "AnimatedLayer",
            width: 5,
            height: 5,
            tileWidth: 32,
            tileHeight: 32);

        tilemap.Layers.Add(layer);

        return tilemap;
    }

}
