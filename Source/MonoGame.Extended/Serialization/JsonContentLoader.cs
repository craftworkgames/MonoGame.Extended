using System.IO;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;
using Newtonsoft.Json;

namespace MonoGame.Extended.Serialization
{
    public class JsonContentLoader : IContentLoader
    {
        public T Load<T>(ContentManager contentManager, string path)
        {
            using (var stream = contentManager.OpenStream(path))
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var serializer = new MonoGameJsonSerializer(contentManager, path);
                return serializer.Deserialize<T>(jsonReader);
            }
        }
    }
}