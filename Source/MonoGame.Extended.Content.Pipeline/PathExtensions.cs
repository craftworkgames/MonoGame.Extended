using System;
using System.IO;

namespace MonoGame.Extended.Content.Pipeline
{
    public static class PathExtensions
    {
        public static string GetApplicationPath(string path)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }
    }
}
