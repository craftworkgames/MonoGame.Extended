using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.BitmapFonts;
using Newtonsoft.Json;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
    [ContentProcessor(DisplayName = "BMFont Processor - MonoGame.Extended")]
    public class BitmapFontProcessor : ContentProcessor<FontFile, FileFileData>
    {
        public override FileFileData Process(FontFile input, ContentProcessorContext context)
        {
            try
            {
                context.Logger.LogMessage("Processing BMFont");
                var json = JsonConvert.SerializeObject(input);
                var output = new FileFileData(json);

                foreach (var fontPage in input.Pages)
                {
                    var assetName = Path.GetFileNameWithoutExtension(fontPage.File);
                    context.Logger.LogMessage("Expected texture asset: {0}", assetName);
                    output.TextureAssets.Add(assetName);
                }

                return output;

            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }
    }
}