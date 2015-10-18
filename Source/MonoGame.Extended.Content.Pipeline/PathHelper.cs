namespace MonoGame.Extended.Content.Pipeline
{
    internal static class PathHelper
    {
        public static string RemoveExtension(string path)
        {
            var dotIndex = path.LastIndexOf('.');
            return dotIndex > 0 ? path.Substring(0, dotIndex) : path;
        }
    }
}
