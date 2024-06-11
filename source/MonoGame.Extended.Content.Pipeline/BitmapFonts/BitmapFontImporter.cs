using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.BitmapFonts.BmfTypes;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
    [ContentImporter(".fnt", DefaultProcessor = "BitmapFontProcessor",
         DisplayName = "BMFont Importer - MonoGame.Extended")]
    public class BitmapFontImporter : ContentImporter<ContentImporterResult<BmfFile>>
    {
        public override ContentImporterResult<BmfFile> Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("Importing FNT file: {0}", filename);

            using FileStream stream = File.OpenRead(filename);
            BmfFile file = BmfFile.FromStream(stream);
            return new ContentImporterResult<BmfFile>(filename, file);
        }
    }
}
