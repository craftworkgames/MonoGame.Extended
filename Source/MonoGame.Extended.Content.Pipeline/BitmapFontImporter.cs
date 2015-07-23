using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Content.Pipeline
{
    [ContentImporter(".fnt", DefaultProcessor = "BitmapFontProcessor", DisplayName = "Pixel Shader Importer")]
    public class BitmapFontImporter : ContentImporter<BitmapFontIn>
    {
        public override BitmapFontIn Import(string filename, ContentImporterContext context)
        {
            return new BitmapFontIn();
        }
    }
}