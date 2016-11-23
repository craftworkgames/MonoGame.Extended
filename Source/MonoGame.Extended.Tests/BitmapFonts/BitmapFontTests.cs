using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using NUnit.Framework;

namespace MonoGame.Extended.Tests.BitmapFonts
{
    [TestFixture]
    public class BitmapFontTests
    {
        [Test]
        public void BitmapFont_MeasureString_Test()
        {
            var graphicsDevice = new TestGraphicsDevice();
            var texture = new Texture2D(graphicsDevice, 17, 20);
            var regions = new[]
            {
                //  <char id="66" x="219" y="61" width="16" height="18" xoffset="2" yoffset="9" xadvance="19" page="0" chnl="15" />
                new BitmapFontRegion(new TextureRegion2D(texture, 219, 61, 16, 18), 'B', 2, 9, 19)
            };

            var font = new BitmapFont("fontName", regions, lineHeight: 50);
            var size = font.MeasureString("B");

            Assert.That(size.Width, Is.EqualTo(regions[0].Width));
            Assert.That(size.Height, Is.EqualTo(regions[0].Height));
        }
    }
}
