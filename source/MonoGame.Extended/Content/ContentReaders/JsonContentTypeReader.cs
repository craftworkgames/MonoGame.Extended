using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Content;


namespace MonoGame.Extended.Content.ContentReaders
{
    public class JsonContentTypeReader<T> : ContentTypeReader<T>
    {
        protected override T Read(ContentReader reader, T existingInstance)
        {
            var json = reader.ReadString();
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
