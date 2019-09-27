using System.IO;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class JsonContentTypeReader<T> : ContentTypeReader<T>
    {
        protected override T Read(ContentReader reader, T existingInstance)
        {
            var json = reader.ReadString();

            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<T>(jsonReader);
            }
        }
    }
}