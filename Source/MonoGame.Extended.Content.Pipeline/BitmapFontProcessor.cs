using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.BitmapFonts;

namespace MonoGame.Extended.Content.Pipeline
{
    [ContentProcessor(DisplayName = "BMFont Processor - MonoGame.Extended")]
    public class BitmapFontProcessor : ContentProcessor<FontFile, BitmapFont>
    {
        public override BitmapFont Process(FontFile input, ContentProcessorContext context)
        {
            try
            {
                context.Logger.LogMessage("Processing BMFont");
                
                var fileName = input.Pages[0].File;
                context.Logger.LogMessage("fileName {0}", fileName);

                var assetName = Path.GetFileNameWithoutExtension(fileName);
                context.Logger.LogMessage("assetName {0}", assetName);

                return new BitmapFont(input)
                {
                    TextureFilename = assetName
                };
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }
    }
}