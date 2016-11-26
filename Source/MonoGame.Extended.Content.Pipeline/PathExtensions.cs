using System;
using System.IO;

namespace MonoGame.Extended.Content.Pipeline
{
    public static class PathExtensions
    {
        public static string GetApplicationFullPath(params string[] pathParts)
        {
            var path = Path.Combine(pathParts);
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }
    }
}
