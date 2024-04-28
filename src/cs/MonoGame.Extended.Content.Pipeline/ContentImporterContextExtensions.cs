using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline;

public static class ContentImporterContextExtensions
{
    public static string AddDependencyWithLogging(this ContentImporterContext context, string filePath, string source)
    {
        source = Path.Combine(Path.GetDirectoryName(filePath), source);
        ContentLogger.Log($"Adding dependency '{source}'");
        context.AddDependency(source);
        return source;
    }
}
