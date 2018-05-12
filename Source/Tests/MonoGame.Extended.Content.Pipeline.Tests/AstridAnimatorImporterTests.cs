using System;
using System.IO;
using MonoGame.Extended.Content.Pipeline.Animations;
using Xunit;

namespace MonoGame.Extended.Content.Pipeline.Tests
{
    
    public class AstridAnimatorImporterTests
    {
        [Fact]
        public void AstridAnimatorImporter_Import_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "astrid-animator.aa");
            var importer = new AstridAnimatorImporter();
            var result = importer.Import(filePath, null);
            var data = result.Data;

            Assert.Equal("astrid-animator-atlas.json", data.TextureAtlas);
            Assert.Equal(2, data.Animations.Count);

            Assert.Equal("appear", data.Animations[0].Name);
            Assert.Equal(8, data.Animations[0].FramesPerSecond);
            Assert.Equal(2, data.Animations[0].Frames.Count);
            Assert.Equal("appear_01", data.Animations[0].Frames[0]);
            Assert.Equal("appear_02", data.Animations[0].Frames[1]);

            Assert.Equal("die", data.Animations[1].Name);
            Assert.Equal(16, data.Animations[1].FramesPerSecond);
            Assert.Single(data.Animations[1].Frames);
            Assert.Equal("die_01", data.Animations[1].Frames[0]);
        }
    }
}
