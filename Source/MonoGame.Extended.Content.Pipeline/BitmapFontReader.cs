using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Content.Pipeline
{
    public class BitmapFontReader : ContentTypeReader<BitmapFontOut>
    {
        protected override BitmapFontOut Read(ContentReader input, BitmapFontOut existingInstance)
        {
            return existingInstance;
        }
    }
}