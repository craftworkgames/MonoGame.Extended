using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;

namespace MonoGame.Extended.Content.Pipeline.Animations
{
    [ContentImporter(".aa", DefaultProcessor = "AstridAnimatorProcessor",
         DisplayName = "Astrid Animator Importer - MonoGame.Extended")]
    public class AstridAnimatorImporter : ContentImporter<ContentImporterResult<AstridAnimatorFile>>
    {
        public override ContentImporterResult<AstridAnimatorFile> Import(string filename, ContentImporterContext context)
        {
            using (var streamReader = new StreamReader(filename))
            {
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();
                    var data = serializer.Deserialize<AstridAnimatorFile>(jsonReader);
                    return new ContentImporterResult<AstridAnimatorFile>(filename, data);
                }
            }
        }
    }
}