using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
	[ContentImporter(".tsx", DefaultProcessor = "TiledMapTilesetProcessor", DisplayName = "Tiled Map Tileset Importer - MonoGame.Extended")]
	public class TiledMapTilesetImporter : ContentImporter<TiledMapTilesetContent>
	{
		public override TiledMapTilesetContent Import(string filePath, ContentImporterContext context)
		{
			try
			{
				if (filePath == null)
					throw new ArgumentNullException(nameof(filePath));

				ContentLogger.Logger = context.Logger;
				ContentLogger.Log($"Importing '{filePath}'");

				var tileset = DeserializeTiledMapTilesetContent(filePath, context);

				ContentLogger.Log($"Imported '{filePath}'");

				return tileset;
			}
			catch (Exception e)
			{
				context.Logger.LogImportantMessage(e.StackTrace);
				return null;
			}
		}

		private TiledMapTilesetContent DeserializeTiledMapTilesetContent(string filePath, ContentImporterContext context)
		{
			using (var reader = new StreamReader(filePath))
			{
				var tilesetSerializer = new XmlSerializer(typeof(TiledMapTilesetContent));
				var tileset = (TiledMapTilesetContent)tilesetSerializer.Deserialize(reader);

				context.AddDependency(tileset.Image.Source);

				foreach (var tile in tileset.Tiles)
					foreach (var obj in tile.Objects)
						if (!String.IsNullOrWhiteSpace(obj.TemplateSource))
							// We depend on the template.
							context.AddDependency(obj.TemplateSource);

				return tileset;
			}
		}
	}
}
