using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.Tests.BitmapFonts
{
    public class BitmapFontTests
    {
        [Fact]
        public void BitmapFont_Constructor_Test()
        {
            var font = CreateTestFont();

            Assert.Equal("Impact", font.Face);
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
            //var size = font.MeasureString("box fox\nbox of fox");
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

        //  Test added for issue #695
        //  https://github.com/craftworkgames/MonoGame.Extended/issues/695
        //
        //  Issue claims measure string does not account for space at the end of string.
        [Fact]
        public void BitmapFont_MeasureString_SpaceAtEnd_Test()
        {
            var font = CreateTestFont();

            var noSpaceAtEnd = font.MeasureString("bfox");
            var spaceAtEnd = font.MeasureString("bfox ");

            Assert.NotEqual(noSpaceAtEnd, spaceAtEnd);

        }

        private static BitmapFont CreateTestFont()
        {
            var textureRegion = new Texture2DRegion("Test Font", x: 219, y: 61, width: 16, height: 18);
            var regions = new[]
            {
                // extracted from 'Impact' font. 'x' is particularly interesting because it has a negative x offset
                new BitmapFontCharacter(textureRegion: textureRegion, character: ' ', xOffset: 0, yOffset: 0, xAdvance: 6),
                new BitmapFontCharacter(textureRegion: textureRegion, character: 'b', xOffset: 0, yOffset: 7, xAdvance: 17),
                new BitmapFontCharacter(textureRegion: textureRegion, character: 'f', xOffset: 0, yOffset: 7, xAdvance: 9),
                new BitmapFontCharacter(textureRegion: textureRegion, character: 'o', xOffset: 0, yOffset: 11, xAdvance: 16),
                new BitmapFontCharacter(textureRegion: textureRegion, character: 'x', xOffset: -1, yOffset: 11, xAdvance: 13),
            };

            var font = new BitmapFont("Impact", size: 32, lineHeight: 22, regions);
            return font;
        }
    }
}
