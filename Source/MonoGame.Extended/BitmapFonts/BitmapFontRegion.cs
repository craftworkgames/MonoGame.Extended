using System;
using MonoGame.Extended.TextureAtlases;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFontRegion
    {
        public BitmapFontRegion(TextureRegion2D textureRegion, int character, int xOffset, int yOffset, int xAdvance)
        {
            TextureRegion = textureRegion;
            Character = character;
            XOffset = xOffset;
            YOffset = yOffset;
            XAdvance = xAdvance;
        }

        public int Character { get; }
        public TextureRegion2D TextureRegion { get; }
        public int XOffset { get; }
        public int YOffset { get; }
        public int XAdvance { get; }
        public int Width => TextureRegion.Width;
        public int Height => TextureRegion.Height;

        public override string ToString()
        {
            return $"{Convert.ToChar(Character)} {TextureRegion}";
        }
    }
}