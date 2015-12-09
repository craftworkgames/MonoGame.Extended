using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json;

namespace MonoGame.Extended.Content.Pipeline
{
    public abstract class JsonContentImporter<T> : ContentImporter<T>
    {
        public override T Import(string filename, ContentImporterContext context)
        {
            using (var streamReader = new StreamReader(filename))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<T>(jsonReader);
            }
        }
    }
}