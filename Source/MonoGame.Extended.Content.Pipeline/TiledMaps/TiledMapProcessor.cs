using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline.TiledMaps
{
    [ContentProcessor(DisplayName = "Tiled Map Processor - MonoGame.Extended")]
    public class TiledMapProcessor : ContentProcessor<TiledMapFile, TiledMapProcessorResult>
    {
        public override TiledMapProcessorResult Process(TiledMapFile input, ContentProcessorContext context)
        {
            return new TiledMapProcessorResult();
        }
    }
}