using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline.Json
{
    [ContentImporter(".json", DefaultProcessor = nameof(JsonContentProcessor), DisplayName = "JSON Importer - MonoGame.Extended")]
    public class JsonContentImporter : ContentImporter<ContentImporterResult<string>>
    {
        public override ContentImporterResult<string> Import(string filename, ContentImporterContext context)
        {
            var json = File.ReadAllText(filename);
            return new ContentImporterResult<string>(filename, json);
        }
    }
}
