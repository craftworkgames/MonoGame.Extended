using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline.Animations
{
    [ContentImporter(".aa", DefaultProcessor = "AstridAnimatorProcessor",
         DisplayName = "Astrid Animator Importer - MonoGame.Extended")]
    public class AstridAnimatorImporter : ContentImporter<ContentImporterResult<AstridAnimatorFile>>
    {
        public override ContentImporterResult<AstridAnimatorFile> Import(string filename, ContentImporterContext context)
        {
            var json = File.ReadAllText(filename);
            var data = JsonSerializer.Deserialize<AstridAnimatorFile>(json);
            return new ContentImporterResult<AstridAnimatorFile>(filename, data);
        }
    }
}
