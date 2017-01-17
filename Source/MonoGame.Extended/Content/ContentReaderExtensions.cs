using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Content
{
    public static class ContentReaderExtensions
    {
        private static readonly FieldInfo ContentReaderGraphicsDeviceFieldInfo = typeof(ContentReader).GetTypeInfo().GetDeclaredField("graphicsDevice");
        private static byte[] _scratchBuffer;

        public static GraphicsDevice GetGraphicsDevice(this ContentReader contentReader)
        {
            return (GraphicsDevice)ContentReaderGraphicsDeviceFieldInfo.GetValue(contentReader);
        }

        public static string GetRelativeAssetPath(this ContentReader contentReader, string relativePath)
        {
            var assetName = contentReader.AssetName;
            var assetNodes = assetName.Split(new[] {'/', '\\'}, StringSplitOptions.RemoveEmptyEntries);
            var relativeNodes = relativePath.Split(new[] {'/', '\\'}, StringSplitOptions.RemoveEmptyEntries);
            var relativeIndex = assetNodes.Length - 1;
            var newPathNodes = new List<string>();

            foreach (var relativeNode in relativeNodes)
            {
                if (relativeNode == "..")
                    relativeIndex--;
                else
                    newPathNodes.Add(relativeNode);
            }

            var values = assetNodes
                .Take(relativeIndex)
                .Concat(newPathNodes)
                .ToArray();

            return string.Join("/", values);
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