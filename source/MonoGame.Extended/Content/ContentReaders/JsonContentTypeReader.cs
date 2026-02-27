using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Content;


namespace MonoGame.Extended.Content.ContentReaders
{
    public class JsonContentTypeReader<T> : ContentTypeReader<T>
    {
#if !FNA && !KNI
        /// <summary>
        /// Registers <see cref="JsonContentTypeReader{T}"/> for the type <typeparamref name="T"/> with the
        /// <see cref="ContentTypeReaderManager"/> so it is resolved without reflection.
        /// </summary>
        /// <remarks>
        /// Call this method once per concrete type during application startup when publishing with
        /// <c>PublishAot</c> or <c>PublishTrimmed</c>. For example:
        /// <code>
        /// JsonContentTypeReader&lt;MyData&gt;.Register();
        /// </code>
        /// </remarks>
        public static void Register() =>
            ContentTypeReaderManager.AddTypeCreator(
                typeof(JsonContentTypeReader<T>).AssemblyQualifiedName,
                () => new JsonContentTypeReader<T>());
#endif

        protected override T Read(ContentReader reader, T existingInstance)
        {
            var json = reader.ReadString();
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
