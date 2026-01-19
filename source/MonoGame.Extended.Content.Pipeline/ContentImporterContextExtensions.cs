using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline;

public static class ContentImporterContextExtensions
{
    public static string AddDependencyWithLogging(this ContentImporterContext context, string filePath, string source)
    {
        source = Path.Combine(Path.GetDirectoryName(filePath), source);

        // Strip relative up and down traversal if exists.
        // ie. we soemetimes have have paths like "X:\\src\\project\\Content\\Maps\\FooBar/../MapImages/blah"
        // This will replace that with "X:\\src\\project\\Content\\Maps\\MapImages\\blah"
        var sanitizedSource = Path.GetFullPath(source);

        ContentLogger.Log($"Adding dependency '{sanitizedSource}'");
        context.AddDependency(sanitizedSource);
        return sanitizedSource;
    }
}
