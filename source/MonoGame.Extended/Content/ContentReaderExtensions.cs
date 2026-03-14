using System;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace MonoGame.Extended.Content
{
    public static class ContentReaderExtensions
    {
		
#if FNA
		public static GraphicsDevice GetGraphicsDevice(this ContentReader contentReader)
		{
			var serviceProvider = contentReader.ContentManager.ServiceProvider;
			var graphicsDeviceService = serviceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
				   
			if (graphicsDeviceService == null)
			{
				throw new InvalidOperationException("No Graphics Device Service");
			}

			return graphicsDeviceService.GraphicsDevice;
		}
#endif

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

    }
}
