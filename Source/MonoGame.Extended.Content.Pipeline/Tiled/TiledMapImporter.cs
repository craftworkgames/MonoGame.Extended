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
                var map = (TmxMap)serializer.Deserialize(reader);

                XmlSerializer xml = new XmlSerializer(typeof(TmxTileset));
                for (int i = 0; i < map.Tilesets.Count; i++)
                {
                    var tileset = map.Tilesets[i];
                    if (!string.IsNullOrWhiteSpace(tileset.Source))
                    {
                        string dir = Path.GetDirectoryName(filename);
                        string tilesetLocation = tileset.Source.Replace('/', Path.DirectorySeparatorChar);
                        string filePath = Path.Combine(dir, tilesetLocation);
                        using (FileStream file = new FileStream(filePath, FileMode.Open))
                            map.Tilesets[i] = (TmxTileset)xml.Deserialize(file);
                    }
                }

                return map;
            }
        }
    }
}