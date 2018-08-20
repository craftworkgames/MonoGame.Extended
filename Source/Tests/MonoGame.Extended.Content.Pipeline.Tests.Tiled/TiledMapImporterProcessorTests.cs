using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.Pipeline.Tiled;
using MonoGame.Extended.Tiled.Serialization;
using NSubstitute;
using Xunit;

namespace MonoGame.Extended.Content.Pipeline.Tests.Tiled
{

    public class TiledMapImporterProcessorTests
    {
        [Fact]
        public void TiledMapImporter_Import_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "level01.tmx");

            var logger = Substitute.For<ContentBuildLogger>();
            var importer = new TiledMapImporter();
            var importerContext = Substitute.For<ContentImporterContext>();
            importerContext.Logger.Returns(logger);

            var contentItem = importer.Import(filePath, importerContext);
            var map = contentItem.Data;

            Assert.Equal("1.0", map.Version);
            Assert.Equal(TiledMapOrientationContent.Orthogonal, map.Orientation);
            Assert.Equal(TiledMapTileDrawOrderContent.RightDown, map.RenderOrder);
            Assert.Equal(20, map.Width);
            Assert.Equal(10, map.Height);
            Assert.Equal(128, map.TileWidth);
            Assert.Equal(128, map.TileHeight);
            Assert.Equal("#7d7d7d", map.BackgroundColor);
            Assert.Equal("awesome", map.Properties[0].Name);
            Assert.Equal("42", map.Properties[0].Value);
            Assert.Single(map.Tilesets);
            Assert.Equal(3, map.Layers.Count);
            Assert.Equal(TiledMapOrientationContent.Orthogonal, map.Orientation);

            var tileset = map.Tilesets.First();
            Assert.Equal(1, tileset.FirstGlobalIdentifier);
            Assert.Equal("free-tileset.png", Path.GetFileName(tileset.Image.Source));
            Assert.Equal(652, tileset.Image.Width);
            Assert.Equal(783, tileset.Image.Height);
            Assert.Equal(2, tileset.Margin);
            Assert.Equal(30, tileset.TileCount);
            Assert.Equal("free-tileset", tileset.Name);
            Assert.Null(tileset.Source);
            Assert.Equal(2, tileset.Spacing);
            //Assert.Equal(0, tileset.TerrainTypes.Count);
            Assert.Empty(tileset.Properties);
            Assert.Equal(128, tileset.TileHeight);
            Assert.Equal(128, tileset.TileWidth);
            Assert.Equal(0, tileset.TileOffset.X);
            Assert.Equal(0, tileset.TileOffset.Y);

            var tileLayer2 = (TiledMapTileLayerContent)map.Layers[0];
            Assert.Equal("Tile Layer 2", tileLayer2.Name);
            Assert.Equal(1, tileLayer2.Opacity);
            Assert.Empty(tileLayer2.Properties);
            Assert.True(tileLayer2.Visible);
            Assert.Equal(200, tileLayer2.Data.Tiles.Count);
            Assert.Equal(0, tileLayer2.X);
            Assert.Equal(0, tileLayer2.Y);

            var imageLayer = (TiledMapImageLayerContent)map.Layers[1];
            Assert.Equal("Image Layer 1", imageLayer.Name);
            Assert.Equal(1, imageLayer.Opacity);
            Assert.Empty(imageLayer.Properties);
            Assert.True(imageLayer.Visible);
            Assert.Equal("hills.png", Path.GetFileName(imageLayer.Image.Source));
            Assert.Equal(100, imageLayer.X);
            Assert.Equal(100, imageLayer.Y);

            var tileLayer1 = (TiledMapTileLayerContent)map.Layers[2];
            Assert.Equal("Tile Layer 1", tileLayer1.Name);
            Assert.Equal(2, tileLayer1.Properties.Count);

            Assert.Equal("customlayerprop", tileLayer1.Properties[0].Name);
            Assert.Equal("1", tileLayer1.Properties[0].Value);

