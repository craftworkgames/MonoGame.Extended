using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tests.Fixtures;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Ogmo;
using MonoGame.Extended.Tilemaps.Parsers;

namespace MonoGame.Extended.Tests.Tilemaps;

[Collection("GraphicsTest")]
public class OgmoJsonParserTests
{
    private readonly GraphicsDevice _graphicsDevice;

    public OgmoJsonParserTests(GraphicsTestFixture fixture)
    {
        _graphicsDevice = fixture.GraphicsDevice;
    }

    [Fact]
    public void Parser_SupportedExtensions_ContainsJson()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);

        Assert.Contains(".json", parser.SupportedExtensions);
    }

    [Fact]
    public void CanParse_WithInvalidExtension_ReturnsFalse()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);

        bool result = parser.CanParse("test.txt");

        Assert.False(result);
    }

    [Fact]
    public void CanParse_WithNullPath_ReturnsFalse()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);

        bool result = parser.CanParse(null);

        Assert.False(result);
    }

    [Fact]
    public void ParseFromFile_WithNullPath_ThrowsArgumentNullException()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);

        Assert.Throws<ArgumentNullException>(() =>
            parser.ParseFromFile(null, _graphicsDevice));
    }

    [Fact]
    public void ParseFromFile_WithNullGraphicsDevice_ThrowsArgumentNullException()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);

        Assert.Throws<ArgumentNullException>(() =>
            parser.ParseFromFile("test.json", null));
    }

    [Fact]
    public void ParseFromFile_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);

        Assert.Throws<FileNotFoundException>(() =>
            parser.ParseFromFile("nonexistent.json", _graphicsDevice));
    }

    [Fact]
    public void ParseFromStream_WithNullStream_ThrowsArgumentNullException()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);

        Assert.Throws<ArgumentNullException>(() =>
            parser.ParseFromStream(null, _graphicsDevice));
    }

    [Fact]
    public void ParseFromStream_WithNullGraphicsDevice_ThrowsArgumentNullException()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);
        using MemoryStream stream = new MemoryStream();

        Assert.Throws<ArgumentNullException>(() =>
            parser.ParseFromStream(stream, null));
    }

    [Fact]
    public void ParseFromStream_WithInvalidJson_ThrowsTilemapParseException()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);
        byte[] invalidJson = System.Text.Encoding.UTF8.GetBytes("{ invalid json }");
        using MemoryStream stream = new MemoryStream(invalidJson);

        Assert.Throws<TilemapParseException>(() =>
            parser.ParseFromStream(stream, _graphicsDevice));
    }

    [Fact]
    public void ParseFromStream_WithMinimalLevel_CreatesValidTilemap()
    {
        string minimalLevel = @"{
            ""ogmoVersion"": ""3.4.0"",
            ""width"": 320,
            ""height"": 240,
            ""offsetX"": 0,
            ""offsetY"": 0,
            ""layers"": []
        }";
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(minimalLevel);
        using MemoryStream stream = new MemoryStream(jsonBytes);

        // Create a minimal project file in temp directory
        string tempDir = Path.Combine(Path.GetTempPath(), "OgmoTest_" + Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        try
        {
            string projectPath = Path.Combine(tempDir, "test.ogmo");
            File.WriteAllText(projectPath, @"{
                ""ogmoVersion"": ""3.4.0"",
                ""name"": ""TestProject"",
                ""levelPaths"": ["".""],
                ""backgroundColor"": ""#282c34ff"",
                ""gridColor"": ""#3c4049cc"",
                ""anglesRadians"": true,
                ""directoryDepth"": 5,
                ""layerGridDefaultSize"": {""x"": 16, ""y"": 16},
                ""levelDefaultSize"": {""x"": 320, ""y"": 240},
                ""levelMinSize"": {""x"": 128, ""y"": 128},
                ""levelMaxSize"": {""x"": 4096, ""y"": 4096},
                ""layers"": [],
                ""entities"": [],
                ""tilesets"": []
            }");

            OgmoJsonParser parser = new OgmoJsonParser(projectPath);
            Tilemap tilemap = parser.ParseFromStream(stream, _graphicsDevice);

            Assert.NotNull(tilemap);
            Assert.Equal(20, tilemap.Width);
            Assert.Equal(15, tilemap.Height);
            Assert.Equal(16, tilemap.TileWidth);
            Assert.Equal(16, tilemap.TileHeight);
            Assert.Equal(TilemapOrientation.Orthogonal, tilemap.Orientation);
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ParseFromStream_WithTileLayer_CreatesLayer()
    {
        string levelWithLayer = @"{
            ""ogmoVersion"": ""3.4.0"",
            ""width"": 320,
            ""height"": 240,
            ""offsetX"": 0,
            ""offsetY"": 0,
            ""layers"": [
                {
                    ""name"": ""Ground"",
                    ""_eid"": ""1234"",
                    ""offsetX"": 0,
                    ""offsetY"": 0,
                    ""gridCellWidth"": 16,
                    ""gridCellHeight"": 16,
                    ""gridCellsX"": 20,
                    ""gridCellsY"": 15,
                    ""tileset"": ""Tileset"",
                    ""data"": [-1, -1, -1],
                    ""arrayMode"": 0
                }
            ]
        }";
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(levelWithLayer);
        using MemoryStream stream = new MemoryStream(jsonBytes);

        string tempDir = Path.Combine(Path.GetTempPath(), "OgmoTest_" + Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        try
        {
            string projectPath = Path.Combine(tempDir, "test.ogmo");
            File.WriteAllText(projectPath, @"{
                ""ogmoVersion"": ""3.4.0"",
                ""name"": ""TestProject"",
                ""backgroundColor"": ""#282c34ff"",
                ""layerGridDefaultSize"": {""x"": 16, ""y"": 16},
                ""layers"": [
                    {
                        ""definition"": ""tile"",
                        ""name"": ""Ground"",
                        ""gridSize"": {""x"": 16, ""y"": 16},
                        ""exportID"": ""12345678"",
                        ""arrayMode"": 0
                    }
                ],
                ""tilesets"": []
            }");

            OgmoJsonParser parser = new OgmoJsonParser(projectPath);
            Tilemap tilemap = parser.ParseFromStream(stream, _graphicsDevice);

            Assert.NotNull(tilemap);
            Assert.Single(tilemap.Layers);

            TilemapLayer layer = tilemap.Layers[0];
            Assert.Equal("Ground", layer.Name);
            Assert.True(layer.IsVisible);
            Assert.Equal(1.0f, layer.Opacity);
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ParseFromStream_StoresOgmoSpecificProperties()
    {
        string level = @"{
            ""ogmoVersion"": ""3.4.0"",
            ""width"": 640,
            ""height"": 480,
            ""offsetX"": 10,
            ""offsetY"": 20,
            ""layers"": []
        }";
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(level);
        using MemoryStream stream = new MemoryStream(jsonBytes);

        string tempDir = Path.Combine(Path.GetTempPath(), "OgmoTest_" + Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        try
        {
            string projectPath = Path.Combine(tempDir, "test.ogmo");
            File.WriteAllText(projectPath, @"{
                ""ogmoVersion"": ""3.4.0"",
                ""name"": ""TestProject"",
                ""backgroundColor"": ""#282c34ff"",
                ""layerGridDefaultSize"": {""x"": 16, ""y"": 16},
                ""layers"": [],
                ""tilesets"": []
            }");

            OgmoJsonParser parser = new OgmoJsonParser(projectPath);
            Tilemap tilemap = parser.ParseFromStream(stream, _graphicsDevice);

            Assert.True(tilemap.Properties.TryGetValue("Ogmo_Version", out TilemapPropertyValue version));
            Assert.Equal("3.4.0", version.AsString());

            Assert.True(tilemap.Properties.TryGetValue("Ogmo_PixelWidth", out TilemapPropertyValue pixelWidth));
            Assert.Equal(640, pixelWidth.AsInt());

            Assert.True(tilemap.Properties.TryGetValue("Ogmo_PixelHeight", out TilemapPropertyValue pixelHeight));
            Assert.Equal(480, pixelHeight.AsInt());

            Assert.True(tilemap.Properties.TryGetValue("Ogmo_OffsetX", out TilemapPropertyValue offsetX));
            Assert.Equal(10, offsetX.AsInt());

            Assert.True(tilemap.Properties.TryGetValue("Ogmo_OffsetY", out TilemapPropertyValue offsetY));
            Assert.Equal(20, offsetY.AsInt());
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void Parse_RealOgmoFile_Simple_LoadsSuccessfully()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        string levelPath = Path.Combine(testDataPath, "Ogmo", "levels", "simple_level.json");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);

        Tilemap tilemap = parser.ParseFromFile(levelPath, _graphicsDevice);

        Assert.NotNull(tilemap);
        Assert.Equal(20, tilemap.Width);
        Assert.Equal(15, tilemap.Height);
        Assert.Equal(16, tilemap.TileWidth);
        Assert.Equal(16, tilemap.TileHeight);
        Assert.Equal(TilemapOrientation.Orthogonal, tilemap.Orientation);
    }

    [Fact]
    public void Parse_RealOgmoFile_Simple_LoadsAllLayers()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        string levelPath = Path.Combine(testDataPath, "Ogmo", "levels", "simple_level.json");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);

        Tilemap tilemap = parser.ParseFromFile(levelPath, _graphicsDevice);

        Assert.NotNull(tilemap.Layers);
        Assert.Equal(4, tilemap.Layers.Count);

        Assert.Equal("Ground", tilemap.Layers[0].Name);
        Assert.IsType<TilemapTileLayer>(tilemap.Layers[0]);

        Assert.Equal("Collision", tilemap.Layers[1].Name);
        Assert.IsType<TilemapTileLayer>(tilemap.Layers[1]);

        Assert.Equal("Entities", tilemap.Layers[2].Name);
        Assert.IsType<TilemapObjectLayer>(tilemap.Layers[2]);

        Assert.Equal("Decals", tilemap.Layers[3].Name);
        Assert.IsType<TilemapObjectLayer>(tilemap.Layers[3]);
    }

    [Fact]
    public void Parse_RealOgmoFile_Simple_LoadsTileset()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        string levelPath = Path.Combine(testDataPath, "Ogmo", "levels", "simple_level.json");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);

        Tilemap tilemap = parser.ParseFromFile(levelPath, _graphicsDevice);

        Assert.NotNull(tilemap.Tilesets);
        Assert.Single(tilemap.Tilesets);

        TilemapTileset tileset = tilemap.Tilesets[0];
        Assert.Equal("TestTileset", tileset.Name);
        Assert.Equal(1, tileset.FirstGlobalId);
        Assert.Equal(16, tileset.TileWidth);
        Assert.Equal(16, tileset.TileHeight);
    }

    [Fact]
    public void Parse_RealOgmoFile_Simple_LoadsTileLayerData()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        string levelPath = Path.Combine(testDataPath, "Ogmo", "levels", "simple_level.json");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);

        Tilemap tilemap = parser.ParseFromFile(levelPath, _graphicsDevice);

        TilemapTileLayer groundLayer = tilemap.Layers.GetLayer<TilemapTileLayer>("Ground");
        Assert.NotNull(groundLayer);
        Assert.Equal(20, groundLayer.Width);
        Assert.Equal(15, groundLayer.Height);

        TilemapTile? tile = groundLayer.GetTile(1, 1);
        Assert.NotNull(tile);
        Assert.Equal(1, tile.Value.GlobalId);
    }

    [Fact]
    public void Parse_RealOgmoFile_Simple_LoadsGridLayerData()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        string levelPath = Path.Combine(testDataPath, "Ogmo", "levels", "simple_level.json");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);

        Tilemap tilemap = parser.ParseFromFile(levelPath, _graphicsDevice);

        TilemapTileLayer collisionLayer = tilemap.Layers.GetLayer<TilemapTileLayer>("Collision");
        Assert.NotNull(collisionLayer);

        Assert.True(collisionLayer.Properties.TryGetValue("Ogmo_GridValues", out TilemapPropertyValue gridValues));
        Assert.NotNull(gridValues.AsString());
    }

    [Fact]
    public void Parse_RealOgmoFile_Simple_LoadsEntities()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        string levelPath = Path.Combine(testDataPath, "Ogmo", "levels", "simple_level.json");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);

        Tilemap tilemap = parser.ParseFromFile(levelPath, _graphicsDevice);

        TilemapObjectLayer entityLayer = tilemap.Layers.GetLayer<TilemapObjectLayer>("Entities");
        Assert.NotNull(entityLayer);
        Assert.Single(entityLayer.Objects);

        TilemapObject player = entityLayer.Objects[0];
        Assert.Equal("Player", player.Name);
        Assert.Equal(64, player.Position.X);
        Assert.Equal(64, player.Position.Y);

        Assert.True(player.Properties.TryGetValue("health", out TilemapPropertyValue health));
        Assert.Equal(100, health.AsInt());

        Assert.True(player.Properties.TryGetValue("speed", out TilemapPropertyValue speed));
        Assert.Equal(5.5f, speed.AsFloat());
    }

    [Fact]
    public void Parse_RealOgmoFile_Simple_StoresOgmoProperties()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        string levelPath = Path.Combine(testDataPath, "Ogmo", "levels", "simple_level.json");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);

        Tilemap tilemap = parser.ParseFromFile(levelPath, _graphicsDevice);

        Assert.True(tilemap.Properties.TryGetValue("Ogmo_Version", out TilemapPropertyValue version));
        Assert.Equal("3.4.0", version.AsString());

        Assert.True(tilemap.Properties.TryGetValue("Ogmo_PixelWidth", out TilemapPropertyValue width));
        Assert.Equal(320, width.AsInt());

        Assert.True(tilemap.Properties.TryGetValue("Ogmo_PixelHeight", out TilemapPropertyValue height));
        Assert.Equal(240, height.AsInt());
    }

    [Fact]
    public void Parse_RealOgmoFile_Complex_LoadsSuccessfully()
    {
        string testDataPath = GetTestDataPath();
        string projectPath = Path.Combine(testDataPath, "Ogmo", "test_project.ogmo");
        string levelPath = Path.Combine(testDataPath, "Ogmo", "levels", "complex_level.json");
        OgmoJsonParser parser = new OgmoJsonParser(projectPath);

        Tilemap tilemap = parser.ParseFromFile(levelPath, _graphicsDevice);

        Assert.NotNull(tilemap);
        Assert.Equal(40, tilemap.Width);
        Assert.Equal(30, tilemap.Height);
        Assert.Equal(4, tilemap.Layers.Count);
    }

    private string GetTestDataPath()
    {
        DirectoryInfo currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
        DirectoryInfo searchDir = currentDir;

        while (searchDir != null && !File.Exists(Path.Combine(searchDir.FullName, "MonoGame.Extended.sln")))
        {
            searchDir = searchDir.Parent;
        }

        if (searchDir == null)
        {
            throw new DirectoryNotFoundException("Could not find solution root");
        }

        return Path.Combine(searchDir.FullName, "tests", "MonoGame.Extended.Tests", "TestData");
    }
}
