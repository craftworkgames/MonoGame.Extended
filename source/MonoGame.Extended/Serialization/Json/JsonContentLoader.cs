using System.Text.Json;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;


namespace MonoGame.Extended.Serialization.Json
{
    public class JsonContentLoader : IContentLoader
    {
        public T Load<T>(ContentManager contentManager, string path)
        {

            using var stream = contentManager.OpenStream(path);
            var monoGameSerializerOptions = MonoGameJsonSerializerOptionsProvider.GetOptions(contentManager, path);
            return JsonSerializer.Deserialize<T>(stream, monoGameSerializerOptions);
        }
    }
}
