using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Content
{
    public static class ContentReaderExtensions
    {
        private static readonly FieldInfo _contentReaderGraphicsDeviceFieldInfo = typeof(ContentReader).GetTypeInfo().GetDeclaredField("graphicsDevice");
        private static byte[] _scratchBuffer;

        public static GraphicsDevice GetGraphicsDevice(this ContentReader contentReader)
        {
            return (GraphicsDevice)_contentReaderGraphicsDeviceFieldInfo.GetValue(contentReader);
        }

        public static string GetRelativeAssetName(this ContentReader contentReader, string relativeName)
        {
            var assetDirectory = Path.GetDirectoryName(contentReader.AssetName);
            var assetName = Path.Combine(assetDirectory, relativeName).Replace('\\', '/');

            var ellipseIndex = assetName.IndexOf("/../", StringComparison.Ordinal);
            while (ellipseIndex != -1)
            {
                var lastDirectoryIndex = assetName.LastIndexOf('/', ellipseIndex - 1);
                if (lastDirectoryIndex == -1)
                    lastDirectoryIndex = 0;
                assetName = assetName.Remove(lastDirectoryIndex, ellipseIndex + 4);
                ellipseIndex = assetName.IndexOf("/../", StringComparison.Ordinal);
            }

            return assetName;
        }

        internal static byte[] GetScratchBuffer(this ContentReader contentReader, int size)
        {
            size = Math.Max(size, 1024 * 1024);
            if (_scratchBuffer == null || _scratchBuffer.Length < size)
                _scratchBuffer = new byte[size];
            return _scratchBuffer;
        }
    }
}