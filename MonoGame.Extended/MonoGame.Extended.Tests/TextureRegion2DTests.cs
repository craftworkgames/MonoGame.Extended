using Microsoft.Xna.Framework.Graphics;
using NSubstitute;
using NUnit.Framework;

namespace MonoGame.Extended.Tests
{
    [TestFixture]
    public class TextureRegion2DTests
    {
        [Test]
        public void TextureRegion2D_ConstructedWithDimensions_Test()
        {
            var graphicsDevice = TestHelper.CreateGraphicsDevice();
            var texture = Substitute.For<Texture2D>(graphicsDevice, 100, 100);
            var textureRegion = new TextureRegion2D(texture, 10, 20, 30, 40)
            {
                Tag = "tag"
            };

            Assert.AreSame(texture, textureRegion.Texture);
            Assert.AreEqual(10, textureRegion.X);
            Assert.AreEqual(20, textureRegion.Y);
            Assert.AreEqual(30, textureRegion.Width);
            Assert.AreEqual(40, textureRegion.Height);
            Assert.AreEqual("tag", textureRegion.Tag);
        }
    }
}
