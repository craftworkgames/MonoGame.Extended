using Microsoft.Xna.Framework.Content.Pipeline;
using TiledSharp;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentImporter(".tmx", DefaultProcessor = "TiledMapProcessor", DisplayName = "Tiled Map Importer - MonoGame.Extended")]
    public class TiledMapImporter : ContentImporter<TmxMap>
    {
        public override TmxMap Import(string filename, ContentImporterContext context)
        {
            // it's not ideal that the constructor reads the XML.
            // in future we might pull apart TmxMap and implement our own parser.
            return new TmxMap(filename);
        }
    }
}
