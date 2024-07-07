using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGame.Extended.Content.Tiled;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public static class ContentWriterExtensions
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        public static void WriteTiledMapProperties(this ContentWriter writer, IReadOnlyCollection<TiledMapPropertyContent> value)
        {
            if (value == null)
            {
                writer.Write(0);
                return;
            }
            writer.Write(value.Count);
            foreach (var property in value)
            {
                writer.Write(property.Name);
                writer.Write(property.Value ?? string.Empty);
                WriteTiledMapProperties(writer, property.Properties);
            }
        }
    }
}
