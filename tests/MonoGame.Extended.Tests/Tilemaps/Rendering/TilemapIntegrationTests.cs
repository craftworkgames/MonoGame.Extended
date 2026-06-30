using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tests.Fixtures;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;

namespace MonoGame.Extended.Tests.Tilemaps.Rendering;

/// <summary>
/// Integration tests covering realistic end-to-end rendering scenarios for both
/// TilemapRenderer and TilemapSpriteBatchRenderer.
///
/// Pixel assertions are possible because:
/// - Camera at Position=(0,0) produces an identity view matrix (verified against the
///   GetVirtualViewMatrix formula), so world coordinates map directly to screen pixels.
/// - The GD renderer's projection is CreateOrthographicOffCenter(0, vpW, vpH, 0),
///   which maps world (x,y) to screen pixel (x,y) without transformation.
/// - Tiles placed at layer cell (0,0) appear at world (0,0), so the tile center is
///   at screen pixel (tileSize/2, tileSize/2) = (16,16) for 32x32 tiles.
/// - Both renderers use BlendState.NonPremultiplied: opacity=0 produces the clear
///   color; a fully-opaque white tile on a black background produces white.
/// </summary>
[Collection("GraphicsTest")]
public class TilemapIntegrationTests
{
    private const int TileSize = 32;
    private const int TileCenterX = TileSize / 2;  // 16
    private const int TileCenterY = TileSize / 2;  // 16

    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;

    public TilemapIntegrationTests(GraphicsTestFixture fixture)
    {
        _graphicsDevice = fixture.GraphicsDevice;
        _spriteBatch = fixture.SpriteBatch;
    }

    // ---- Tiles are drawn ----

    [Fact]
    public void GdRenderer_OrthogonalMap_WithTilesPlaced_DrawsPixels()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateOrthogonalMap(Color.White);
        renderer.LoadTilemap(tilemap);

        var (pixels, width) = RenderToPixels(() =>
        {
            renderer.Draw(new OrthographicCamera(_graphicsDevice));
        });

