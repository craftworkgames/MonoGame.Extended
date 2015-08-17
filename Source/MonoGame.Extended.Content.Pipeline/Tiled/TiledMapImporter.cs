using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentImporter(".tmx", DefaultProcessor = "TiledMapProcessor", DisplayName = "Tiled Map Importer - MonoGame.Extended")]
    public class TiledMapImporter : ContentImporter<TmxMap>
    {
        public override TmxMap Import(string filename, ContentImporterContext context)
        {
            using (var reader = new StreamReader(filename))
            {
                var serializer = new XmlSerializer(typeof(TmxMap));
                return (TmxMap)serializer.Deserialize(reader);
            }
        }
    }
}
