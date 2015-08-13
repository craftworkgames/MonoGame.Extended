using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.Pipeline.Tiled;
using NSubstitute;
using NUnit.Framework;

namespace MonoGame.Extended.Content.Pipeline.Tests
{
    [TestFixture]
    public class TiledMapImporterTests
    {
        [Test]
        public void TiledMapImporter_Import_Test()
        {
            const string filename = @"TestData\level01.tmx";
            var importer = new TiledMapImporter();
            var map = importer.Import(filename, Substitute.For<ContentImporterContext>());

            Assert.IsNotNull(map);
            //Assert.AreEqual("Tile Layer 2", map.Layers[0].Name);
        }

        [Test]
        public void TiledMapImporter_Xml_Test()
        {
            const string filename = @"TestData\test-tileset-xml.tmx";
            var importer = new TiledMapImporter();
            var map = importer.Import(filename, Substitute.For<ContentImporterContext>());
            var expectedData = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var actualData = map.Layers[0].Data.Tiles.Select(i => i.Gid).ToArray();

            Assert.IsNull(map.Layers[0].Data.Encoding);
            Assert.IsNull(map.Layers[0].Data.Compression);
            Assert.IsTrue(expectedData.SequenceEqual(actualData));
        }

        [Test]
        public void TiledMapImporter_Csv_Test()
        {
            const string filename = @"TestData\test-tileset-csv.tmx";
            var importer = new TiledMapImporter();
            var map = importer.Import(filename, Substitute.For<ContentImporterContext>());
            var expectedData = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var actualData = map.Layers[0].Data.Tiles.Select(i => i.Gid).ToArray();

            Assert.AreEqual("csv", map.Layers[0].Data.Encoding);
            Assert.IsNull(map.Layers[0].Data.Compression);
            //Assert.IsTrue(expectedData.SequenceEqual(actualData));
        }
    }
}
