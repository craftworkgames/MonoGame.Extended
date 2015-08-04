using Microsoft.Xna.Framework.Content.Pipeline;
using TiledSharp;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentProcessor(DisplayName = "Tiled Map Processor - MonoGame.Extended")]
    public class TiledMapProcessor : ContentProcessor<TmxMap, TiledMapProcessorResult>
    {
        public override TiledMapProcessorResult Process(TmxMap map, ContentProcessorContext context)
        {
            return new TiledMapProcessorResult(map);
        }
    }
}