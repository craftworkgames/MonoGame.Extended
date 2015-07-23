using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.BitmapFonts;

namespace MonoGame.Extended.Content.Pipeline
{
    [ContentImporter(".fnt", DefaultProcessor = "BitmapFontProcessor", DisplayName = "BMFont Importer - MonoGame.Extended")]
    public class BitmapFontImporter : ContentImporter<FontFile>
    {
        public override FontFile Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("Importing XML file: {0}", filename);

            using (var streamReader = new StreamReader(filename))
            {
                var deserializer = new XmlSerializer(typeof(FontFile));
                return (FontFile)deserializer.Deserialize(streamReader);
            }
        }
    }
}