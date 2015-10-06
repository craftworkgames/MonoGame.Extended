using System;
using System.Diagnostics;
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
            if (filename == null) throw new ArgumentNullException("filename");

            using (var reader = new StreamReader(filename))
            {
                var serializer = new XmlSerializer(typeof(TmxMap));
                var map = (TmxMap)serializer.Deserialize(reader);
                var xmlSerializer = new XmlSerializer(typeof(TmxTileset));

                for (var i = 0; i < map.Tilesets.Count; i++)
                {
                    var tileset = map.Tilesets[i];

                    if (!string.IsNullOrWhiteSpace(tileset.Source))
                    {
                        var directoryName = Path.GetDirectoryName(filename);
                        var tilesetLocation = tileset.Source.Replace('/', Path.DirectorySeparatorChar);

                        Debug.Assert(directoryName != null, "directoryName != null");
                        var filePath = Path.Combine(directoryName, tilesetLocation);

                        using (var file = new FileStream(filePath, FileMode.Open))
                        {
                            map.Tilesets[i] = (TmxTileset) xmlSerializer.Deserialize(file);
                        }
                    }
                }

                return map;
            }
        }
    }
}