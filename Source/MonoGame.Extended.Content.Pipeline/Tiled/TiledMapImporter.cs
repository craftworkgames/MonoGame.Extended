using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentImporter(".tmx", DefaultProcessor = "TiledMapProcessor", DisplayName = "Tiled Map Importer - MonoGame.Extended")]
    public class TiledMapImporter : ContentImporter<TiledMapContent>
    {
        public override TiledMapContent Import(string filePath, ContentImporterContext context)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            ContentLogger.Logger = context.Logger;
            ContentLogger.Log($"Importing '{filePath}'");

            var map = DeserializeTiledMapContent(filePath);

            if (map.Width > ushort.MaxValue || map.Height > ushort.MaxValue)
                throw new InvalidContentException($"The map '{filePath} is much too large. The maximum supported width and height for a Tiled map is {ushort.MaxValue}.");

            ContentLogger.Log($"Imported '{filePath}'");

            return map;
        }

        private static TiledMapContent DeserializeTiledMapContent(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                var mapSerializer = new XmlSerializer(typeof(TiledMapContent));
                var map = (TiledMapContent)mapSerializer.Deserialize(reader);

                map.FilePath = filePath;

                var tilesetSerializer = new XmlSerializer(typeof(TiledMapTilesetContent));

                for (var i = 0; i < map.Tilesets.Count; i++)
                {
                    var tileset = map.Tilesets[i];

                    if (string.IsNullOrWhiteSpace(tileset.Source))
                        continue;
                    var directoryName = Path.GetDirectoryName(filePath);
                    var tilesetLocation = tileset.Source.Replace('/', Path.DirectorySeparatorChar);

                    Debug.Assert(directoryName != null, "directoryName != null");
                    var tilesetFilePath = Path.Combine(directoryName, tilesetLocation);

                    ContentLogger.Log($"Importing tileset '{tilesetFilePath}'");

                    using (var file = new FileStream(tilesetFilePath, FileMode.Open))
                    {
                        map.Tilesets[i] = (TiledMapTilesetContent)tilesetSerializer.Deserialize(file);
                    }

                    ContentLogger.Log($"Imported tileset '{tilesetFilePath}'");
                }

                map.Name = filePath;

                return map;
            }
        }
    }
}