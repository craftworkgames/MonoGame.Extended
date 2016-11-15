using System;
using System.IO;
using MonoGame.Extended.Content.Pipeline.Animations;
using NUnit.Framework;

namespace MonoGame.Extended.Content.Pipeline.Tests
{
    [TestFixture]
    public class AstridAnimatorImporterTests
    {
        [Test]
        public void AstridAnimatorImporter_Import_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "astrid-animator.aa");
            var importer = new AstridAnimatorImporter();
            var result = importer.Import(filePath, null);
            var data = result.Data;

            Assert.AreEqual("astrid-animator-atlas.json", data.TextureAtlas);
            Assert.AreEqual(2, data.Animations.Count);

            Assert.AreEqual("appear", data.Animations[0].Name);
            Assert.AreEqual(8, data.Animations[0].FramesPerSecond);
            Assert.AreEqual(2, data.Animations[0].Frames.Count);
            Assert.AreEqual("appear_01", data.Animations[0].Frames[0]);
            Assert.AreEqual("appear_02", data.Animations[0].Frames[1]);

            Assert.AreEqual("die", data.Animations[1].Name);
            Assert.AreEqual(16, data.Animations[1].FramesPerSecond);
            Assert.AreEqual(1, data.Animations[1].Frames.Count);
            Assert.AreEqual("die_01", data.Animations[1].Frames[0]);
        }
    }
}
