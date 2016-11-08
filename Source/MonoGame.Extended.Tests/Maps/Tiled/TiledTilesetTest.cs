using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.TextureAtlases;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.Maps.Tiled
{
    [TestFixture]
    public class TiledTilesetTest
    {
        [Test]
        public void GetTileRegion_BlankTile()
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            Texture2D texture = new Texture2D(gd, 64, 64);

            TiledTileset ts = new TiledTileset(texture, 10, 32, 32, 4, 0, 0);

            Assert.IsNull(ts.GetTileRegion(0));
        }

        [Test]
        [TestCase(9, Result = false, Description = "Too low")]
        [TestCase(10, Result = true, Description = "Min tile")]
        [TestCase(11, Result = true, Description = "Middle tile")]
        [TestCase(13, Result = true, Description = "Last tile")]
        [TestCase(14, Result = false, Description = "Too high")]
        public bool ContainsTileId(int id)
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            Texture2D texture = new Texture2D(gd, 64, 64);

            TiledTileset ts = new TiledTileset(texture, 10, 32, 32, 4, 0, 0);

            return ts.ContainsTileId(id);
        }

        [Test]
        public void Constructor_NoMargin([Values(0, 2)]int spacing)
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            Texture2D texture = new Texture2D(gd, 64, 64);

            TiledTileset ts = new TiledTileset(texture, 1, 32, 32, 4, spacing, 0);

            TextureRegion2D r = ts.GetTileRegion(1);
            Assert.AreEqual(texture, r.Texture);
            Assert.AreEqual(0, r.X);
            Assert.AreEqual(0, r.Y);
            Assert.AreEqual(32, r.Width);
            Assert.AreEqual(32, r.Height);

            r = ts.GetTileRegion(2);
            Assert.AreEqual(texture, r.Texture);
            Assert.AreEqual(spacing + 32, r.X);
            Assert.AreEqual(0, r.Y);
            Assert.AreEqual(32, r.Width);
            Assert.AreEqual(32, r.Height);

            r = ts.GetTileRegion(3);
            Assert.AreEqual(texture, r.Texture);
            Assert.AreEqual(0, r.X);
            Assert.AreEqual(spacing + 32, r.Y);
            Assert.AreEqual(32, r.Width);
            Assert.AreEqual(32, r.Height);

            r = ts.GetTileRegion(4);
            Assert.AreEqual(texture, r.Texture);
            Assert.AreEqual(spacing + 32, r.X);
            Assert.AreEqual(spacing + 32, r.Y);
            Assert.AreEqual(32, r.Width);
            Assert.AreEqual(32, r.Height);
        }

        [Test]
        public void Constructor_NoSpacing([Values(0, 2)]int margin)
        {
            GraphicsDevice gd = TestHelper.CreateGraphicsDevice();
            Texture2D texture = new Texture2D(gd, 64, 64);

            TiledTileset ts = new TiledTileset(texture, 1, 32, 32, 4, 0, margin);

            TextureRegion2D r = ts.GetTileRegion(1);
            Assert.AreEqual(texture, r.Texture);
            Assert.AreEqual(margin, r.X);
            Assert.AreEqual(margin, r.Y);
            Assert.AreEqual(32, r.Width);
            Assert.AreEqual(32, r.Height);

            r = ts.GetTileRegion(2);
            Assert.AreEqual(texture, r.Texture);
            Assert.AreEqual(margin + 32, r.X);
            Assert.AreEqual(margin, r.Y);
            Assert.AreEqual(32, r.Width);
            Assert.AreEqual(32, r.Height);

            r = ts.GetTileRegion(3);
            Assert.AreEqual(texture, r.Texture);
            Assert.AreEqual(margin, r.X);
            Assert.AreEqual(margin + 32, r.Y);
            Assert.AreEqual(32, r.Width);
            Assert.AreEqual(32, r.Height);

            r = ts.GetTileRegion(4);
            Assert.AreEqual(texture, r.Texture);
            Assert.AreEqual(margin + 32, r.X);
            Assert.AreEqual(margin + 32, r.Y);
            Assert.AreEqual(32, r.Width);
            Assert.AreEqual(32, r.Height);
        }
    }
}
