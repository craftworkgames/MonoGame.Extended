using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

namespace MonoGame.Extended.Content
{
    public static class ContentReaderExtensions
    {
        private static readonly FieldInfo _contentReaderGraphicsDeviceFieldInfo = typeof(ContentReader).GetTypeInfo().GetDeclaredField("graphicsDevice");

        public static GraphicsDevice GetGraphicsDevice(this ContentReader contentReader)
        {
            return (GraphicsDevice)_contentReaderGraphicsDeviceFieldInfo.GetValue(contentReader);
        }

        public static string RemoveExtension(string path)
        {
            return Path.ChangeExtension(path, null).TrimEnd('.');
        }

        public static string GetRelativeAssetName(this ContentReader contentReader, string relativeName)
        {
            var assetDirectory = Path.GetDirectoryName(contentReader.AssetName);
            var assetName = RemoveExtension(Path.Combine(assetDirectory, relativeName).Replace('\\', '/'));

            return ShortenRelativePath(assetName);
        }

        public static string ShortenRelativePath(string relativePath)
        {
            var ellipseIndex = relativePath.IndexOf("/../", StringComparison.Ordinal);
            while (ellipseIndex != -1)
            {
                var lastDirectoryIndex = relativePath.LastIndexOf('/', ellipseIndex - 1) + 1;
                relativePath = relativePath.Remove(lastDirectoryIndex, ellipseIndex + 4 - lastDirectoryIndex);
                ellipseIndex = relativePath.IndexOf("/../", StringComparison.Ordinal);
            }

            return relativePath;
        }

        public static void ReadTiledMapProperties(this ContentReader reader, TiledMapProperties properties)
        {
            var count = reader.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                var key = reader.ReadString();
                var value = new TiledMapPropertyValue(reader.ReadString());
                ReadTiledMapProperties(reader, value.Properties);
                properties[key] = value;
            }
        }
    }
}
