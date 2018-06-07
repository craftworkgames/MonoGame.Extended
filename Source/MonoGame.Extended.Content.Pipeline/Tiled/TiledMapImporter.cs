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
            try
            {
                if (filePath == null)
                    throw new ArgumentNullException(nameof(filePath));

                ContentLogger.Logger = context.Logger;
                ContentLogger.Log($"Importing '{filePath}'");

                var map = DeserializeTiledMapContent(filePath, context);

                if (map.Width > ushort.MaxValue || map.Height > ushort.MaxValue)
                    throw new InvalidContentException($"The map '{filePath} is much too large. The maximum supported width and height for a Tiled map is {ushort.MaxValue}.");

                ContentLogger.Log($"Imported '{filePath}'");

                return map;

            }
            catch (Exception e)
            {
                context.Logger.LogImportantMessage(e.StackTrace);
                return null;
            }
        }

        private static TiledMapContent DeserializeTiledMapContent(string mapFilePath, ContentImporterContext context)
        {
            using (var reader = new StreamReader(mapFilePath))
            {
                var mapSerializer = new XmlSerializer(typeof(TiledMapContent));
                var map = (TiledMapContent)mapSerializer.Deserialize(reader);

                map.FilePath = mapFilePath;

                var tilesetSerializer = new XmlSerializer(typeof(TiledMapTilesetContent));

                for (var i = 0; i < map.Tilesets.Count; i++)
                {
                    var tileset = map.Tilesets[i];

                    if (!string.IsNullOrWhiteSpace(tileset.Source))
                    {
                        var tilesetFilePath = GetTilesetFilePath(mapFilePath, tileset);
                        map.Tilesets[i] = ImportTileset(tilesetFilePath, tilesetSerializer, tileset, context);
                    }
                }

				for (var i = 0; i < map.Layers.Count; i++)
				{
					if (map.Layers[i] is TiledMapImageLayerContent imageLayer)
						context.AddDependency(imageLayer.Image.Source);
					if (map.Layers[i] is TiledMapObjectLayerContent objectLayer)
						foreach (var obj in objectLayer.Objects)
							if (!String.IsNullOrWhiteSpace(obj.TemplateSource))
								context.AddDependency(obj.TemplateSource);
				}

                map.Name = mapFilePath;
                return map;
            }
        }

        private static string GetTilesetFilePath(string mapFilePath, TiledMapTilesetContent tileset)
        {
            var directoryName = Path.GetDirectoryName(mapFilePath);
            Debug.Assert(directoryName != null, "directoryName != null");

            var tilesetLocation = tileset.Source.Replace('/', Path.DirectorySeparatorChar);
            var tilesetFilePath = Path.Combine(directoryName, tilesetLocation);
            return tilesetFilePath;
        }

        private static TiledMapTilesetContent ImportTileset(string tilesetFilePath, XmlSerializer tilesetSerializer, TiledMapTilesetContent tileset, ContentImporterContext context)
        {
            TiledMapTilesetContent result;

            ContentLogger.Log($"Importing tileset '{tilesetFilePath}'");

            using (var file = new FileStream(tilesetFilePath, FileMode.Open))
            {
                var importedTileset = (TiledMapTilesetContent)tilesetSerializer.Deserialize(file);
                importedTileset.FirstGlobalIdentifier = tileset.FirstGlobalIdentifier;
				context.AddDependency(importedTileset.Image.Source);

				foreach (var tile in importedTileset.Tiles)
					foreach (var obj in tile.Objects)
						if (!String.IsNullOrWhiteSpace(obj.TemplateSource))
							context.AddDependency(obj.TemplateSource);

                result = importedTileset;
            }

            ContentLogger.Log($"Imported tileset '{tilesetFilePath}'");

            return result;
        }
    }
}