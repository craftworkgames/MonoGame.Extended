using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline
{
    [ContentProcessor(DisplayName = "BitmapFont Processor")]
    public class BitmapFontProcessor : ContentProcessor<BitmapFontIn, BitmapFontOut>
    {
        public override BitmapFontOut Process(BitmapFontIn input, ContentProcessorContext context)
        {
            return new BitmapFontOut();
        }
    }
}