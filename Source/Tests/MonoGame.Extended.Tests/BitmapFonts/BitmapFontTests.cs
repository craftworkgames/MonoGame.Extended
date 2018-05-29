using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using Xunit;

namespace MonoGame.Extended.Tests.BitmapFonts
{
    public class BitmapFontTests
    {
        [Fact]
        public void BitmapFont_Constructor_Test()
        {
            var font = CreateTestFont();

            Assert.Equal("Impact", font.Name);
            Assert.Equal(22, font.LineHeight);
        }

        [Fact]
        public void BitmapFont_MeasureString_SingleWord_Test()
        {
            var font = CreateTestFont();
            var size = font.MeasureString("fox");

            Assert.Equal(40, size.Width);
            Assert.Equal(font.LineHeight, size.Height);
        }

        [Fact]
        public void BitmapFont_MeasureString_WithLetterSpacing_Test()
        {
            var font = CreateTestFont();
            font.LetterSpacing = 3;

            var size = font.MeasureString("fox");

            Assert.Equal(46, size.Width);
            Assert.Equal(size.Height, font.LineHeight);
        }

        [Fact]
        public void BitmapFont_MeasureString_MultipleLines_Test()
        {
            var font = CreateTestFont();
            var size = font.MeasureString("box fox\nbox of fox");

            Assert.Equal(123, size.Width);
            Assert.Equal(size.Height, font.LineHeight * 2);
        }

        [Fact]
        public void BitmapFont_MeasureString_EmptyString_Test()
        {
            var font = CreateTestFont();
            var size = font.MeasureString(string.Empty);

            Assert.Equal(0, size.Width);
            Assert.Equal(0, size.Height);
        }

        private static BitmapFont CreateTestFont()
        {
            var textureRegion = new TextureRegion2D(null, x: 219, y: 61, width: 16, height: 18);
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
