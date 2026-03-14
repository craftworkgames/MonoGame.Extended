using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tests.Fixtures;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.LDtk;
using MonoGame.Extended.Tilemaps.Parsers;

namespace MonoGame.Extended.Tests.Tilemaps;

[Collection("GraphicsTest")]
public class LDtkJsonParserTests
{
    private readonly GraphicsDevice _graphicsDevice;

    public LDtkJsonParserTests(GraphicsTestFixture fixture)
    {
        _graphicsDevice = fixture.GraphicsDevice;
    }

    [Fact]
    public void Parser_SupportedExtensions_ContainsLdtk()
    {
        LDtkJsonParser parser = new LDtkJsonParser();

        Assert.Contains(".ldtk", parser.SupportedExtensions);
    }

    [Fact]
    public void CanParse_WithInvalidExtension_ReturnsFalse()
    {
        LDtkJsonParser parser = new LDtkJsonParser();

        bool result = parser.CanParse("test.txt");

        Assert.False(result);
    }

    [Fact]
    public void CanParse_WithNullPath_ReturnsFalse()
    {
        LDtkJsonParser parser = new LDtkJsonParser();

        bool result = parser.CanParse(null);

        Assert.False(result);
    }

    [Fact]
    public void ParseFromFile_WithNullPath_ThrowsArgumentNullException()
    {
        LDtkJsonParser parser = new LDtkJsonParser();

        Assert.Throws<ArgumentNullException>(() =>
            parser.ParseFromFile(null, _graphicsDevice));
    }

    [Fact]
    public void ParseFromFile_WithNullGraphicsDevice_ThrowsArgumentNullException()
    {
        LDtkJsonParser parser = new LDtkJsonParser();

        Assert.Throws<ArgumentNullException>(() =>
            parser.ParseFromFile("test.ldtk", null));
    }

    [Fact]
    public void ParseFromFile_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        LDtkJsonParser parser = new LDtkJsonParser();

