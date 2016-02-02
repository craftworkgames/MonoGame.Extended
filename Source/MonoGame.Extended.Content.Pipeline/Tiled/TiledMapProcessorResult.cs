
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public class TiledMapProcessorResult
    {
        public TiledMapProcessorResult(TmxMap map, ContentBuildLogger logger)
        {
            Map = map;
            Logger = logger;
        }

        public TmxMap Map { get; private set; }
        public ContentBuildLogger Logger { get; private set; }
    }
}