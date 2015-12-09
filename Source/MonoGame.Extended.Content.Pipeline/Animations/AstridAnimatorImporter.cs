using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;

namespace MonoGame.Extended.Content.Pipeline.Animations
{
    public class ContentImporterResult<T>
    {
        public ContentImporterResult(string filePath, T data)
        {
            FilePath = filePath;
            Data = data;
        }

        public string FilePath { get; private set; }
        public T Data { get; private set; }
    }

    [ContentImporter(".aa", DefaultProcessor = "AstridAnimatorProcessor", DisplayName = "Astrid Animator Importer - MonoGame.Extended")]
    public class AstridAnimatorImporter : ContentImporter<ContentImporterResult<AstridAnimatorFile>>
    {
        public override ContentImporterResult<AstridAnimatorFile> Import(string filename, ContentImporterContext context)
        {
            using (var streamReader = new StreamReader(filename))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = new JsonSerializer();
                var data = serializer.Deserialize<AstridAnimatorFile>(jsonReader);
                return new ContentImporterResult<AstridAnimatorFile>(filename, data);
            }
        }
    }
}