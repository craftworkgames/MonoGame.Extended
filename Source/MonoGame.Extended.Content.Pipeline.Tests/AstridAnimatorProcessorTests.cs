using System.Collections.Generic;
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
            var input  = new AstridAnimatorFile
            {
                TextureAtlas = "astrid-animator-atlas.json",
                Animations = new List<AstridAnimatorAnimation>
                {
                    new AstridAnimatorAnimation("animation1", 8)
                    {
                        Frames = new List<string>
                        {
                            "frame01",
                            "frame02"
                        }
                    }
                }
            };
            var processor = new AstridAnimatorProcessor();
            var result = processor.Process(input, null);

            Assert.IsNotNull(result);
        }
    }
}