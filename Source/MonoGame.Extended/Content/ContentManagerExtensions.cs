using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Content
{
    public interface IContentLoader<out T>
    {
        T Load(ContentManager contentManager, string path);
    }

    public interface IContentLoader
    {
        T Load<T>(ContentManager contentManager, string path);
    }

    public static class ContentManagerExtensions
    {
        public const string DirectorySeparatorChar = "/";

        public static Stream OpenStream(this ContentManager contentManager, string path)
        {
            return TitleContainer.OpenStream(contentManager.RootDirectory + DirectorySeparatorChar + path);
        }

        public static GraphicsDevice GetGraphicsDevice(this ContentManager contentManager)
        {
            // http://konaju.com/?p=21
            var serviceProvider = contentManager.ServiceProvider;
            var graphicsDeviceService = (IGraphicsDeviceService) serviceProvider.GetService(typeof(IGraphicsDeviceService));
            return graphicsDeviceService.GraphicsDevice;
        }

        /// <summary>
        /// Loads the content using a custom content loader.
        /// </summary>
        public static T Load<T>(this ContentManager contentManager, string path, IContentLoader contentLoader)
        {
            return contentLoader.Load<T>(contentManager, path);
        }

        /// <summary>
        /// Loads the content using a custom content loader.
        /// </summary>
        public static T Load<T>(this ContentManager contentManager, string path, IContentLoader<T> contentLoader)
        {
            return contentLoader.Load(contentManager, path);
        }
    }
}