using System.IO;
using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Content
{
    public abstract class ContentLoader<T>
    {
        protected ContentLoader()
        {
        }

        public abstract T LoadContentFromStream(ContentManager contentManager, Stream stream);
    }
}