#region

using System;
using System.IO;

#endregion

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