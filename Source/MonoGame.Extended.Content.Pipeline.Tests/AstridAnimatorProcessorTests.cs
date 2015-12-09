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
            const string filePath = @"TestData\astrid-animator.aa";

            var importer = new AstridAnimatorImporter();
            var importerResult = importer.Import(filePath, null);

            var processor = new AstridAnimatorProcessor();
            var result = processor.Process(importerResult, null);

            Assert.AreEqual("", result.TextureAtlasAssetName);
            Assert.AreEqual("", result.Directory);
            Assert.IsNotNull(result);
        }
    }
}