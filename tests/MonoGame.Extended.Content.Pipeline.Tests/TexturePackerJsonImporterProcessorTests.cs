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
            var importResult = importer.Import(filePath, Substitute.For<ContentImporterContext>());

            Assert.NotNull(importResult);

            // Check meta.image contains image name
            Assert.Equal("test-tileset.png", importResult.Data.Meta.Image);

            // Check regions count and textures
            Assert.Equal(9, importResult.Data.Regions.Count);
            Assert.Null(importResult.Data.Textures);  // only used by new MonoGame.Extended JSON format

            // Check first region
            var firstRegion = importResult.Data.Regions[0];
            Assert.Equal("1.png", firstRegion.FileName);
            Assert.Equal(2, firstRegion.Frame.X);
            Assert.Equal(2, firstRegion.Frame.Y);
            Assert.Equal(32, firstRegion.Frame.Width);
            Assert.Equal(32, firstRegion.Frame.Height);
            Assert.Equal(0.5, firstRegion.PivotPoint.X);
            Assert.Equal(0.5, firstRegion.PivotPoint.Y);
            Assert.False(firstRegion.Rotated);
            Assert.Equal(32, firstRegion.SourceSize.Width);
            Assert.Equal(32, firstRegion.SourceSize.Height);
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

        [Fact]
        public void TexturePackerJsonImporter_Import_NewFormat_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath(@"TestData/test-atlas.json");
            var importer = new TexturePackerJsonImporter();
            var importResult = importer.Import(filePath, Substitute.For<ContentImporterContext>());

            // Regions must be null (only used by old generic json format)
            Assert.Null(importResult.Data.Regions);

            // Textures contains 1 texture
            Assert.NotNull(importResult.Data.Textures);
            Assert.Single(importResult.Data.Textures);

            var texture = importResult.Data.Textures[0];
            Assert.Equal("test-atlas-texture.png", texture.FileName);

            // The first texture contains frames
            Assert.NotNull(texture.Frames);
            Assert.Contains("0001.png", texture.Frames.Keys);

            // Check properties of the first frame ("0001.png")
            var frame = texture.Frames["0001.png"];
            Assert.Equal(164, frame.Frame.X);
            Assert.Equal(1, frame.Frame.Y);
            Assert.Equal(158, frame.Frame.Width);
            Assert.Equal(316, frame.Frame.Height);

            Assert.NotNull(frame.Size);
            Assert.Equal(187, frame.Size.Width);
            Assert.Equal(324, frame.Size.Height);

            Assert.NotNull(frame.Offset);
            Assert.Equal(15, frame.Offset.X);
            Assert.Equal(3, frame.Offset.Y);

            Assert.Equal(0, frame.Rotated);

            Assert.NotNull(frame.Pivot);
            Assert.Equal(0.5, frame.Pivot.X);
            Assert.Equal(1.0, frame.Pivot.Y);
        }
    }
}
