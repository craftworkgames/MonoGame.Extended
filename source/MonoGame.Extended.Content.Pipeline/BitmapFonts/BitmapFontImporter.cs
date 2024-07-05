using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.BitmapFonts;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
    [ContentImporter(".fnt", DefaultProcessor = "BitmapFontProcessor", DisplayName = "BMFont Importer - MonoGame.Extended")]
    public class BitmapFontImporter : ContentImporter<ContentImporterResult<BitmapFontFileContent>>
    {
        public override ContentImporterResult<BitmapFontFileContent> Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("Importing FNT file: {0}", filename);

            using FileStream stream = File.OpenRead(filename);
            var bmfFile = BitmapFontFileReader.Read(stream);
            return new ContentImporterResult<BitmapFontFileContent>(filename, bmfFile);
        }
    }
}
