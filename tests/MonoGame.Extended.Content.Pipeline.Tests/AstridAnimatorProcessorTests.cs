using System;
using System.IO;
using MonoGame.Extended.Content.Pipeline.Animations;
using Xunit;

namespace MonoGame.Extended.Content.Pipeline.Tests
{
    
    public class AstridAnimatorProcessorTests
    {
        [Fact]
        public void AstridAnimatorProcessor_Process_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "astrid-animator.aa");

            var importer = new AstridAnimatorImporter();
            var importerResult = importer.Import(filePath, null);

            var processor = new AstridAnimatorProcessor();
            var result = processor.Process(importerResult, null);

            Assert.Equal("astrid-animator-atlas", result.TextureAtlasAssetName);
            Assert.Equal("TestData", Path.GetFileName(result.Directory));
            Assert.Equal(3, result.Frames.Count);
            Assert.Equal("appear_01", result.Frames[0]);
            Assert.Equal("appear_02", result.Frames[1]);
            Assert.Equal("die_01", result.Frames[2]);
        }
    }
}