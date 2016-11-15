using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.Pipeline.Tiled;
using NSubstitute;
using NUnit.Framework;

namespace MonoGame.Extended.Content.Pipeline.Tests
{
    [TestFixture]
    public class TiledMapImporterProcessorTests
    {
        [Test]
        public void TiledMapImporter_Import_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "level01.tmx");

            var logger = Substitute.For<ContentBuildLogger>();
            var importer = new TiledMapImporter();
            var importerContext = Substitute.For<ContentImporterContext>();
            importerContext.Logger.Returns(logger);

            var map = importer.Import(filePath, importerContext);

            Assert.AreEqual("1.0", map.Version);
            Assert.AreEqual(TmxOrientation.Orthogonal, map.Orientation);
            Assert.AreEqual(TmxRenderOrder.RightDown, map.RenderOrder);
            Assert.AreEqual(20, map.Width);
            Assert.AreEqual(10, map.Height);
            Assert.AreEqual(128, map.TileWidth);
            Assert.AreEqual(128, map.TileHeight);
            Assert.AreEqual("#7d7d7d", map.BackgroundColor);
            Assert.AreEqual("awesome", map.Properties[0].Name);
            Assert.AreEqual("42", map.Properties[0].Value);
            Assert.AreEqual(1, map.Tilesets.Count);
            Assert.AreEqual(3, map.Layers.Count);
            Assert.AreEqual(TmxOrientation.Orthogonal,map.Orientation);

            var tileset = map.Tilesets.First();
            Assert.AreEqual(1, tileset.FirstGid);
            Assert.AreEqual("free-tileset.png", tileset.Image.Source);
            Assert.AreEqual(652, tileset.Image.Width);
            Assert.AreEqual(783, tileset.Image.Height);
            Assert.AreEqual(2, tileset.Margin);
            Assert.AreEqual(30, tileset.TileCount);
            Assert.AreEqual("free-tileset", tileset.Name);
            Assert.AreEqual(null, tileset.Source);
            Assert.AreEqual(2, tileset.Spacing);
            Assert.AreEqual(0, tileset.TerrainTypes.Count);
            Assert.AreEqual(0, tileset.Properties.Count);
            Assert.AreEqual(128, tileset.TileHeight);
            Assert.AreEqual(128, tileset.TileWidth);
            Assert.AreEqual(0, tileset.TileOffset.X);
            Assert.AreEqual(0, tileset.TileOffset.Y);

            var tileLayer2 = (TmxTileLayer) map.Layers[0];
            Assert.AreEqual("Tile Layer 2", tileLayer2.Name);
            Assert.AreEqual(1, tileLayer2.Opacity);
            Assert.AreEqual(0, tileLayer2.Properties.Count);
            Assert.AreEqual(true, tileLayer2.Visible);
            Assert.AreEqual(200, tileLayer2.Data.Tiles.Count);
            Assert.AreEqual(0, tileLayer2.X);
            Assert.AreEqual(0, tileLayer2.Y);

            var imageLayer = (TmxImageLayer)map.Layers[1];
            Assert.AreEqual("Image Layer 1", imageLayer.Name);
            Assert.AreEqual(1, imageLayer.Opacity);
            Assert.AreEqual(0, imageLayer.Properties.Count);
            Assert.AreEqual(true, imageLayer.Visible);
            Assert.AreEqual("hills.png", imageLayer.Image.Source);
            Assert.AreEqual(100, imageLayer.X);
            Assert.AreEqual(100, imageLayer.Y);

            var tileLayer1 = (TmxTileLayer)map.Layers[2];
            Assert.AreEqual("Tile Layer 1", tileLayer1.Name);
            Assert.AreEqual(2, tileLayer1.Properties.Count);

            Assert.AreEqual("customlayerprop", tileLayer1.Properties[0].Name);
            Assert.AreEqual("1", tileLayer1.Properties[0].Value);

