using NUnit.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.Tests.TextureAtlases
{
    [TestFixture]
    public class TextureRegion2DTests
    {
        private Game _game;

        [SetUp]
        public void RunBeforeAnyTests()
        {
            _game = TestHelper.CreateGame();
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            _game.Dispose();
        }

        [Test]
        public void TextureRegion2D_FromTexture_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = new Texture2D(graphicsDevice, 100, 200);
            var textureRegion = new TextureRegion2D(texture);

            Assert.AreSame(texture, textureRegion.Texture);
            Assert.AreEqual(0, textureRegion.X);
            Assert.AreEqual(0, textureRegion.Y);
            Assert.AreEqual(100, textureRegion.Width);
            Assert.AreEqual(200, textureRegion.Height);
            Assert.IsNull(textureRegion.Tag);
        }

        [Test]
        public void TextureRegion2D_Specified_Test()
        {
            var graphicsDevice = _game.GraphicsDevice;
            var texture = new Texture2D(graphicsDevice, 100, 200);
            var textureRegion = new TextureRegion2D(texture, 10, 20, 30, 40);

            Assert.AreSame(texture, textureRegion.Texture);
            Assert.AreEqual(10, textureRegion.X);
            Assert.AreEqual(20, textureRegion.Y);
            Assert.AreEqual(30, textureRegion.Width);
            Assert.AreEqual(40, textureRegion.Height);
            Assert.IsNull(textureRegion.Tag);
        }
    }
}