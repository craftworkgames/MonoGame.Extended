using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;

namespace MonoGame.Extended.Content.Pipeline.TextureAtlases
{
    [ContentImporter(".json", DefaultProcessor = "TexturePackerProcessor", DisplayName = "TexturePacker JSON Importer - MonoGame.Extended")]
    public class TexturePackerJsonImporter : ContentImporter<TexturePackerFile>
    {
        public override TexturePackerFile Import(string filename, ContentImporterContext context)
        {
            using (var streamReader = new StreamReader(filename))
            {
                var serializer = new JsonSerializer();
                return (TexturePackerFile) serializer.Deserialize(streamReader, typeof(TexturePackerFile));
            }
        }
    }
}
