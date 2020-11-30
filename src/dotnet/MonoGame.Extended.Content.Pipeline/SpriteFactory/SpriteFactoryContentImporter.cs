using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.Pipeline.Json;

namespace MonoGame.Extended.Content.Pipeline.SpriteFactory
{
    [ContentImporter(".sf", DefaultProcessor = nameof(SpriteFactoryContentProcessor), DisplayName = "Sprite Factory Importer - MonoGame.Extended")]
    public class SpriteFactoryContentImporter : JsonContentImporter
    {
    }
}
