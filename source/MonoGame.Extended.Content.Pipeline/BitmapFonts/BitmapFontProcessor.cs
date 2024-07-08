using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.BitmapFonts;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
    [ContentProcessor(DisplayName = "BMFont Processor - MonoGame.Extended")]
    public class BitmapFontProcessor : ContentProcessor<ContentImporterResult<BitmapFontFileContent>, BitmapFontProcessorResult>
    {
        public override BitmapFontProcessorResult Process(ContentImporterResult<BitmapFontFileContent> importerResult, ContentProcessorContext context)
        {
            try
            {
                BitmapFontFileContent bmfFile = importerResult.Data;
                context.Logger.LogMessage("Processing BMFont");
                var result = new BitmapFontProcessorResult(bmfFile);

                foreach (var fontPage in bmfFile.Pages)
                {
                    var assetName = Path.GetFileNameWithoutExtension(fontPage);
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