            Assert.Equal("customlayerprop2", tileLayer1.Properties[1].Name);
            Assert.Equal("2", tileLayer1.Properties[1].Value);
        }

        [Fact]
        public void TiledMapImporter_Xml_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "test-tileset-xml.tmx");
            var map = ImportAndProcessMap(filePath);
            var layer = map.Layers.OfType<TiledMapTileLayerContent>().First();
            var actualData = layer.Data.Tiles.Select(i => i.GlobalIdentifier).ToArray();

            Assert.Null(layer.Data.Encoding);
            Assert.Null(layer.Data.Compression);
            Assert.True(new uint[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.SequenceEqual(actualData));
        }

        [Fact]
        public void TiledMapImporter_Csv_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "test-tileset-csv.tmx");
            var map = ImportAndProcessMap(filePath);
            var layer = map.Layers.OfType<TiledMapTileLayerContent>().First();
            var data = layer.Data.Tiles.Select(i => i.GlobalIdentifier).ToArray();

            Assert.Equal("csv", layer.Data.Encoding);
            Assert.Null(layer.Data.Compression);
            //Assert.True(new uint[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.SequenceEqual(data));
        }

        [Fact]
        public void TiledMapImporter_Base64_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "test-tileset-base64.tmx");
            var map = ImportAndProcessMap(filePath);
            var layer = map.Layers.OfType<TiledMapTileLayerContent>().First();
            var data = layer.Data.Tiles.Select(i => i.GlobalIdentifier).ToArray();

            Assert.Equal("base64", layer.Data.Encoding);
            Assert.Null(layer.Data.Compression);
            //Assert.True(new uint[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.SequenceEqual(data));
        }

        [Fact]
        public void TiledMapImporter_Gzip_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "test-tileset-gzip.tmx");
            var map = ImportAndProcessMap(filePath);
            var layer = map.Layers.OfType<TiledMapTileLayerContent>().First();
            var data = layer.Data.Tiles.Select(i => i.GlobalIdentifier).ToArray();

            Assert.Equal("base64", layer.Data.Encoding);
            Assert.Equal("gzip", layer.Data.Compression);
            //Assert.True(new uint[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.SequenceEqual(data));
        }


        [Fact]
        public void TiledMapImporter_Zlib_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "test-tileset-zlib.tmx");
            var map = ImportAndProcessMap(filePath);
            var layer = map.Layers.OfType<TiledMapTileLayerContent>().First();
            var data = layer.Data.Tiles.Select(i => i.GlobalIdentifier).ToArray();

            Assert.Equal("base64", layer.Data.Encoding);
            Assert.Equal("zlib", layer.Data.Compression);
            //Assert.True(new uint[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.SequenceEqual(data));
        }

        [Fact]
        public void TiledMapImporter_ObjectLayer_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "test-object-layer.tmx");
            var map = ImportAndProcessMap(filePath);

            Assert.Single(map.Layers);
            Assert.IsType<TiledMapObjectLayerContent>(map.Layers[0]);
            var tmxObjectGroup = map.Layers[0] as TiledMapObjectLayerContent;
            var tmxObject = tmxObjectGroup.Objects[0];
            var tmxPolygon = tmxObjectGroup.Objects[3].Polygon;
            var tmxPolyline = tmxObjectGroup.Objects[4].Polyline;

            Assert.Equal("Object Layer 1", tmxObjectGroup.Name);
            Assert.Equal(1, tmxObject.Identifier);
            Assert.Equal(131.345f, tmxObject.X);
            Assert.Equal(65.234f, tmxObject.Y);
            Assert.Equal(311.111f, tmxObject.Width);
            Assert.Equal(311.232f, tmxObject.Height);
            Assert.Single(tmxObject.Properties);
            Assert.Equal("shape", tmxObject.Properties[0].Name);
            Assert.Equal("circle", tmxObject.Properties[0].Value);
            Assert.NotNull(tmxObject.Ellipse);
            Assert.False(tmxObjectGroup.Objects[1].Visible);
            Assert.Equal((uint)0, tmxObjectGroup.Objects[1].GlobalIdentifier);
            Assert.Equal((uint)23, tmxObjectGroup.Objects[5].GlobalIdentifier);
            Assert.Equal("rectangle", tmxObjectGroup.Objects[2].Type);
            Assert.NotNull(tmxPolygon);
            Assert.Equal("0,0 180,90 -8,275 -45,81 38,77", tmxPolygon.Points);
            Assert.NotNull(tmxPolyline);
            Assert.Equal("0,0 28,299 326,413 461,308", tmxPolyline.Points);
        }


        private static TiledMapContent ImportAndProcessMap(string filename)
        {
            var logger = Substitute.For<ContentBuildLogger>();
            var importer = new TiledMapImporter();
            var importerContext = Substitute.For<ContentImporterContext>();
            importerContext.Logger.Returns(logger);

            var processor = new TiledMapProcessor();
            var processorContext = Substitute.For<ContentProcessorContext>();
            processorContext.Logger.Returns(logger);

            var import = importer.Import(filename, importerContext);
            var result = processor.Process(import, processorContext);

            return result.Data;
        }
    }
}
