using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tests.Fixtures;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Parsers;
using MonoGame.Extended.Tilemaps.Tiled;

namespace MonoGame.Extended.Tests.Tilemaps;

[Collection("GraphicsTest")]
public class TiledTmxParserTests
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly TiledTmxParser _parser;

    public TiledTmxParserTests(GraphicsTestFixture fixture)
    {
        _graphicsDevice = fixture.GraphicsDevice;
        _parser = new TiledTmxParser();
    }

    [Fact]
    public void Parser_SupportedExtensions_ContainsTmx()
    {
        Assert.Contains(".tmx", _parser.SupportedExtensions);
    }

    [Fact]
    public void ParseFromFile_WithNullPath_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _parser.ParseFromFile(null, _graphicsDevice));
    }

    [Fact]
    public void ParseFromFile_WithNullGraphicsDevice_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _parser.ParseFromFile("test.tmx", null));
    }

    [Fact]
    public void ParseFromFile_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() =>
            _parser.ParseFromFile("nonexistent.tmx", _graphicsDevice));
    }

    [Fact]
    public void ParseFromStream_WithNullStream_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _parser.ParseFromStream(null, _graphicsDevice, null));
    }

    [Fact]
    public void ParseFromStream_WithNullGraphicsDevice_ThrowsArgumentNullException()
    {
        using MemoryStream stream = new MemoryStream();
        Assert.Throws<ArgumentNullException>(() =>
            _parser.ParseFromStream(stream, null, null));
    }

    [Fact]
    public void ParseFromStream_WithInvalidXml_ThrowsTilemapParseException()
    {
        using MemoryStream stream = new MemoryStream();
        using StreamWriter writer = new StreamWriter(stream);
        writer.Write("invalid xml");
        writer.Flush();
        stream.Position = 0;

        Assert.Throws<TilemapParseException>(() =>
            _parser.ParseFromStream(stream, _graphicsDevice, null));
    }

    [Fact]
    public void Parse_SimpleCsvMap_CreatesValidTilemap()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-tileset-csv.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);

        Assert.NotNull(tilemap);
        Assert.Equal(3, tilemap.Width);
        Assert.Equal(3, tilemap.Height);
        Assert.Equal(32, tilemap.TileWidth);
        Assert.Equal(32, tilemap.TileHeight);
        Assert.Equal(TilemapOrientation.Orthogonal, tilemap.Orientation);
    }

    [Fact]
    public void Parse_SimpleCsvMap_LoadsTileset()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-tileset-csv.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);

        Assert.NotNull(tilemap.Tilesets);
        Assert.Single(tilemap.Tilesets);

        TilemapTileset tileset = tilemap.Tilesets[0];
        Assert.Equal("test-tileset", tileset.Name);
        Assert.Equal(1, tileset.FirstGlobalId);
        Assert.Equal(32, tileset.TileWidth);
        Assert.Equal(32, tileset.TileHeight);
        Assert.Equal(2, tileset.Spacing);
        Assert.Equal(2, tileset.Margin);
    }

    [Fact]
    public void Parse_SimpleCsvMap_LoadsTileLayer()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-tileset-csv.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);

        Assert.NotNull(tilemap.Layers);
        Assert.Single(tilemap.Layers);

        TilemapLayer layer = tilemap.Layers[0];
        Assert.IsType<TilemapTileLayer>(layer);
        Assert.Equal("Tile Layer 1", layer.Name);

        TilemapTileLayer tileLayer = (TilemapTileLayer)layer;
        Assert.Equal(3, tileLayer.Width);
        Assert.Equal(3, tileLayer.Height);
    }

    [Fact]
    public void Parse_SimpleCsvMap_LoadsTileData()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-tileset-csv.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);
        TilemapTileLayer tileLayer = (TilemapTileLayer)tilemap.Layers[0];

        TilemapTile? tile00 = tileLayer.GetTile(0, 0);
        Assert.NotNull(tile00);
        Assert.Equal(1, tile00.Value.GlobalId);

        TilemapTile? tile10 = tileLayer.GetTile(1, 0);
        Assert.NotNull(tile10);
        Assert.Equal(2, tile10.Value.GlobalId);

        TilemapTile? tile20 = tileLayer.GetTile(2, 0);
        Assert.NotNull(tile20);
        Assert.Equal(3, tile20.Value.GlobalId);
    }

    [Fact]
    public void Parse_ObjectLayerMap_LoadsObjects()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-object-layer.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);

        Assert.NotNull(tilemap.Layers);
        Assert.Single(tilemap.Layers);

        TilemapLayer layer = tilemap.Layers[0];
        Assert.IsType<TilemapObjectLayer>(layer);

        TilemapObjectLayer objectLayer = (TilemapObjectLayer)layer;
        Assert.Equal("Object Layer 1", objectLayer.Name);
        Assert.Equal(6, objectLayer.Objects.Count);
    }

    [Fact]
    public void Parse_ObjectLayerMap_LoadsEllipseObject()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-object-layer.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);
        TilemapObjectLayer objectLayer = (TilemapObjectLayer)tilemap.Layers[0];

        TilemapObject ellipse = objectLayer.Objects[0];
        Assert.IsType<TilemapEllipseObject>(ellipse);
        Assert.Equal(1, ellipse.Id);
        Assert.Equal(new Vector2(131.345f, 65.234f), ellipse.Position);
        Assert.Equal(new Vector2(311.111f, 311.232f), ((TilemapEllipseObject)ellipse).Size);
    }

    [Fact]
    public void Parse_ObjectLayerMap_LoadsPolygonObject()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-object-layer.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);
        TilemapObjectLayer objectLayer = (TilemapObjectLayer)tilemap.Layers[0];

        TilemapObject polygon = objectLayer.Objects[3];
        Assert.IsType<TilemapPolygonObject>(polygon);
        Assert.Equal("polygon", polygon.Name);

        TilemapPolygonObject polygonObj = (TilemapPolygonObject)polygon;
        Assert.Equal(5, polygonObj.Points.Length);
        Assert.Equal(new Vector2(0, 0), polygonObj.Points[0]);
        Assert.Equal(new Vector2(180, 90), polygonObj.Points[1]);
    }

    [Fact]
    public void Parse_ObjectLayerMap_LoadsPolylineObject()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-object-layer.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);
        TilemapObjectLayer objectLayer = (TilemapObjectLayer)tilemap.Layers[0];

        TilemapObject polyline = objectLayer.Objects[4];
        Assert.IsType<TilemapPolylineObject>(polyline);

        TilemapPolylineObject polylineObj = (TilemapPolylineObject)polyline;
        Assert.Equal(4, polylineObj.Points.Length);
        Assert.Equal(new Vector2(0, 0), polylineObj.Points[0]);
    }

    [Fact]
    public void Parse_ObjectLayerMap_LoadsObjectProperties()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-object-layer.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);
        TilemapObjectLayer objectLayer = (TilemapObjectLayer)tilemap.Layers[0];

        TilemapObject ellipse = objectLayer.Objects[0];
        Assert.NotNull(ellipse.Properties);
        Assert.True(ellipse.Properties.TryGetValue("shape", out TilemapPropertyValue shapeValue));
        Assert.Equal("circle", shapeValue.AsString());
    }

    [Fact]
    public void Parse_IsometricMap_SetsCorrectOrientation()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "isometric.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);

        Assert.Equal(TilemapOrientation.Isometric, tilemap.Orientation);
    }

    [Fact]
    public void Parse_ImageLayerMap_LoadsImageLayers()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-image-layer.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);

        Assert.Equal(3, tilemap.Layers.Count);
        Assert.IsType<TilemapImageLayer>(tilemap.Layers[0]);
        Assert.IsType<TilemapTileLayer>(tilemap.Layers[1]);
        Assert.IsType<TilemapImageLayer>(tilemap.Layers[2]);
    }

    [Fact]
    public void Parse_ImageLayerMap_ImageLayerHasCorrectName()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-image-layer.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);

        Assert.Equal("Background", tilemap.Layers[0].Name);
        Assert.Equal("Overlay", tilemap.Layers[2].Name);
    }

    [Fact]
    public void Parse_ImageLayerMap_ImageLayerRepeatFlagsAreSet()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-image-layer.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);

        TilemapImageLayer background = (TilemapImageLayer)tilemap.Layers[0];
        Assert.True(background.RepeatX);
        Assert.False(background.RepeatY);

        TilemapImageLayer overlay = (TilemapImageLayer)tilemap.Layers[2];
        Assert.False(overlay.RepeatX);
        Assert.False(overlay.RepeatY);
    }

    [Fact]
    public void Parse_ImageLayerMap_ImageLayerHasTexture()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-image-layer.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);

        TilemapImageLayer background = (TilemapImageLayer)tilemap.Layers[0];
        Assert.NotNull(background.Texture);
    }

    [Fact]
    public void Parse_GroupLayerMap_FlattensLayersIntoTopLevel()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-group-layer.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);

        Assert.Equal(4, tilemap.Layers.Count);
    }

    [Fact]
    public void Parse_GroupLayerMap_NoGroupLayerInstancesInCollection()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-group-layer.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);

        foreach (TilemapLayer layer in tilemap.Layers)
            Assert.IsNotType<TilemapGroupLayer>(layer);
    }

    [Fact]
    public void Parse_GroupLayerMap_LayersHaveFullDepthPathNames()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-group-layer.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);

        Assert.Equal("World/Background/Sky", tilemap.Layers[0].Name);
        Assert.Equal("World/Background/Mountains", tilemap.Layers[1].Name);
        Assert.Equal("World/Ground", tilemap.Layers[2].Name);
        Assert.Equal("HUD", tilemap.Layers[3].Name);
    }

    [Fact]
    public void Parse_GroupLayerMap_FlattenedLayersAreAccessibleByPathName()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-group-layer.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);

        Assert.NotNull(tilemap.Layers.GetLayer<TilemapTileLayer>("World/Background/Sky"));
        Assert.NotNull(tilemap.Layers.GetLayer<TilemapTileLayer>("World/Background/Mountains"));
        Assert.NotNull(tilemap.Layers.GetLayer<TilemapTileLayer>("World/Ground"));
        Assert.NotNull(tilemap.Layers.GetLayer<TilemapTileLayer>("HUD"));
    }

    [Fact]
    public void Parse_GroupLayerMap_TopLevelLayerRetainsOriginalName()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-group-layer.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);

        TilemapLayer hud = tilemap.Layers.GetLayer<TilemapTileLayer>("HUD");
        Assert.NotNull(hud);
        Assert.Equal("HUD", hud.Name);
    }

    // ---- Infinite map (chunks flattened to TilemapTileLayer) ----

    [Fact]
    public void Parse_InfiniteMap_LayerIsFlatTileLayer()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-infinite.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);

        Assert.Single(tilemap.Layers);
        Assert.IsType<TilemapTileLayer>(tilemap.Layers[0]);
    }

    [Fact]
    public void Parse_InfiniteMap_LayerDimensionsCoverAllChunks()
    {
        // Chunks at (-16,-16), (0,0), (16,16), each 16x16 tiles.
        // Bounding box: minX=-16, minY=-16, maxX=32, maxY=32 -> 48x48 tiles.
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-infinite.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);
        TilemapTileLayer layer = (TilemapTileLayer)tilemap.Layers[0];

        Assert.Equal(48, layer.Width);
        Assert.Equal(48, layer.Height);
    }

    [Fact]
    public void Parse_InfiniteMap_NegativeChunkTilePreserved()
    {
        // The tile at world tile (-16,-16) was in the chunk at origin (-16,-16).
        // After flattening it lives at array index (0,0) with the layer offset
        // shifting rendering to the correct world position.
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-infinite.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);
        TilemapTileLayer layer = (TilemapTileLayer)tilemap.Layers[0];

        TilemapTile? tile = layer.GetTile(0, 0);
        Assert.NotNull(tile);
        Assert.Equal(5, tile.Value.GlobalId);
    }

    [Fact]
    public void Parse_InfiniteMap_LayerOffsetPositionsAtChunkOrigin()
    {
        // tileWidth=32, minTileX=-16 => offset.X = -16 * 32 = -512.
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-infinite.tmx");

        Tilemap tilemap = _parser.ParseFromFile(tmxPath, _graphicsDevice);
        TilemapTileLayer layer = (TilemapTileLayer)tilemap.Layers[0];

        Assert.Equal(-512f, layer.Offset.X);
        Assert.Equal(-512f, layer.Offset.Y);
    }

    // ---- Unsupported compression ----

    [Fact]
    public void Parse_ZstdCompressedLayer_ThrowsTilemapParseException()
    {
        string testDataPath = GetTestDataPath();
        string tmxPath = Path.Combine(testDataPath, "test-tileset-zstd.tmx");

        TilemapParseException ex = Assert.Throws<TilemapParseException>(() =>
            _parser.ParseFromFile(tmxPath, _graphicsDevice));

        Assert.Contains("Zstandard", ex.Message);
        Assert.Contains("zlib", ex.Message);
    }

    // ---- Exception quality ----

    [Fact]
    public void Parse_MissingExternalTsx_ExceptionNamesTilesetSource()
    {
        string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<map version=""1.0"" orientation=""orthogonal"" renderorder=""right-down""
     width=""1"" height=""1"" tilewidth=""32"" tileheight=""32"">
 <tileset firstgid=""1"" source=""missing-tileset.tsx""/>
 <layer name=""Layer 1"" width=""1"" height=""1"">
  <data encoding=""csv"">1</data>
 </layer>
</map>";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(xml);
        using MemoryStream stream = new MemoryStream(bytes);

        TilemapParseException ex = Assert.Throws<TilemapParseException>(() =>
            _parser.ParseFromStream(stream, _graphicsDevice, "/nonexistent/basepath"));

        Assert.Contains("missing-tileset.tsx", ex.Message);
        Assert.Contains("firstgid=1", ex.Message);
    }

    [Fact]
    public void Parse_MissingTilesetTexture_ExceptionNamesTileset()
    {
        string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<map version=""1.0"" orientation=""orthogonal"" renderorder=""right-down""
     width=""1"" height=""1"" tilewidth=""32"" tileheight=""32"">
 <tileset firstgid=""1"" name=""TerrainTiles"" tilewidth=""32"" tileheight=""32"">
  <image source=""terrain.png"" width=""64"" height=""64""/>
 </tileset>
 <layer name=""Layer 1"" width=""1"" height=""1"">
  <data encoding=""csv"">1</data>
 </layer>
</map>";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(xml);
        using MemoryStream stream = new MemoryStream(bytes);

        TilemapParseException ex = Assert.Throws<TilemapParseException>(() =>
            _parser.ParseFromStream(stream, _graphicsDevice, "/nonexistent/basepath"));

        Assert.Contains("TerrainTiles", ex.Message);
        Assert.Contains("terrain.png", ex.Message);
    }

    [Fact]
    public void Parse_MissingImageLayerTexture_ExceptionNamesLayer()
    {
        string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<map version=""1.0"" orientation=""orthogonal"" renderorder=""right-down""
     width=""1"" height=""1"" tilewidth=""32"" tileheight=""32"">
 <imagelayer name=""BackgroundSky"">
  <image source=""sky.png"" width=""256"" height=""256""/>
 </imagelayer>
</map>";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(xml);
        using MemoryStream stream = new MemoryStream(bytes);

        TilemapParseException ex = Assert.Throws<TilemapParseException>(() =>
            _parser.ParseFromStream(stream, _graphicsDevice, "/nonexistent/basepath"));

        Assert.Contains("BackgroundSky", ex.Message);
        Assert.Contains("sky.png", ex.Message);
    }

    [Fact]
    public void ParseFromStream_WithExternalResourceResolver_LoadsExternalTileset()
    {
        string testDataPath = GetTestDataPath();
        byte[] textureBytes = File.ReadAllBytes(Path.Combine(testDataPath, "test-tileset.png"));

        string tmx = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<map version=""1.0"" orientation=""orthogonal"" renderorder=""right-down""
     width=""1"" height=""1"" tilewidth=""32"" tileheight=""32"">
 <tileset firstgid=""1"" source=""external.tsx""/>
 <layer name=""Layer 1"" width=""1"" height=""1"">
  <data encoding=""csv"">1</data>
 </layer>
</map>";

        string tsx = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<tileset version=""1.0"" name=""ExternalTiles"" tilewidth=""32"" tileheight=""32"" tilecount=""9"" columns=""3"">
 <image source=""test-tileset.png"" width=""104"" height=""104""/>
</tileset>";

        Dictionary<string, byte[]> resources = new Dictionary<string, byte[]>
        {
            ["external.tsx"] = Encoding.UTF8.GetBytes(tsx),
            ["test-tileset.png"] = textureBytes
        };

        TiledTmxParser parser = new TiledTmxParser(resourceResolver: path => OpenResourceByFileName(resources, path));
        using MemoryStream stream = CreateXmlStream(tmx);

        Tilemap tilemap = parser.ParseFromStream(stream, _graphicsDevice, "virtual");

        TilemapTileset tileset = Assert.Single(tilemap.Tilesets);
        Assert.Equal("ExternalTiles", tileset.Name);
        Assert.NotNull(tileset.Texture);
    }

    [Fact]
    public void ParseFromStream_WithExternalResourceResolver_LoadsImageLayerTexture()
    {
        string testDataPath = GetTestDataPath();
        byte[] textureBytes = File.ReadAllBytes(Path.Combine(testDataPath, "test-tileset.png"));

        string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<map version=""1.0"" orientation=""orthogonal"" renderorder=""right-down""
     width=""1"" height=""1"" tilewidth=""32"" tileheight=""32"">
 <imagelayer name=""Background"">
  <image source=""background.png"" width=""104"" height=""104""/>
 </imagelayer>
</map>";

        Dictionary<string, byte[]> resources = new Dictionary<string, byte[]>
        {
            ["background.png"] = textureBytes
        };

        TiledTmxParser parser = new TiledTmxParser(resourceResolver: path => OpenResourceByFileName(resources, path));
        using MemoryStream stream = CreateXmlStream(xml);

        Tilemap tilemap = parser.ParseFromStream(stream, _graphicsDevice, "virtual");

        TilemapImageLayer layer = Assert.IsType<TilemapImageLayer>(Assert.Single(tilemap.Layers));
        Assert.Equal("Background", layer.Name);
        Assert.NotNull(layer.Texture);
    }

    // Added while investigating issue #1138:
    // https://github.com/MonoGame-Extended/Monogame-Extended/issues/1138
    [Fact]
    public void Parse_TmxWithDiagonalAndVerticalFlips_PreservesFlipFlags()
    {
        string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<map version=""1.10"" tiledversion=""1.11.0"" orientation=""orthogonal"" renderorder=""right-down""
     width=""2"" height=""2"" tilewidth=""16"" tileheight=""16"" infinite=""0"" nextlayerid=""8"" nextobjectid=""8"">
 <tileset firstgid=""1"" name=""TiledIcons"" tilewidth=""16"" tileheight=""16"" tilecount=""1024"" columns=""32""/>
 <tileset firstgid=""1025"" name=""sheet"" tilewidth=""16"" tileheight=""16"" tilecount=""136"" columns=""17""/>
 <layer id=""7"" name=""Tree"" width=""2"" height=""2"">
  <data encoding=""csv"">
1610613767,1610613784,
1610613766,1610613783
</data>
 </layer>
</map>";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(xml);
        using MemoryStream stream = new MemoryStream(bytes);

        Tilemap tilemap = _parser.ParseFromStream(stream, _graphicsDevice);
        TilemapTileLayer layer = Assert.IsType<TilemapTileLayer>(Assert.Single(tilemap.Layers));

        TilemapTile? topLeft = layer.GetTile(0, 0);
        TilemapTile? topRight = layer.GetTile(1, 0);
        TilemapTile? bottomLeft = layer.GetTile(0, 1);
        TilemapTile? bottomRight = layer.GetTile(1, 1);

        Assert.NotNull(topLeft);
        Assert.NotNull(topRight);
        Assert.NotNull(bottomLeft);
        Assert.NotNull(bottomRight);

        TilemapTileFlipFlags expectedFlags = TilemapTileFlipFlags.FlipVertically | TilemapTileFlipFlags.FlipDiagonally;

        Assert.Equal(1031, topLeft.Value.GlobalId);
        Assert.Equal(expectedFlags, topLeft.Value.FlipFlags);

        Assert.Equal(1048, topRight.Value.GlobalId);
        Assert.Equal(expectedFlags, topRight.Value.FlipFlags);

        Assert.Equal(1030, bottomLeft.Value.GlobalId);
        Assert.Equal(expectedFlags, bottomLeft.Value.FlipFlags);

        Assert.Equal(1047, bottomRight.Value.GlobalId);
        Assert.Equal(expectedFlags, bottomRight.Value.FlipFlags);
    }

    private string GetTestDataPath()
    {
        // The test data is in the MonoGame.Extended.Content.Pipeline.Tests project
        // We need to navigate from the current test assembly location to find it
        string currentDir = Directory.GetCurrentDirectory();

        // Try to find the solution root by looking for the .sln file
        DirectoryInfo searchDir = new DirectoryInfo(currentDir);
        while (searchDir != null && !File.Exists(Path.Combine(searchDir.FullName, "MonoGame.Extended.sln")))
        {
            searchDir = searchDir.Parent;
        }

        if (searchDir == null)
        {
            throw new DirectoryNotFoundException("Could not find solution root directory");
        }

        string testDataPath = Path.Combine(searchDir.FullName, "tests",
            "MonoGame.Extended.Content.Pipeline.Tests", "TestData");

        if (!Directory.Exists(testDataPath))
        {
            throw new DirectoryNotFoundException($"Test data directory not found: {testDataPath}");
        }

        return testDataPath;
    }

    private static MemoryStream CreateXmlStream(string xml)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(xml));
    }

    private static Stream OpenResourceByFileName(Dictionary<string, byte[]> resources, string path)
    {
        string fileName = Path.GetFileName(path);

        if (!resources.TryGetValue(fileName, out byte[] bytes))
        {
            throw new FileNotFoundException($"Resource not found: {path}", path);
        }

        return new MemoryStream(bytes, writable: false);
    }
}
