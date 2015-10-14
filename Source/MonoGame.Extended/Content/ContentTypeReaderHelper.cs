using System.Linq;

namespace MonoGame.Extended.Content
{
    internal static class ContentTypeReaderHelper
    {
        public static string GetDirectory(string assetPath)
        {
            var pathNodes = assetPath.Split('\\', '/');

            if (pathNodes.Length > 1)
                return string.Join("/", pathNodes.Take(pathNodes.Length - 1).ToArray()) + "/";

            return string.Empty;
        }
    }
}