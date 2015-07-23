using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFontReader : ContentTypeReader<BitmapFont>
    {
        protected override BitmapFont Read(ContentReader input, BitmapFont existingInstance)
        {
            var json = input.ReadString();
            var bitmapFont = JsonConvert.DeserializeObject<BitmapFont>(json);
            bitmapFont.Texture = input.ContentManager.Load<Texture2D>(bitmapFont.TextureFilename);
            return bitmapFont;
        }
    }
}