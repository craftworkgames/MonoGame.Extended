using Microsoft.Xna.Framework.Content.Pipeline;
using TiledSharp;

namespace MonoGame.Extended.Content.Pipeline.TiledMaps
{
    [ContentImporter(".tmx", DefaultProcessor = "TiledMapProcessor", DisplayName = "Tiled Map Importer - MonoGame.Extended")]
    public class TiledMapImporter : ContentImporter<TiledMapFile>
    {
        public override TiledMapFile Import(string filename, ContentImporterContext context)
        {
            var map = new TmxMap(filename);
            return new TiledMapFile();
        }
    }
}
