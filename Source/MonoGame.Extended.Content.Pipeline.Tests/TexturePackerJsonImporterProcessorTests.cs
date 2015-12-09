using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.Pipeline.TextureAtlases;
using NSubstitute;
using NUnit.Framework;

namespace MonoGame.Extended.Content.Pipeline.Tests
{
    [TestFixture]
    public class TexturePackerJsonImporterProcessorTests
    {
        [Test]
        public void TexturePackerJsonImporter_Import_Test()
        {
            const string filename = @"TestData\test-tileset.json";
            var importer = new TexturePackerJsonImporter();
            var data = importer.Import(filename, Substitute.For<ContentImporterContext>());

            Assert.IsNotNull(data);
        }

        [Test]
        public void TexturePackerJsonImporter_Processor_Test()
        {
            const string filename = @"TestData\test-tileset.json";
            var importer = new TexturePackerJsonImporter();
            var input = importer.Import(filename, Substitute.For<ContentImporterContext>());
            var processor = new TexturePackerProcessor();
            var output = processor.Process(input, Substitute.For<ContentProcessorContext>());

            Assert.IsNotNull(output);
        }
    }
}