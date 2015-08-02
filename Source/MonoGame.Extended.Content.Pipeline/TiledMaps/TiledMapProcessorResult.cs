using TiledSharp;

namespace MonoGame.Extended.Content.Pipeline.TiledMaps
{
    public class TiledMapProcessorResult
    {
        public TiledMapProcessorResult(TmxMap map)
        {
            Map = map;
        }

        public TmxMap Map { get; private set; }
    }
}