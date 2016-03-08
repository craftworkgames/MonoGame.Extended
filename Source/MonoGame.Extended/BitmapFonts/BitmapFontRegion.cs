using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFontRegion
    {
        public char Character { get; set; }

        public int Height => TextureRegion.Height;
        public TextureRegion2D TextureRegion { get; }

        public int Width => TextureRegion.Width;
        public int XAdvance { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        public BitmapFontRegion(TextureRegion2D textureRegion, char character, int xOffset, int yOffset, int xAdvance)
        {
            TextureRegion = textureRegion;
            Character = character;
            XOffset = xOffset;
            YOffset = yOffset;
            XAdvance = xAdvance;
        }
    }
}