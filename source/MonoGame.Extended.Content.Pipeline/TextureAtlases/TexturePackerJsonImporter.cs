using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.TextureAtlases;


namespace MonoGame.Extended.Content.Pipeline.TextureAtlases
{
    [ContentImporter(".json", DefaultProcessor = "TexturePackerProcessor", DisplayName = "TexturePacker JSON Importer - MonoGame.Extended")]
    public class TexturePackerJsonImporter : ContentImporter<TexturePackerFile>
    {
        public override TexturePackerFile Import(string filename, ContentImporterContext context)
        {
            var json = File.ReadAllText(filename);
            return JsonSerializer.Deserialize<TexturePackerFile>(json);
        }
    }
}