        Assert.Equal(Color.White, GetPixel(pixels, width, TileCenterX, TileCenterY));
    }

    [Fact]
    public void SbRenderer_OrthogonalMap_WithTilesPlaced_DrawsPixels()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateOrthogonalMap(Color.White);
        renderer.LoadTilemap(tilemap);

        var (pixels, width) = RenderToPixels(() =>
        {
            OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);
            renderer.Draw(_spriteBatch, camera);
        });

        Assert.Equal(Color.White, GetPixel(pixels, width, TileCenterX, TileCenterY));
    }

    // ---- Layer visibility ----

    [Fact]
    public void GdRenderer_HiddenLayer_DrawsNoPixels()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateOrthogonalMap(Color.White);
        tilemap.Layers[0].IsVisible = false;
        renderer.LoadTilemap(tilemap);

        var (pixels, width) = RenderToPixels(() =>
        {
            renderer.Draw(new OrthographicCamera(_graphicsDevice));
        });

        Assert.Equal(Color.Black, GetPixel(pixels, width, TileCenterX, TileCenterY));
    }

    [Fact]
    public void SbRenderer_HiddenLayer_DrawsNoPixels()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateOrthogonalMap(Color.White);
        renderer.LoadTilemap(tilemap);
        tilemap.Layers[0].IsVisible = false;

        var (pixels, width) = RenderToPixels(() =>
        {
            renderer.Draw(_spriteBatch, new OrthographicCamera(_graphicsDevice));
        });

        Assert.Equal(Color.Black, GetPixel(pixels, width, TileCenterX, TileCenterY));
    }

    // ---- Opacity ----

    [Fact]
    public void GdRenderer_ZeroOpacity_DrawsNoPixels()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        // Opacity must be set before LoadTilemap: GD renderer bakes opacity into vertex alpha.
        Tilemap tilemap = CreateOrthogonalMap(Color.White, opacity: 0f);
        renderer.LoadTilemap(tilemap);

        var (pixels, width) = RenderToPixels(() =>
        {
            renderer.Draw(new OrthographicCamera(_graphicsDevice));
        });

        Assert.Equal(Color.Black, GetPixel(pixels, width, TileCenterX, TileCenterY));
    }

    [Fact]
    public void SbRenderer_ZeroOpacity_DrawsNoPixels()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateOrthogonalMap(Color.White, opacity: 0f);
        renderer.LoadTilemap(tilemap);

        var (pixels, width) = RenderToPixels(() =>
        {
            renderer.Draw(_spriteBatch, new OrthographicCamera(_graphicsDevice));
        });

        Assert.Equal(Color.Black, GetPixel(pixels, width, TileCenterX, TileCenterY));
    }

    [Fact]
    public void SbRenderer_PartialOpacity_ProducesDifferentPixelThanFullOpacity()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();

        Tilemap fullyOpaque = CreateOrthogonalMap(Color.White, opacity: 1f);
        renderer.LoadTilemap(fullyOpaque);
        var (fullPixels, fullWidth) = RenderToPixels(() =>
        {
            renderer.Draw(_spriteBatch, new OrthographicCamera(_graphicsDevice));
        });

        Tilemap halfOpacity = CreateOrthogonalMap(Color.White, opacity: 0.5f);
        renderer.LoadTilemap(halfOpacity);
        var (halfPixels, halfWidth) = RenderToPixels(() =>
        {
            renderer.Draw(_spriteBatch, new OrthographicCamera(_graphicsDevice));
        });

        Color fullPixel = GetPixel(fullPixels, fullWidth, TileCenterX, TileCenterY);
        Color halfPixel = GetPixel(halfPixels, halfWidth, TileCenterX, TileCenterY);

        Assert.NotEqual(fullPixel, halfPixel);
    }

    // ---- Tint color (SpriteBatch only; GD renderer bakes opacity but not tint) ----

    [Fact]
    public void SbRenderer_RedTintOnWhiteTile_RendersRedPixel()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateOrthogonalMap(Color.White);
        tilemap.Layers[0].TintColor = Color.Red;
        renderer.LoadTilemap(tilemap);

        var (pixels, width) = RenderToPixels(() =>
        {
            renderer.Draw(_spriteBatch, new OrthographicCamera(_graphicsDevice));
        });

        Color pixel = GetPixel(pixels, width, TileCenterX, TileCenterY);
        Assert.True(pixel.R > 200, $"Expected high red component, got {pixel}");
        Assert.True(pixel.G < 10, $"Expected near-zero green component, got {pixel}");
        Assert.True(pixel.B < 10, $"Expected near-zero blue component, got {pixel}");
    }

    // ---- Camera transformations ----

    [Fact]
    public void GdRenderer_CameraViewingTiles_DrawsPixels()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateOrthogonalMap(Color.White);
        renderer.LoadTilemap(tilemap);

        var (pixels, _) = RenderToPixels(() =>
        {
            renderer.Draw(new OrthographicCamera(_graphicsDevice));
        });

        Assert.True(AnyPixelNotBlack(pixels));
    }

    [Fact]
    public void GdRenderer_CameraFarFromTiles_DrawsNoPixels()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateOrthogonalMap(Color.White);
        renderer.LoadTilemap(tilemap);

        var (pixels, _) = RenderToPixels(() =>
        {
            OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);
            camera.LookAt(new Vector2(100000f, 100000f));
            renderer.Draw(camera);
        });

        Assert.True(AllPixelsBlack(pixels));
    }

    [Fact]
    public void SbRenderer_CameraViewingTiles_DrawsPixels()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateOrthogonalMap(Color.White);
        renderer.LoadTilemap(tilemap);

        var (pixels, _) = RenderToPixels(() =>
        {
            renderer.Draw(_spriteBatch, new OrthographicCamera(_graphicsDevice));
        });

        Assert.True(AnyPixelNotBlack(pixels));
    }

    [Fact]
    public void SbRenderer_CameraFarFromTiles_DrawsNoPixels()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateOrthogonalMap(Color.White);
        renderer.LoadTilemap(tilemap);

        var (pixels, _) = RenderToPixels(() =>
        {
            OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);
            camera.LookAt(new Vector2(100000f, 100000f));
            renderer.Draw(_spriteBatch, camera);
        });

        Assert.True(AllPixelsBlack(pixels));
    }

    [Fact]
    public void GdRenderer_ZoomedInCamera_DrawsPixels()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateOrthogonalMap(Color.White);
        renderer.LoadTilemap(tilemap);

        var (pixels, _) = RenderToPixels(() =>
        {
            OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);
            camera.ZoomIn(2f);
            renderer.Draw(camera);
        });

        Assert.True(AnyPixelNotBlack(pixels));
    }

    [Fact]
    public void SbRenderer_ZoomedInCamera_DrawsPixels()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateOrthogonalMap(Color.White);
        renderer.LoadTilemap(tilemap);

        var (pixels, _) = RenderToPixels(() =>
        {
            OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);
            camera.ZoomIn(2f);
            renderer.Draw(_spriteBatch, camera);
        });

        Assert.True(AnyPixelNotBlack(pixels));
    }

    // ---- Animation frame advancement ----

    [Fact]
    public void SbRenderer_AnimationFrameAdvancement_PixelColorChanges()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateAnimatedTilemap(frame0Color: Color.Red, frame1Color: Color.Blue, frameDuration: 0.1f);
        renderer.LoadTilemap(tilemap);

        var (frame0Pixels, frame0Width) = RenderToPixels(() =>
        {
            renderer.Draw(_spriteBatch, new OrthographicCamera(_graphicsDevice));
        });

        renderer.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.15)));

        var (frame1Pixels, frame1Width) = RenderToPixels(() =>
        {
            renderer.Draw(_spriteBatch, new OrthographicCamera(_graphicsDevice));
        });

        Color before = GetPixel(frame0Pixels, frame0Width, TileCenterX, TileCenterY);
        Color after = GetPixel(frame1Pixels, frame1Width, TileCenterX, TileCenterY);

        // Frame 0 = red; frame 1 = blue. Colors must be visibly different.
        Assert.True(before.R > 200, $"Expected red pixel before animation advance, got {before}");
        Assert.True(after.B > 200, $"Expected blue pixel after animation advance, got {after}");
        Assert.NotEqual(before, after);
    }

    [Fact]
    public void GdRenderer_AnimationFrameAdvancement_PixelColorChanges()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateAnimatedTilemap(frame0Color: Color.Red, frame1Color: Color.Blue, frameDuration: 0.1f);
        renderer.LoadTilemap(tilemap);

        var (frame0Pixels, frame0Width) = RenderToPixels(() =>
        {
            renderer.Draw(new OrthographicCamera(_graphicsDevice));
        });

        // Update advances the animation and marks the VBO dirty.
        renderer.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.15)));

        // The second draw rebuilds the VBO with the new frame's UV coordinates.
        var (frame1Pixels, frame1Width) = RenderToPixels(() =>
        {
            renderer.Draw(new OrthographicCamera(_graphicsDevice));
        });

        Color before = GetPixel(frame0Pixels, frame0Width, TileCenterX, TileCenterY);
        Color after = GetPixel(frame1Pixels, frame1Width, TileCenterX, TileCenterY);

        Assert.True(before.R > 200, $"Expected red pixel before animation advance, got {before}");
        Assert.True(after.B > 200, $"Expected blue pixel after animation advance, got {after}");
        Assert.NotEqual(before, after);
    }

    // ---- All flip flag combinations ----

    [Theory]
    [InlineData(TilemapTileFlipFlags.None)]
    [InlineData(TilemapTileFlipFlags.FlipHorizontally)]
    [InlineData(TilemapTileFlipFlags.FlipVertically)]
    [InlineData(TilemapTileFlipFlags.FlipHorizontally | TilemapTileFlipFlags.FlipVertically)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally | TilemapTileFlipFlags.FlipHorizontally)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally | TilemapTileFlipFlags.FlipVertically)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally | TilemapTileFlipFlags.FlipHorizontally | TilemapTileFlipFlags.FlipVertically)]
    public void SbRenderer_AllFlipFlagCombinations_TileIsDrawn(TilemapTileFlipFlags flags)
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateSingleTileMap(Color.White, flags);
        renderer.LoadTilemap(tilemap);

        var (pixels, _) = RenderToPixels(() =>
        {
            renderer.Draw(_spriteBatch, new OrthographicCamera(_graphicsDevice));
        });

        Assert.True(AnyPixelNotBlack(pixels), $"Expected tile to be drawn for flip flags {flags}");
    }

    [Theory]
    [InlineData(TilemapTileFlipFlags.None)]
    [InlineData(TilemapTileFlipFlags.FlipHorizontally)]
    [InlineData(TilemapTileFlipFlags.FlipVertically)]
    [InlineData(TilemapTileFlipFlags.FlipHorizontally | TilemapTileFlipFlags.FlipVertically)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally | TilemapTileFlipFlags.FlipHorizontally)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally | TilemapTileFlipFlags.FlipVertically)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally | TilemapTileFlipFlags.FlipHorizontally | TilemapTileFlipFlags.FlipVertically)]
    public void GdRenderer_AllFlipFlagCombinations_TileIsDrawn(TilemapTileFlipFlags flags)
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateSingleTileMap(Color.White, flags);
        renderer.LoadTilemap(tilemap);

        var (pixels, _) = RenderToPixels(() =>
        {
            renderer.Draw(new OrthographicCamera(_graphicsDevice));
        });

        Assert.True(AnyPixelNotBlack(pixels), $"Expected tile to be drawn for flip flags {flags}");
    }

    [Theory]
    // Added while investigating issue #1138:
    // https://github.com/MonoGame-Extended/Monogame-Extended/issues/1138
    // These assertions verify the exact orientation Tiled expects for every
    // diagonal/horizontal/vertical flag combination, not just that some pixels draw.
    [InlineData(TilemapTileFlipFlags.None)]
    [InlineData(TilemapTileFlipFlags.FlipHorizontally)]
    [InlineData(TilemapTileFlipFlags.FlipVertically)]
    [InlineData(TilemapTileFlipFlags.FlipHorizontally | TilemapTileFlipFlags.FlipVertically)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally | TilemapTileFlipFlags.FlipHorizontally)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally | TilemapTileFlipFlags.FlipVertically)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally | TilemapTileFlipFlags.FlipHorizontally | TilemapTileFlipFlags.FlipVertically)]
    public void SbRenderer_AllFlipFlagCombinations_MatchTiledOrientation(TilemapTileFlipFlags flags)
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateSingleTileMap(CreateQuadrantTexture(), flags);
        renderer.LoadTilemap(tilemap);

        var (pixels, width) = RenderToPixels(() =>
        {
            renderer.Draw(_spriteBatch, new OrthographicCamera(_graphicsDevice));
        });

        AssertTileQuadrantsMatchExpected(pixels, width, flags);
    }

    [Theory]
    // Added while investigating issue #1138:
    // https://github.com/MonoGame-Extended/Monogame-Extended/issues/1138
    // This mirrors the SpriteBatch test above so both renderers are held to the
    // same Tiled orientation rules for diagonal flip combinations.
    [InlineData(TilemapTileFlipFlags.None)]
    [InlineData(TilemapTileFlipFlags.FlipHorizontally)]
    [InlineData(TilemapTileFlipFlags.FlipVertically)]
    [InlineData(TilemapTileFlipFlags.FlipHorizontally | TilemapTileFlipFlags.FlipVertically)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally | TilemapTileFlipFlags.FlipHorizontally)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally | TilemapTileFlipFlags.FlipVertically)]
    [InlineData(TilemapTileFlipFlags.FlipDiagonally | TilemapTileFlipFlags.FlipHorizontally | TilemapTileFlipFlags.FlipVertically)]
    public void GdRenderer_AllFlipFlagCombinations_MatchTiledOrientation(TilemapTileFlipFlags flags)
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateSingleTileMap(CreateQuadrantTexture(), flags);
        renderer.LoadTilemap(tilemap);

        var (pixels, width) = RenderToPixels(() =>
        {
            renderer.Draw(new OrthographicCamera(_graphicsDevice));
        });

        AssertTileQuadrantsMatchExpected(pixels, width, flags);
    }

    // ---- Vic's scenario: many layers with groups ----

    [Fact]
    public void GdRenderer_ManyLayersWithGroups_DrawsPixels()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateManyLayerTilemap(layerCount: 30, tileColor: Color.White);
        renderer.LoadTilemap(tilemap);

        renderer.DefineLayerGroup("Background", startIndex: 0, count: 10);
        renderer.DefineLayerGroup("Midground", startIndex: 10, count: 10);
        renderer.DefineLayerGroup("Foreground", startIndex: 20, count: 10);

        var (pixels, _) = RenderToPixels(() =>
        {
            OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);
            renderer.BeginDraw(camera);
            renderer.DrawLayerGroup("Background");
            renderer.DrawLayerGroup("Midground");
            renderer.DrawLayerGroup("Foreground");
            renderer.EndDraw();
        });

        Assert.True(AnyPixelNotBlack(pixels));
    }

    // ---- Dynamic group changes at runtime ----

    [Fact]
    public void GdRenderer_DynamicGroupChanges_DrawsPixelsAfterEachChange()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateManyLayerTilemap(layerCount: 6, tileColor: Color.White);
        renderer.LoadTilemap(tilemap);

        renderer.DefineLayerGroup("GroupA", startIndex: 0, count: 3);
        renderer.DefineLayerGroup("GroupB", startIndex: 3, count: 3);
        var (pixels1, _) = RenderToPixels(() =>
            renderer.Draw(new OrthographicCamera(_graphicsDevice)));
        Assert.True(AnyPixelNotBlack(pixels1));

        renderer.DefineLayerGroup("GroupA", startIndex: 0, count: 2);
        renderer.DefineLayerGroup("GroupB", startIndex: 2, count: 4);
        var (pixels2, _) = RenderToPixels(() =>
            renderer.Draw(new OrthographicCamera(_graphicsDevice)));
        Assert.True(AnyPixelNotBlack(pixels2));

        renderer.RemoveLayerGroup("GroupA");
        var (pixels3, _) = RenderToPixels(() =>
            renderer.Draw(new OrthographicCamera(_graphicsDevice)));
        Assert.True(AnyPixelNotBlack(pixels3));
    }

    // ---- Mixed GD renderer + SpriteBatch interleaving ----

    [Fact]
    public void GdRenderer_InterleavedWithSpriteBatch_DrawsPixels()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateManyLayerTilemap(layerCount: 4, tileColor: Color.White);
        renderer.LoadTilemap(tilemap);

        renderer.DefineLayerGroup("Background", startIndex: 0, count: 2);
        renderer.DefineLayerGroup("Foreground", startIndex: 2, count: 2);

        var (pixels, _) = RenderToPixels(() =>
        {
            OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);
            renderer.BeginDraw(camera);
            renderer.DrawLayerGroup("Background");

            renderer.SaveGraphicsDeviceState();
            _spriteBatch.Begin();
            _spriteBatch.End();
            renderer.RestoreGraphicsDeviceState();

            renderer.DrawLayerGroup("Foreground");
            renderer.EndDraw();
        });

        Assert.True(AnyPixelNotBlack(pixels));
    }

    // ---- LoadTilemap / UnloadTilemap / reload cycle ----

    [Fact]
    public void GdRenderer_ReloadCycle_DrawsPixelsBothTimes()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);

        Tilemap firstMap = CreateOrthogonalMap(Color.White);
        Tilemap secondMap = CreateManyLayerTilemap(layerCount: 3, tileColor: Color.White);

        renderer.LoadTilemap(firstMap);
        var (pixels1, _) = RenderToPixels(() =>
            renderer.Draw(new OrthographicCamera(_graphicsDevice)));
        Assert.True(AnyPixelNotBlack(pixels1));

        renderer.UnloadTilemap();

        renderer.LoadTilemap(secondMap);
        var (pixels2, _) = RenderToPixels(() =>
            renderer.Draw(new OrthographicCamera(_graphicsDevice)));
        Assert.True(AnyPixelNotBlack(pixels2));
    }

    [Fact]
    public void SbRenderer_ReloadCycle_DrawsPixelsBothTimes()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();

        Tilemap firstMap = CreateOrthogonalMap(Color.White);
        Tilemap secondMap = CreateManyLayerTilemap(layerCount: 3, tileColor: Color.White);

        renderer.LoadTilemap(firstMap);
        var (pixels1, _) = RenderToPixels(() =>
            renderer.Draw(_spriteBatch, new OrthographicCamera(_graphicsDevice)));
        Assert.True(AnyPixelNotBlack(pixels1));

        renderer.UnloadTilemap();

        renderer.LoadTilemap(secondMap);
        var (pixels2, _) = RenderToPixels(() =>
            renderer.Draw(_spriteBatch, new OrthographicCamera(_graphicsDevice)));
        Assert.True(AnyPixelNotBlack(pixels2));
    }

    // ---- Parallax layers ----

    [Fact]
    public void SbRenderer_ParallaxLayers_DrawsPixels()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateParallaxTilemap(Color.White);
        renderer.LoadTilemap(tilemap);

        var (pixels, _) = RenderToPixels(() =>
        {
            renderer.Draw(_spriteBatch, new OrthographicCamera(_graphicsDevice));
        });

        Assert.True(AnyPixelNotBlack(pixels));
    }

    [Fact]
    // Added while investigating issue #1139:
    // https://github.com/MonoGame-Extended/Monogame-Extended/issues/1139
    public void SbRenderer_WideImageCollectionTile_RemainsVisibleAtLeftEdge()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        Tilemap tilemap = CreateSingleImageCollectionTileMap(
            mapWidth: 4,
            mapHeight: 4,
            tileX: 0,
            tileY: 0,
            imageWidth: TileSize * 2,
            imageHeight: TileSize,
            tileColor: Color.White);
        renderer.LoadTilemap(tilemap);

        var (pixels, width) = RenderToPixels(() =>
        {
            OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);
            camera.Position = new Vector2(TileSize + TileSize / 2f, 0f);
            renderer.Draw(_spriteBatch, camera);
        });

        Assert.Equal(Color.White, GetPixel(pixels, width, 8, TileCenterY));
    }

    [Fact]
    // Added while investigating issue #1139:
    // https://github.com/MonoGame-Extended/Monogame-Extended/issues/1139
    public void SbRenderer_TallImageCollectionTile_RemainsVisibleAtBottomEdge()
    {
        TilemapSpriteBatchRenderer renderer = new TilemapSpriteBatchRenderer();
        int viewportHeight = _graphicsDevice.Viewport.Height;
        int tileY = (int)Math.Ceiling(viewportHeight / (float)TileSize);

        Tilemap tilemap = CreateSingleImageCollectionTileMap(
            mapWidth: 4,
            mapHeight: tileY + 2,
            tileX: 0,
            tileY: tileY,
            imageWidth: TileSize,
            imageHeight: TileSize * 2,
            tileColor: Color.White);
        renderer.LoadTilemap(tilemap);

        var (pixels, width) = RenderToPixels(() =>
        {
            OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);
            renderer.Draw(_spriteBatch, camera);
        });

        int height = pixels.Length / width;
        Assert.Equal(Color.White, GetPixel(pixels, width, TileCenterX, height - 8));
    }

    // ---- Infrastructure ----

    private (Color[] Pixels, int Width) RenderToPixels(Action render)
    {
        int width = _graphicsDevice.Viewport.Width;
        int height = _graphicsDevice.Viewport.Height;

        using RenderTarget2D rt = new RenderTarget2D(_graphicsDevice, width, height);
        _graphicsDevice.SetRenderTarget(rt);
        _graphicsDevice.Clear(Color.Black);

        render();

        _graphicsDevice.SetRenderTarget(null);

        Color[] pixels = new Color[width * height];
        rt.GetData(pixels);
        return (pixels, width);
    }

    private static Color GetPixel(Color[] pixels, int width, int x, int y) =>
        pixels[y * width + x];

    private static bool AnyPixelNotBlack(Color[] pixels) =>
        Array.Exists(pixels, p => p.R > 0 || p.G > 0 || p.B > 0);

    private static bool AllPixelsBlack(Color[] pixels) =>
        Array.TrueForAll(pixels, p => p.R == 0 && p.G == 0 && p.B == 0);

    private static Color[] GetExpectedQuadrantColors(TilemapTileFlipFlags flags)
    {
        Color[] sourceColors =
        {
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Yellow
        };

        Color[] destinationColors = new Color[4];

        for (int sourceIndex = 0; sourceIndex < sourceColors.Length; sourceIndex++)
        {
            int x = sourceIndex % 2;
            int y = sourceIndex / 2;

            if ((flags & TilemapTileFlipFlags.FlipDiagonally) != 0)
            {
                (x, y) = (y, x);
            }

            if ((flags & TilemapTileFlipFlags.FlipHorizontally) != 0)
            {
                x = 1 - x;
            }

            if ((flags & TilemapTileFlipFlags.FlipVertically) != 0)
            {
                y = 1 - y;
            }

            destinationColors[y * 2 + x] = sourceColors[sourceIndex];
        }

        return destinationColors;
    }

    private static void AssertColorEquals(string label, Color expected, Color actual)
    {
        Assert.True(
            actual == expected,
            $"{label} expected {expected} but got {actual}");
    }

    private static void AssertTileQuadrantsMatchExpected(Color[] pixels, int width, TilemapTileFlipFlags flags)
    {
        Color[] expected = GetExpectedQuadrantColors(flags);
        int quarter = TileSize / 4;
        int threeQuarter = quarter * 3;

        AssertColorEquals("top-left", expected[0], GetPixel(pixels, width, quarter, quarter));
        AssertColorEquals("top-right", expected[1], GetPixel(pixels, width, threeQuarter, quarter));
        AssertColorEquals("bottom-left", expected[2], GetPixel(pixels, width, quarter, threeQuarter));
        AssertColorEquals("bottom-right", expected[3], GetPixel(pixels, width, threeQuarter, threeQuarter));
    }

    // ---- Tilemap factories ----

    // 10x10 tilemap with one tileset and all cells filled with a single tile.
    private Tilemap CreateOrthogonalMap(Color tileColor, float opacity = 1f)
    {
        Tilemap tilemap = new Tilemap(
            name: "OrthogonalMap",
            width: 10,
            height: 10,
            tileWidth: TileSize,
            tileHeight: TileSize,
            orientation: TilemapOrientation.Orthogonal);

        Texture2D texture = CreateFilledTexture(TileSize * 2, TileSize, tileColor);
        TilemapTileset tileset = new TilemapTileset(
            name: "Tileset",
            texture: texture,
            tileWidth: TileSize,
            tileHeight: TileSize,
            tileCount: 2,
            columns: 2,
            spacing: 0,
            margin: 0)
        {
            FirstGlobalId = 1
        };

        tilemap.Tilesets.Add(tileset);

        TilemapTileLayer layer = new TilemapTileLayer(
            name: "Ground",
            width: 10,
            height: 10,
            tileWidth: TileSize,
            tileHeight: TileSize);

        layer.Opacity = opacity;

        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                layer.SetTile(x, y, new TilemapTile(tileset.FirstGlobalId));
            }
        }

        tilemap.Layers.Add(layer);
        return tilemap;
    }

    // Tilemap with one tile at cell (0,0) using the specified flip flags.
    private Tilemap CreateSingleTileMap(Color tileColor, TilemapTileFlipFlags flags)
    {
        return CreateSingleTileMap(CreateFilledTexture(TileSize, TileSize, tileColor), flags);
    }

    private Tilemap CreateSingleTileMap(Texture2D texture, TilemapTileFlipFlags flags)
    {
        Tilemap tilemap = new Tilemap(
            name: "SingleTileMap",
            width: 5,
            height: 5,
            tileWidth: TileSize,
            tileHeight: TileSize,
            orientation: TilemapOrientation.Orthogonal);

        TilemapTileset tileset = new TilemapTileset(
            name: "Tileset",
            texture: texture,
            tileWidth: TileSize,
            tileHeight: TileSize,
            tileCount: 1,
            columns: 1,
            spacing: 0,
            margin: 0)
        {
            FirstGlobalId = 1
        };

        tilemap.Tilesets.Add(tileset);

        TilemapTileLayer layer = new TilemapTileLayer(
            name: "Layer",
            width: 5,
            height: 5,
            tileWidth: TileSize,
            tileHeight: TileSize);

        layer.SetTile(0, 0, new TilemapTile(tileset.FirstGlobalId, flags));
        tilemap.Layers.Add(layer);
        return tilemap;
    }

    private Tilemap CreateSingleImageCollectionTileMap(int mapWidth, int mapHeight, int tileX, int tileY, int imageWidth, int imageHeight, Color tileColor)
    {
        Tilemap tilemap = new Tilemap(
            name: "ImageCollectionMap",
            width: mapWidth,
            height: mapHeight,
            tileWidth: TileSize,
            tileHeight: TileSize,
            orientation: TilemapOrientation.Orthogonal);

        TilemapTileset tileset = new TilemapTileset(
            name: "ImageCollectionTileset",
            texture: null,
            tileWidth: imageWidth,
            tileHeight: imageHeight,
            tileCount: 1,
            columns: 0,
            spacing: 0,
            margin: 0);
        tileset.FirstGlobalId = 1;

        TilemapTileData tileData = new TilemapTileData(localId: 0)
        {
            CustomImage = CreateFilledTexture(imageWidth, imageHeight, tileColor)
        };
        tileset.AddTileData(tileData);
        tilemap.Tilesets.Add(tileset);

        TilemapTileLayer layer = new TilemapTileLayer(
            name: "Layer",
            width: mapWidth,
            height: mapHeight,
            tileWidth: TileSize,
            tileHeight: TileSize);

        layer.SetTile(tileX, tileY, new TilemapTile(tileset.FirstGlobalId));
        tilemap.Layers.Add(layer);
        return tilemap;
    }

    // Tilemap with N layers, each with one tile at cell (0,0).
    private Tilemap CreateManyLayerTilemap(int layerCount, Color tileColor)
    {
        Tilemap tilemap = new Tilemap(
            name: "ManyLayerMap",
            width: 8,
            height: 8,
            tileWidth: TileSize,
            tileHeight: TileSize,
            orientation: TilemapOrientation.Orthogonal);

        Texture2D texture = CreateFilledTexture(TileSize * 2, TileSize, tileColor);
        TilemapTileset tileset = new TilemapTileset(
            name: "Tileset",
            texture: texture,
            tileWidth: TileSize,
            tileHeight: TileSize,
            tileCount: 2,
            columns: 2,
            spacing: 0,
            margin: 0)
        {
            FirstGlobalId = 1
        };

        tilemap.Tilesets.Add(tileset);

        for (int i = 0; i < layerCount; i++)
        {
            TilemapTileLayer layer = new TilemapTileLayer(
                name: $"Layer{i}",
                width: 8,
                height: 8,
                tileWidth: TileSize,
                tileHeight: TileSize);

            layer.SetTile(0, 0, new TilemapTile(tileset.FirstGlobalId));
            tilemap.Layers.Add(layer);
        }

        return tilemap;
    }

    // Tilemap with 3 tile layers having different parallax factors.
    private Tilemap CreateParallaxTilemap(Color tileColor)
    {
        Tilemap tilemap = new Tilemap(
            name: "ParallaxMap",
            width: 10,
            height: 10,
            tileWidth: TileSize,
            tileHeight: TileSize,
            orientation: TilemapOrientation.Orthogonal);

        Texture2D texture = CreateFilledTexture(TileSize * 2, TileSize, tileColor);
        TilemapTileset tileset = new TilemapTileset(
            name: "Tileset",
            texture: texture,
            tileWidth: TileSize,
            tileHeight: TileSize,
            tileCount: 2,
            columns: 2,
            spacing: 0,
            margin: 0)
        {
            FirstGlobalId = 1
        };

        tilemap.Tilesets.Add(tileset);

        float[] factors = { 0.25f, 0.5f, 1.0f };
        foreach (float factor in factors)
        {
            TilemapTileLayer layer = new TilemapTileLayer(
                name: $"Layer_{factor}",
                width: 10,
                height: 10,
                tileWidth: TileSize,
                tileHeight: TileSize);

            layer.ParallaxFactor = new Vector2(factor, factor);
            layer.SetTile(0, 0, new TilemapTile(tileset.FirstGlobalId));
            tilemap.Layers.Add(layer);
        }

        return tilemap;
    }

    // Tilemap with a 2-frame animated tile: frame 0 = frame0Color, frame 1 = frame1Color.
    // Tile localId 0 occupies the left half of the texture; localId 1 the right half.
    private Tilemap CreateAnimatedTilemap(Color frame0Color, Color frame1Color, float frameDuration)
    {
        Tilemap tilemap = new Tilemap(
            name: "AnimatedMap",
            width: 5,
            height: 5,
            tileWidth: TileSize,
            tileHeight: TileSize,
            orientation: TilemapOrientation.Orthogonal);

        Texture2D texture = CreateTwoFrameTexture(frame0Color, frame1Color);
        TilemapTileset tileset = new TilemapTileset(
            name: "AnimatedTileset",
            texture: texture,
            tileWidth: TileSize,
            tileHeight: TileSize,
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
            new TilemapTileAnimationFrame(tileId: 0, duration: frameDuration),
            new TilemapTileAnimationFrame(tileId: 1, duration: frameDuration),
        });

        tileset.AddTileData(tileData);
        tilemap.Tilesets.Add(tileset);

        TilemapTileLayer layer = new TilemapTileLayer(
            name: "AnimatedLayer",
            width: 5,
            height: 5,
            tileWidth: TileSize,
            tileHeight: TileSize);

        layer.SetTile(0, 0, new TilemapTile(tileset.FirstGlobalId));
        tilemap.Layers.Add(layer);
        return tilemap;
    }

    // A solid-color texture filled entirely with the given color.
    private Texture2D CreateFilledTexture(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(_graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        Array.Fill(data, color);
        texture.SetData(data);
        return texture;
    }

    // The quadrant texture is:
    //
    //   top-left = red
    //   top-right = green
    //   bottom-left = blue
    //   bottom-right = yellow
    //
    // Using four distinct corners we can sample each qudrant after rendeirng and
    // tell whether the diagonal, horizontal, and vertical flips were applied in
    // the same order Tiled does.
    private Texture2D CreateQuadrantTexture()
    {
        Texture2D texture = new Texture2D(_graphicsDevice, TileSize, TileSize);
        Color[] data = new Color[TileSize * TileSize];

        for (int y = 0; y < TileSize; y++)
        {
            for (int x = 0; x < TileSize; x++)
            {
                Color color =
                    x < TileCenterX
                        ? y < TileCenterY ? Color.Red : Color.Blue
                        : y < TileCenterY ? Color.Green : Color.Yellow;

                data[y * TileSize + x] = color;
            }
        }

        texture.SetData(data);
        return texture;
    }

    // A 64x32 texture: left 32x32 = frame0Color, right 32x32 = frame1Color.
    private Texture2D CreateTwoFrameTexture(Color frame0Color, Color frame1Color)
    {
        Texture2D texture = new Texture2D(_graphicsDevice, TileSize * 2, TileSize);
        Color[] data = new Color[TileSize * 2 * TileSize];

        for (int y = 0; y < TileSize; y++)
        {
            for (int x = 0; x < TileSize; x++)
            {
                data[y * (TileSize * 2) + x] = frame0Color;  // left tile
                data[y * (TileSize * 2) + x + TileSize] = frame1Color; // right tile
            }
        }

        texture.SetData(data);
        return texture;
    }
}
