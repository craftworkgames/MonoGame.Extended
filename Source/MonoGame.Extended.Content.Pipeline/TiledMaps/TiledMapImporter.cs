using Microsoft.Xna.Framework.Content.Pipeline;
using TiledSharp;

namespace MonoGame.Extended.Content.Pipeline.TiledMaps
{
    [ContentImporter(".tmx", DefaultProcessor = "TiledMapProcessor", DisplayName = "Tiled Map Importer - MonoGame.Extended")]
    public class TiledMapImporter : ContentImporter<TmxMap>
    {
        public override TmxMap Import(string filename, ContentImporterContext context)
        {
            return new TmxMap(filename);
        }
    }
}
