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
            var texture = new Texture2D(graphicsDevice, width: 17, height: 20);
            var textureRegion = new TextureRegion2D(texture, x: 219, y: 61, width: 16, height: 18);
            var regions = new[]
            {
                //  <char id="66" x="219" y="61" width="16" height="18" xoffset="2" yoffset="9" xadvance="19" page="0" chnl="15" />
                new BitmapFontRegion(textureRegion, character: 'B', xOffset: 2, yOffset: 9, xAdvance: 19)
            };

            var font = new BitmapFont("fontName", regions, lineHeight: 22);
            var size = font.MeasureString("B");

            var region = regions[0];

            Assert.That(size.Width, Is.EqualTo(region.Width + region.XOffset));
            Assert.That(size.Height, Is.EqualTo(font.LineHeight));
        }
    }
}
