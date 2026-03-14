using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tests.Fixtures;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;

namespace MonoGame.Extended.Tests.Tilemaps.Rendering;

[Collection("GraphicsTest")]
public class TilemapRendererTests
{
    private readonly GraphicsDevice _graphicsDevice;

    public TilemapRendererTests(GraphicsTestFixture fixture)
    {
        _graphicsDevice = fixture.GraphicsDevice;
    }

    [Fact]
    public void Constructor_WithNullGraphicsDevice_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TilemapRenderer(null));
    }

    [Fact]
    public void LoadTilemap_WithNullTilemap_ThrowsArgumentNullException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);

        Assert.Throws<ArgumentNullException>(() => renderer.LoadTilemap(null));
    }

    [Fact]
    public void SetDefaultRenderMode_ChangesMode()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);

        renderer.SetDefaultRenderMode(RenderMode.Separate);

        Assert.Equal(RenderMode.Separate, renderer.DefaultRenderMode);
    }

    [Fact]
    public void BeginDraw_WithNullCamera_ThrowsArgumentNullException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);

        Assert.Throws<ArgumentNullException>(() => renderer.BeginDraw(null));
    }

    [Fact]
    public void BeginDraw_WithoutLoadedTilemap_ThrowsInvalidOperationException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<InvalidOperationException>(() => renderer.BeginDraw(camera));
    }

    [Fact]
    public void BeginDraw_CalledTwice_ThrowsInvalidOperationException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        renderer.BeginDraw(camera);

        Assert.Throws<InvalidOperationException>(() => renderer.BeginDraw(camera));

        renderer.EndDraw();
    }

    [Fact]
    public void EndDraw_WithoutBeginDraw_ThrowsInvalidOperationException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);

        Assert.Throws<InvalidOperationException>(() => renderer.EndDraw());
    }

    [Fact]
    public void DrawLayer_ByName_WithInvalidName_ThrowsKeyNotFoundException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        renderer.BeginDraw(camera);

        Assert.Throws<System.Collections.Generic.KeyNotFoundException>(() => renderer.DrawLayer("NonExistentLayer"));

        renderer.EndDraw();
    }

    [Fact]
    public void DrawLayer_ByIndex_WithInvalidIndex_ThrowsArgumentOutOfRangeException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        renderer.BeginDraw(camera);

        Assert.Throws<ArgumentOutOfRangeException>(() => renderer.DrawLayer(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => renderer.DrawLayer(999));

        renderer.EndDraw();
    }

    [Fact]
    public void DrawLayer_WithoutBeginDraw_ThrowsInvalidOperationException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);

        Assert.Throws<InvalidOperationException>(() => renderer.DrawLayer(0));
    }

    [Fact]
    public void Draw_WithNullCamera_ThrowsArgumentNullException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateSimpleTilemap();
        renderer.LoadTilemap(tilemap);

        Assert.Throws<ArgumentNullException>(() => renderer.Draw(null));
    }

    [Fact]
    public void Draw_WithoutLoadedTilemap_ThrowsInvalidOperationException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        Assert.Throws<InvalidOperationException>(() => renderer.Draw(camera));
    }

    [Fact]
    public void Update_WithNullGameTime_ThrowsArgumentNullException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);

        Assert.Throws<ArgumentNullException>(() => renderer.Update(null));
    }

    [Fact]
    public void HasLayerGroup_WithNonExistentGroup_ReturnsFalse()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);

        bool result = renderer.HasLayerGroup("NonExistentGroup");

        Assert.False(result);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);

        renderer.Dispose();
        renderer.Dispose();
    }

    [Fact]
    public void Dispose_PreventsSubsequentOperations()
    {
        TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        renderer.Dispose();

        Assert.Throws<ObjectDisposedException>(() => renderer.DefaultRenderMode);
    }

    #region DefineLayerGroup (by Name)

    [Fact]
    public void DefineLayerGroup_WithNullGroupName_ThrowsArgumentNullException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);

        Assert.Throws<ArgumentNullException>(() => renderer.DefineLayerGroup(null, "Layer1", "Layer2"));
    }

    [Fact]
    public void DefineLayerGroup_WithNullLayerNames_ThrowsArgumentNullException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);

        Assert.Throws<ArgumentNullException>(() => renderer.DefineLayerGroup("Group", (string[])null));
    }

    [Fact]
    public void DefineLayerGroup_WithInvalidLayerName_ThrowsKeyNotFoundException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);

        Assert.Throws<System.Collections.Generic.KeyNotFoundException>(
            () => renderer.DefineLayerGroup("Group", "DoesNotExist"));
    }

    [Fact]
    public void DefineLayerGroup_WithValidNames_CreatesGroup()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);

        renderer.DefineLayerGroup("Background", "Layer1", "Layer2");

        Assert.True(renderer.HasLayerGroup("Background"));
        Assert.Contains("Background", renderer.LayerGroups);
    }

    [Fact]
    public void DefineLayerGroup_WithSameName_RedefinesGroup()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);

        renderer.DefineLayerGroup("Group", "Layer1", "Layer2");
        renderer.DefineLayerGroup("Group", "Layer3", "Layer4");

        Assert.True(renderer.HasLayerGroup("Group"));
        Assert.Single(renderer.LayerGroups);
    }

    [Fact]
    public void DefineLayerGroup_LayerMovedFromExistingGroup_EnforcesExclusiveMembership()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);

        renderer.DefineLayerGroup("GroupA", "Layer1", "Layer2");
        renderer.DefineLayerGroup("GroupB", "Layer2", "Layer3");

        // Layer2 was moved to GroupB; both groups must still exist.
        Assert.True(renderer.HasLayerGroup("GroupA"));
        Assert.True(renderer.HasLayerGroup("GroupB"));
    }

    [Fact]
    public void DefineLayerGroup_WithoutLoadedTilemap_ThrowsInvalidOperationException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);

        Assert.Throws<InvalidOperationException>(() => renderer.DefineLayerGroup("Group", "Layer1"));
    }

    #endregion

    #region DefineLayerGroup (by Index)

    [Fact]
    public void DefineLayerGroup_ByIndex_WithValidRange_CreatesGroup()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);

        renderer.DefineLayerGroup("Background", startIndex: 0, count: 2);

        Assert.True(renderer.HasLayerGroup("Background"));
    }

    [Fact]
    public void DefineLayerGroup_ByIndex_WithNegativeStart_ThrowsArgumentOutOfRangeException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => renderer.DefineLayerGroup("Group", startIndex: -1, count: 2));
    }

    [Fact]
    public void DefineLayerGroup_ByIndex_WithCountExceedingBounds_ThrowsArgumentOutOfRangeException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => renderer.DefineLayerGroup("Group", startIndex: 3, count: 99));
    }

    #endregion

    #region RemoveLayerGroup

    [Fact]
    public void RemoveLayerGroup_ExistingGroup_RemovesGroup()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);
        renderer.DefineLayerGroup("Background", "Layer1", "Layer2");

        renderer.RemoveLayerGroup("Background");

        Assert.False(renderer.HasLayerGroup("Background"));
    }

    [Fact]
    public void RemoveLayerGroup_NonExistentGroup_DoesNotThrow()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);

        renderer.RemoveLayerGroup("DoesNotExist");
    }

    #endregion

    #region MarkGroupDirty

    [Fact]
    public void MarkGroupDirty_NonExistentGroup_ThrowsArgumentException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);

        Assert.Throws<ArgumentException>(() => renderer.MarkGroupDirty("DoesNotExist"));
    }

    #endregion

    #region DrawLayerGroup

    [Fact]
    public void DrawLayerGroup_WithoutBeginDraw_ThrowsInvalidOperationException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);
        renderer.DefineLayerGroup("Background", "Layer1", "Layer2");

        Assert.Throws<InvalidOperationException>(() => renderer.DrawLayerGroup("Background"));
    }

    [Fact]
    public void DrawLayerGroup_WithNonExistentGroup_ThrowsArgumentException()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        renderer.BeginDraw(camera);

        Assert.Throws<ArgumentException>(() => renderer.DrawLayerGroup("DoesNotExist"));

        renderer.EndDraw();
    }

    #endregion

    #region RenderMode

    [Fact]
    public void Draw_InMergedMode_AfterRemoveLayerGroup_RebuildsMergedBuffer()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);
        renderer.SetDefaultRenderMode(RenderMode.Merged);
        renderer.DefineLayerGroup("Background", "Layer1", "Layer2");
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        renderer.Draw(camera);

        renderer.RemoveLayerGroup("Background");

        // After removing the group, Layer1 and Layer2 return to ungrouped.
        // A second draw must rebuild the merged buffer without throwing.
        renderer.Draw(camera);
    }

    [Fact]
    public void SwitchingRenderMode_BetweenMergedAndSeparate_Succeeds()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        renderer.SetDefaultRenderMode(RenderMode.Separate);
        renderer.Draw(camera);

        renderer.SetDefaultRenderMode(RenderMode.Merged);
        renderer.Draw(camera);

        renderer.SetDefaultRenderMode(RenderMode.Separate);
        renderer.Draw(camera);
    }

    [Fact]
    public void Draw_InMergedMode_WithAllLayersGrouped_Succeeds()
    {
        using TilemapRenderer renderer = new TilemapRenderer(_graphicsDevice);
        Tilemap tilemap = CreateMultiLayerTilemap();
        renderer.LoadTilemap(tilemap);
        renderer.SetDefaultRenderMode(RenderMode.Merged);
        renderer.DefineLayerGroup("All", "Layer1", "Layer2", "Layer3", "Layer4");
        OrthographicCamera camera = new OrthographicCamera(_graphicsDevice);

        // All layers are grouped; the merged ungrouped buffer will be empty.
        renderer.Draw(camera);
    }

    #endregion

    #region Helper Methods

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
            margin: 0);

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
            margin: 0);

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

    #endregion
}
