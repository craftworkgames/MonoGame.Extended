using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Content
{
    public static class ContentManagerExtensions
    {
        public const string DirectorySeparatorChar = "/";

        public static Stream OpenStream(this ContentManager contentManager, string path)
        {
            return TitleContainer.OpenStream(contentManager.RootDirectory + DirectorySeparatorChar + path);
        }

        public static T Load<T>(this ContentManager contentManager, string assetName, ContentLoader<T> contentLoader)
        {
            using (var stream = OpenStream(contentManager, assetName))
            {
                return contentLoader.LoadContentFromStream(contentManager, stream);
            }
        }

        public static GraphicsDevice GetGraphicsDevice(this ContentManager contentManager)
        {
            // http://konaju.com/?p=21
            var graphicsDeviceService = (IGraphicsDeviceService) contentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
            return graphicsDeviceService.GraphicsDevice;
        }
    }
}
