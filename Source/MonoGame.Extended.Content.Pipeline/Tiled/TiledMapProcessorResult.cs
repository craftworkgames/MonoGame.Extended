
namespace MonoGame.Extended.Content.Pipeline.Tiled
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