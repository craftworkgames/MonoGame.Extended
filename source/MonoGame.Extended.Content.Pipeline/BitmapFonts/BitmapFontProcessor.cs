using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.BitmapFonts.BmfTypes;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
    [ContentProcessor(DisplayName = "BMFont Processor - MonoGame.Extended")]
    public class BitmapFontProcessor : ContentProcessor<ContentImporterResult<BmfFile>, BitmapFontProcessorResult>
    {
        public override BitmapFontProcessorResult Process(ContentImporterResult<BmfFile> importerResult, ContentProcessorContext context)
        {
            try
            {
                BmfFile bmfFile = importerResult.Data;
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
