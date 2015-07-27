using MonoGame.Extended.Graphics;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFontRegion
    {
        public BitmapFontRegion(TextureRegion2D textureRegion, BitmapFontChar fontCharacter)
        {
            TextureRegion = textureRegion;
            FontCharacter = fontCharacter;
        }

        public TextureRegion2D TextureRegion { get; private set; }
        public BitmapFontChar FontCharacter { get; private set; }
    }
}