        Assert.Throws<FileNotFoundException>(() =>
            parser.ParseFromFile("nonexistent.ldtk", _graphicsDevice));
    }

    [Fact]
    public void ParseFromStream_WithNullStream_ThrowsArgumentNullException()
    {
        LDtkJsonParser parser = new LDtkJsonParser();

        Assert.Throws<ArgumentNullException>(() =>
            parser.ParseFromStream(null, _graphicsDevice, "."));
    }

    [Fact]
    public void ParseFromStream_WithNullGraphicsDevice_ThrowsArgumentNullException()
    {
        LDtkJsonParser parser = new LDtkJsonParser();
        using MemoryStream stream = new MemoryStream();

        Assert.Throws<ArgumentNullException>(() =>
            parser.ParseFromStream(stream, null, "."));
    }

    [Fact]
    public void ParseFromStream_WithInvalidJson_ThrowsTilemapParseException()
    {
        LDtkJsonParser parser = new LDtkJsonParser();
        byte[] invalidJson = System.Text.Encoding.UTF8.GetBytes("{ invalid json }");
        using MemoryStream stream = new MemoryStream(invalidJson);

        Assert.Throws<TilemapParseException>(() =>
            parser.ParseFromStream(stream, _graphicsDevice, "."));
    }

    [Fact]
    public void ParseFromStream_WithEmptyProject_ThrowsTilemapParseException()
    {
        LDtkJsonParser parser = new LDtkJsonParser();
        string emptyProject = @"{
            ""jsonVersion"": ""1.5.3"",
            ""levels"": [],
            ""defs"": {
                ""layers"": [],
                ""entities"": [],
                ""tilesets"": [],
                ""enums"": []
            }
        }";
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(emptyProject);
        using MemoryStream stream = new MemoryStream(jsonBytes);

        Assert.Throws<TilemapParseException>(() =>
            parser.ParseFromStream(stream, _graphicsDevice, "."));
    }

    [Fact]
    public void ParseFromStream_WithMinimalProject_CreatesValidTilemap()
    {
        LDtkJsonParser parser = new LDtkJsonParser();
        string minimalProject = @"{
            ""jsonVersion"": ""1.5.3"",
            ""defaultGridSize"": 16,
            ""levels"": [
                {
                    ""identifier"": ""Level_0"",
                    ""iid"": ""test-iid"",
                    ""uid"": 1,
                    ""worldX"": 0,
                    ""worldY"": 0,
                    ""worldDepth"": 0,
                    ""pxWid"": 256,
                    ""pxHei"": 256,
                    ""__bgColor"": ""#40465B"",
                    ""layerInstances"": []
                }
            ],
            ""defs"": {
                ""layers"": [],
                ""entities"": [],
                ""tilesets"": [],
                ""enums"": []
            }
        }";
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(minimalProject);
        using MemoryStream stream = new MemoryStream(jsonBytes);

        Tilemap tilemap = parser.ParseFromStream(stream, _graphicsDevice, ".");

        Assert.NotNull(tilemap);
        Assert.Equal("Level_0", tilemap.Name);
        Assert.Equal(16, tilemap.Width);
        Assert.Equal(16, tilemap.Height);
        Assert.Equal(16, tilemap.TileWidth);
        Assert.Equal(16, tilemap.TileHeight);
        Assert.Equal(TilemapOrientation.Orthogonal, tilemap.Orientation);
    }

    [Fact]
    public void ParseFromStream_WithTileLayer_CreatesLayer()
    {
        LDtkJsonParser parser = new LDtkJsonParser();
        string projectWithLayer = @"{
            ""jsonVersion"": ""1.5.3"",
            ""defaultGridSize"": 16,
            ""levels"": [
                {
                    ""identifier"": ""Level_0"",
                    ""iid"": ""test-iid"",
                    ""uid"": 1,
                    ""worldX"": 0,
                    ""worldY"": 0,
                    ""worldDepth"": 0,
                    ""pxWid"": 256,
                    ""pxHei"": 256,
                    ""__bgColor"": ""#40465B"",
                    ""layerInstances"": [
                        {
                            ""__identifier"": ""Ground"",
                            ""__type"": ""Tiles"",
                            ""__cWid"": 16,
                            ""__cHei"": 16,
                            ""__gridSize"": 16,
                            ""__opacity"": 1.0,
                            ""__pxTotalOffsetX"": 0,
                            ""__pxTotalOffsetY"": 0,
                            ""__tilesetDefUid"": null,
                            ""iid"": ""layer-iid"",
                            ""levelId"": 1,
                            ""layerDefUid"": 1,
                            ""pxOffsetX"": 0,
                            ""pxOffsetY"": 0,
                            ""visible"": true,
                            ""gridTiles"": [],
                            ""autoLayerTiles"": [],
                            ""entityInstances"": [],
                            ""intGridCsv"": []
                        }
                    ]
                }
            ],
            ""defs"": {
                ""layers"": [],
                ""entities"": [],
                ""tilesets"": [],
                ""enums"": []
            }
        }";
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(projectWithLayer);
        using MemoryStream stream = new MemoryStream(jsonBytes);

        Tilemap tilemap = parser.ParseFromStream(stream, _graphicsDevice, ".");

        Assert.NotNull(tilemap);
        Assert.Single(tilemap.Layers);

        TilemapLayer layer = tilemap.Layers[0];
        Assert.Equal("Ground", layer.Name);
        Assert.True(layer.IsVisible);
        Assert.Equal(1.0f, layer.Opacity);
    }

    [Fact]
    public void ParseFromStream_StoresLDtkSpecificProperties()
    {
        LDtkJsonParser parser = new LDtkJsonParser();
        string project = @"{
            ""jsonVersion"": ""1.5.3"",
            ""defaultGridSize"": 16,
            ""levels"": [
                {
                    ""identifier"": ""Level_0"",
                    ""iid"": ""test-level-iid"",
                    ""uid"": 42,
                    ""worldX"": 100,
                    ""worldY"": 200,
                    ""worldDepth"": 0,
                    ""pxWid"": 256,
                    ""pxHei"": 256,
                    ""__bgColor"": ""#40465B"",
                    ""layerInstances"": []
                }
            ],
            ""defs"": {
                ""layers"": [],
                ""entities"": [],
                ""tilesets"": [],
                ""enums"": []
            }
        }";
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(project);
        using MemoryStream stream = new MemoryStream(jsonBytes);

        Tilemap tilemap = parser.ParseFromStream(stream, _graphicsDevice, ".");

        Assert.True(tilemap.Properties.TryGetValue("LDtk_Iid", out TilemapPropertyValue iid));
        Assert.Equal("test-level-iid", iid.AsString());

        Assert.True(tilemap.Properties.TryGetValue("LDtk_Uid", out TilemapPropertyValue uid));
        Assert.Equal(42, uid.AsInt());

        Assert.True(tilemap.Properties.TryGetValue("LDtk_WorldX", out TilemapPropertyValue worldX));
        Assert.Equal(100, worldX.AsInt());

        Assert.True(tilemap.Properties.TryGetValue("LDtk_WorldY", out TilemapPropertyValue worldY));
        Assert.Equal(200, worldY.AsInt());
    }

    [Fact]
    public void Parse_RealLDtkFile_LoadsSuccessfully()
    {
        string testDataPath = GetTestDataPath();
        string ldtkPath = Path.Combine(testDataPath, "LDtk", "LDtk_test_file_all_features.ldtk");
        LDtkJsonParser parser = new LDtkJsonParser();

        Tilemap tilemap = parser.ParseFromFile(ldtkPath, _graphicsDevice);

        Assert.NotNull(tilemap);
        Assert.Equal("Everything", tilemap.Name);
        Assert.True(tilemap.Width > 0);
        Assert.True(tilemap.Height > 0);
    }

    [Fact]
    public void Parse_RealLDtkFile_LoadsExternalLevel()
    {
        string testDataPath = GetTestDataPath();
        string ldtkPath = Path.Combine(testDataPath, "LDtk", "LDtk_test_file_all_features.ldtk");
        LDtkJsonParser parser = new LDtkJsonParser();

        Tilemap tilemap = parser.ParseFromFile(ldtkPath, _graphicsDevice);

        Assert.NotNull(tilemap.Layers);
        Assert.True(tilemap.Layers.Count > 0, "Should have loaded layers from external .ldtkl file");
    }

    [Fact]
    public void Parse_RealLDtkFile_LoadsTilesets()
    {
        string testDataPath = GetTestDataPath();
        string ldtkPath = Path.Combine(testDataPath, "LDtk", "LDtk_test_file_all_features.ldtk");
        LDtkJsonParser parser = new LDtkJsonParser();

        Tilemap tilemap = parser.ParseFromFile(ldtkPath, _graphicsDevice);

        Assert.NotNull(tilemap.Tilesets);
        Assert.True(tilemap.Tilesets.Count > 0, "Should have loaded tilesets");
    }

    [Fact]
    public void ParseAllLevels_RealLDtkFile_LoadsMultipleLevels()
    {
        string testDataPath = GetTestDataPath();
        string ldtkPath = Path.Combine(testDataPath, "LDtk", "LDtk_test_file_all_features.ldtk");
        LDtkJsonParser parser = new LDtkJsonParser();

        IReadOnlyList<Tilemap> tilemaps = parser.ParseAllLevels(ldtkPath, _graphicsDevice);

        Assert.NotNull(tilemaps);
        Assert.True(tilemaps.Count >= 4, "Test file should have at least 4 levels");

        Assert.Contains(tilemaps, t => t.Name == "Everything");
        Assert.Contains(tilemaps, t => t.Name == "Autolayer");
        Assert.Contains(tilemaps, t => t.Name == "Tiles_and_intgrid");
    }

    // ---- Exception quality ----

    [Fact]
    public void ParseFromStream_MissingExternalLevel_ExceptionNamesLevel()
    {
        LDtkJsonParser parser = new LDtkJsonParser();
        string projectJson = @"{
            ""jsonVersion"": ""1.5.3"",
            ""defaultGridSize"": 16,
            ""levels"": [
                {
                    ""identifier"": ""Level_0"",
                    ""iid"": ""test-iid"",
                    ""uid"": 1,
                    ""worldX"": 0,
                    ""worldY"": 0,
                    ""worldDepth"": 0,
                    ""pxWid"": 256,
                    ""pxHei"": 256,
                    ""__bgColor"": ""#40465B"",
                    ""externalRelPath"": ""levels/Level_0.ldtkl"",
                    ""layerInstances"": null
                }
            ],
            ""defs"": {
                ""layers"": [],
                ""entities"": [],
                ""tilesets"": [],
                ""enums"": []
            }
        }";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(projectJson);
        using MemoryStream stream = new MemoryStream(bytes);

        TilemapParseException ex = Assert.Throws<TilemapParseException>(() =>
            parser.ParseFromStream(stream, _graphicsDevice, "/nonexistent/basepath"));

        Assert.Contains("Level_0", ex.Message);
        Assert.Contains("levels/Level_0.ldtkl", ex.Message);
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
