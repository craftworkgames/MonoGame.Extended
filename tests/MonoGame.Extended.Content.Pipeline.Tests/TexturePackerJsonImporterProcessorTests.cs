using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.Pipeline.TextureAtlases;
using NSubstitute;

namespace MonoGame.Extended.Content.Pipeline.Tests
{

    public class TexturePackerJsonImporterProcessorTests
    {
        [Fact]
        public void TexturePackerJsonImporter_Import_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath(@"TestData/test-tileset.json");
            var importer = new TexturePackerJsonImporter();
            var data = importer.Import(filePath, Substitute.For<ContentImporterContext>());

            Assert.NotNull(data);
        }

        [Fact]
        public void TexturePackerJsonImporter_Processor_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath(@"TestData/test-tileset.json");
            var importer = new TexturePackerJsonImporter();
            var input = importer.Import(filePath, Substitute.For<ContentImporterContext>());
            var processor = new TexturePackerProcessor();
            var output = processor.Process(input, Substitute.For<ContentProcessorContext>());

            Assert.NotNull(output);
        }
    }
}
