using System;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
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
            var typeInfo = (JsonTypeInfo<T>)monoGameSerializerOptions.GetTypeInfo(typeof(T));

            return JsonSerializer.Deserialize(stream, typeInfo)!;
        }
    }

    public class JsonContentLoader<T> : IContentLoader<T>
    {
        private readonly JsonTypeInfo<T> _typeInfo;

        public JsonContentLoader(JsonTypeInfo<T> typeInfo)
        {
            ArgumentNullException.ThrowIfNull(typeInfo);
            _typeInfo = typeInfo;
        }

        public T Load(ContentManager contentManager, string path)
        {
            using var stream = contentManager.OpenStream(path);

            return JsonSerializer.Deserialize(stream, _typeInfo);
        }
    }
}
