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
        public void BitmapFont_Constructor_Test()
        {
            var font = CreateTestFont();

            Assert.That(font.Name, Is.EqualTo("Impact"));
            Assert.That(font.LineHeight, Is.EqualTo(22));
        }

        [Test]
        public void BitmapFont_MeasureString_SingleWord_Test()
        {
            var font = CreateTestFont();
            var size = font.MeasureString("fox");

            Assert.That(size.Width, Is.EqualTo(40));
            Assert.That(size.Height, Is.EqualTo(font.LineHeight));
        }

        [Test]
        public void BitmapFont_MeasureString_WithLetterSpacing_Test()
        {
            var font = CreateTestFont();
            font.LetterSpacing = 3;

            var size = font.MeasureString("fox");

            Assert.That(size.Width, Is.EqualTo(46));
            Assert.That(size.Height, Is.EqualTo(font.LineHeight));
        }

        [Test]
        public void BitmapFont_MeasureString_MultipleLines_Test()
        {
            var font = CreateTestFont();
            var size = font.MeasureString("box fox\nbox of fox");

            Assert.That(size.Width, Is.EqualTo(123));
            Assert.That(size.Height, Is.EqualTo(font.LineHeight * 2));
        }

        [Test]
        public void BitmapFont_MeasureString_EmptyString_Test()
        {
            var font = CreateTestFont();
            var size = font.MeasureString(string.Empty);

            Assert.That(size.Width, Is.EqualTo(0));
            Assert.That(size.Height, Is.EqualTo(0));
        }

        private static BitmapFont CreateTestFont()
        {
            var graphicsDevice = new TestGraphicsDevice();
            var texture = new Texture2D(graphicsDevice, width: 256, height: 256);
            var textureRegion = new TextureRegion2D(texture, x: 219, y: 61, width: 16, height: 18);
            var regions = new[]
            {
                // extracted from 'Impact' font. 'x' is particularly interesting because it has a negative x offset
                new BitmapFontRegion(textureRegion, character: ' ', xOffset: 0, yOffset: 0, xAdvance: 6),
                new BitmapFontRegion(textureRegion, character: 'b', xOffset: 0, yOffset: 7, xAdvance: 17),
                new BitmapFontRegion(textureRegion, character: 'f', xOffset: 0, yOffset: 7, xAdvance: 9),
                new BitmapFontRegion(textureRegion, character: 'o', xOffset: 0, yOffset: 11, xAdvance: 16),
                new BitmapFontRegion(textureRegion, character: 'x', xOffset: -1, yOffset: 11, xAdvance: 13),
            };

            var font = new BitmapFont("Impact", regions, lineHeight: 22);
            return font;
        }
    }
}
