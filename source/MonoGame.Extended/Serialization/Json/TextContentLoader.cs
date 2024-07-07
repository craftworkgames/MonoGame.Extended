using System.IO;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;

namespace MonoGame.Extended.Serialization.Json
{
    public class TextContentLoader : IContentLoader<string>
    {
        public string Load(ContentManager contentManager, string path)
        {
            using (var stream = contentManager.OpenStream(path))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
