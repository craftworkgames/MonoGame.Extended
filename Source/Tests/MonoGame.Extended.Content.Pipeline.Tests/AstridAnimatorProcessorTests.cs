using System;
using System.IO;
using MonoGame.Extended.Content.Pipeline.Animations;
using NUnit.Framework;

namespace MonoGame.Extended.Content.Pipeline.Tests
{
    [TestFixture]
    public class AstridAnimatorProcessorTests
    {
        [Test]
        public void AstridAnimatorProcessor_Process_Test()
        {
            var filePath = PathExtensions.GetApplicationFullPath("TestData", "astrid-animator.aa");

            var importer = new AstridAnimatorImporter();
            var importerResult = importer.Import(filePath, null);

            var processor = new AstridAnimatorProcessor();
            var result = processor.Process(importerResult, null);

            Assert.AreEqual("astrid-animator-atlas", result.TextureAtlasAssetName);
            Assert.AreEqual("TestData", Path.GetFileName(result.Directory));
            Assert.AreEqual(3, result.Frames.Count);
            Assert.AreEqual("appear_01", result.Frames[0]);
            Assert.AreEqual("appear_02", result.Frames[1]);
            Assert.AreEqual("die_01", result.Frames[2]);
        }
    }
}