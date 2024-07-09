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
                var result = new BitmapFontProcessorResult(bmfFile);

                foreach (var page in bmfFile.Pages)
                {
                    context.AddDependency(Path.GetFileName(page));
                    result.TextureAssets.Add(Path.GetFileNameWithoutExtension(page));
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
