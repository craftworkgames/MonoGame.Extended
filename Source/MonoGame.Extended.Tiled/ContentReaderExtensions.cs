using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tiled
{
    public static class ContentReaderExtensions
    {
        public static void ReadTiledMapProperties(this ContentReader reader, TiledMapProperties properties)
        {
            var count = reader.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                var key = reader.ReadString();
                var value = reader.ReadString();
                properties[key] = value;
            }
        }
    }
}
