using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.Pipeline.Json;

namespace MonoGame.Extended.Content.Pipeline.SpriteFactory
{
    [ContentProcessor(DisplayName = "Sprite Factory Processor - MonoGame.Extended")]
    public class SpriteFactoryContentProcessor : JsonContentProcessor
    {
        public SpriteFactoryContentProcessor()
        {
            ContentType = "MonoGame.Extended MonoGame.Extended.Animations.SpriteFactory.SpriteFactoryFileReader, MonoGame.Extended.Animations";
        }

    }
}