            Assert.AreEqual("customlayerprop2", tileLayer1.Properties[1].Name);
            Assert.AreEqual("2", tileLayer1.Properties[1].Value);
        }

        [Test]
        public void TiledMapImporter_Xml_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "test-tileset-xml.tmx");
            var map = ImportAndProcessMap(filePath);
            var layer = map.Layers.OfType<TmxTileLayer>().First();
            var actualData = layer.Data.Tiles.Select(i => i.Gid).ToArray();

            Assert.IsNull(layer.Data.Encoding);
            Assert.IsNull(layer.Data.Compression);
            Assert.IsTrue(new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.SequenceEqual(actualData));
        }
        
        [Test]
        public void TiledMapImporter_Csv_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "test-tileset-csv.tmx");
            var map = ImportAndProcessMap(filePath);
            var layer = map.Layers.OfType<TmxTileLayer>().First();
            var data = layer.Data.Tiles.Select(i => i.Gid).ToArray();

            Assert.AreEqual("csv", layer.Data.Encoding);
            Assert.IsNull(layer.Data.Compression);
            Assert.IsTrue(new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.SequenceEqual(data));
        }

        [Test]
        public void TiledMapImporter_Base64_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "test-tileset-base64.tmx");
            var map = ImportAndProcessMap(filePath);
            var layer = map.Layers.OfType<TmxTileLayer>().First();
            var data = layer.Data.Tiles.Select(i => i.Gid).ToArray();

            Assert.AreEqual("base64", layer.Data.Encoding);
            Assert.IsNull(layer.Data.Compression);
            Assert.IsTrue(new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.SequenceEqual(data));
        }

        [Test]
        public void TiledMapImporter_Gzip_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "test-tileset-gzip.tmx");
            var map = ImportAndProcessMap(filePath);
            var layer = map.Layers.OfType<TmxTileLayer>().First();
            var data = layer.Data.Tiles.Select(i => i.Gid).ToArray();

            Assert.AreEqual("base64", layer.Data.Encoding);
            Assert.AreEqual("gzip", layer.Data.Compression);
            Assert.IsTrue(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.SequenceEqual(data));
        }


        [Test]
        public void TiledMapImporter_Zlib_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "test-tileset-zlib.tmx");
            var map = ImportAndProcessMap(filePath);
            var layer = map.Layers.OfType<TmxTileLayer>().First();
            var data = layer.Data.Tiles.Select(i => i.Gid).ToArray();

            Assert.AreEqual("base64", layer.Data.Encoding);
            Assert.AreEqual("zlib", layer.Data.Compression);
            Assert.IsTrue(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.SequenceEqual(data));
        }

        [Test]
        public void TiledMapImporter_ObjectLayer_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "test-object-layer.tmx");
            var map = ImportAndProcessMap(filePath);

            Assert.AreEqual(1, map.Layers.Count);
            Assert.IsInstanceOf<TmxObjectLayer>(map.Layers[0]);
            var tmxObjectGroup = map.Layers[0] as TmxObjectLayer;
            var tmxObject = tmxObjectGroup.Objects[0];
            var tmxPolygon = tmxObjectGroup.Objects[3].Polygon;
            var tmxPolyline = tmxObjectGroup.Objects[4].Polyline;

            Assert.AreEqual("Object Layer 1", tmxObjectGroup.Name);
            Assert.AreEqual(1, tmxObject.Id);
            Assert.AreEqual(131.345f, tmxObject.X);
            Assert.AreEqual(65.234f, tmxObject.Y);
            Assert.AreEqual(311.111f, tmxObject.Width);
            Assert.AreEqual(311.232f, tmxObject.Height);
            Assert.AreEqual(1, tmxObject.Properties.Count);
            Assert.AreEqual("shape", tmxObject.Properties[0].Name);
            Assert.AreEqual("circle", tmxObject.Properties[0].Value);
            Assert.IsNotNull(tmxObject.Ellipse);
            Assert.IsFalse(tmxObjectGroup.Objects[1].Visible);
            Assert.AreEqual(-1, tmxObjectGroup.Objects[1].Gid);
            Assert.AreEqual(23, tmxObjectGroup.Objects[5].Gid);
            Assert.AreEqual("rectangle", tmxObjectGroup.Objects[2].Type);
            Assert.IsNotNull(tmxPolygon);
            Assert.AreEqual("0,0 180,90 -8,275 -45,81 38,77", tmxPolygon.Points);
            Assert.IsNotNull(tmxPolyline);
            Assert.AreEqual("0,0 28,299 326,413 461,308", tmxPolyline.Points);
        }


        private static TmxMap ImportAndProcessMap(string filename)
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

            return result.Map;
        }
    }
}
