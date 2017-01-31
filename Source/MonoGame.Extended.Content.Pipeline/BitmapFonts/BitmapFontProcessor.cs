using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
    [ContentProcessor(DisplayName = "BMFont Processor - MonoGame.Extended")]
    public class BitmapFontProcessor : ContentProcessor<BitmapFontFile, BitmapFontProcessorResult>
    {
        public override BitmapFontProcessorResult Process(BitmapFontFile bitmapFontFile, ContentProcessorContext context)
        {
            try
            {
                context.Logger.LogMessage("Processing BMFont");
                var result = new BitmapFontProcessorResult(bitmapFontFile);

                foreach (var fontPage in bitmapFontFile.Pages)
                {
                    var assetName = Path.GetFileNameWithoutExtension(fontPage.File);
                    context.Logger.LogMessage("Expected texture asset: {0}", assetName);
                    result.TextureAssets.Add(assetName);
                }

                return result;
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }
    }
}