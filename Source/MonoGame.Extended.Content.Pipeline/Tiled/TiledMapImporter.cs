using System;
using System.Collections.Generic;
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
						map.Tilesets[i] = ImportTileset(tileset.Source, tilesetSerializer, tileset, context);
				}

				ImportLayers(context, map.Layers);

				map.Name = mapFilePath;
				return map;
			}
		}

		private static void ImportLayers(ContentImporterContext context, List<TiledMapLayerContent> layers)
		{
			for (var i = 0; i < layers.Count; i++)
			{
				if (layers[i] is TiledMapImageLayerContent imageLayer)
					// Tell the pipeline that we depend on this image and need to rebuild the map if the image changes.
					// (Maybe the image is a different size)
					context.AddDependency(imageLayer.Image.Source);
				if (layers[i] is TiledMapObjectLayerContent objectLayer)
					foreach (var obj in objectLayer.Objects)
						if (!String.IsNullOrWhiteSpace(obj.TemplateSource))
							// Tell the pipeline that we depend on this template and need to rebuild the map if the template changes.
							// (Templates are loaded into objects on process, so all objects which depend on the template file
							//  need the change to the template)
							context.AddDependency(obj.TemplateSource);
				if (layers[i] is TiledMapGroupLayerContent groupLayer)
					// Yay recursion!
					ImportLayers(context, groupLayer.Layers);
			}
		}

        private static TiledMapTilesetContent ImportTileset(string tilesetFilePath, XmlSerializer tilesetSerializer, TiledMapTilesetContent tileset, ContentImporterContext context)
        {
            TiledMapTilesetContent result;

            ContentLogger.Log($"Importing tileset '{tilesetFilePath}'");

            using (var reader = new StreamReader(tilesetFilePath))
            {
                var importedTileset = (TiledMapTilesetContent)tilesetSerializer.Deserialize(reader);
                importedTileset.FirstGlobalIdentifier = tileset.FirstGlobalIdentifier;
				// We depend on the tileset. If the tileset changes, the map also needs to rebuild.
				context.AddDependency(importedTileset.Image.Source);

				foreach (var tile in importedTileset.Tiles)
					foreach (var obj in tile.Objects)
						if (!String.IsNullOrWhiteSpace(obj.TemplateSource))
							// We depend on the template.
							context.AddDependency(obj.TemplateSource);

                result = importedTileset;
            }

            ContentLogger.Log($"Imported tileset '{tilesetFilePath}'");

            return result;
        }
    }
}