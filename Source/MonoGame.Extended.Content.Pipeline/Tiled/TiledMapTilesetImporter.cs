using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.IO;
using System.Xml.Serialization;
using MonoGame.Extended.Tiled.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentImporter(".tsx", DefaultProcessor = "TiledMapTilesetProcessor", DisplayName = "Tiled Map Tileset Importer - MonoGame.Extended")]
	public class TiledMapTilesetImporter : ContentImporter<TiledMapTilesetContentItem>
	{
		public override TiledMapTilesetContentItem Import(string filePath, ContentImporterContext context)
		{
			try
			{
				if (filePath == null)
					throw new ArgumentNullException(nameof(filePath));

				ContentLogger.Logger = context.Logger;
				ContentLogger.Log($"Importing '{filePath}'");

				var tileset = DeserializeTiledMapTilesetContent(filePath, context);

				ContentLogger.Log($"Imported '{filePath}'");

				return new TiledMapTilesetContentItem(tileset);
			}
			catch (Exception e)
			{
				context.Logger.LogImportantMessage(e.StackTrace);
				throw;
			}
		}

		private TiledMapTilesetContent DeserializeTiledMapTilesetContent(string filePath, ContentImporterContext context)
		{
			using (var reader = new StreamReader(filePath))
			{
				var tilesetSerializer = new XmlSerializer(typeof(TiledMapTilesetContent));
				var tileset = (TiledMapTilesetContent)tilesetSerializer.Deserialize(reader);

				tileset.Image.Source = $"{Path.GetDirectoryName(filePath)}/{tileset.Image.Source}";
				ContentLogger.Log($"Adding dependency '{tileset.Image.Source}'");
				context.AddDependency(tileset.Image.Source);

				foreach (var tile in tileset.Tiles)
				{
				    foreach (var obj in tile.Objects)
				    {
				        if (!string.IsNullOrWhiteSpace(obj.TemplateSource))
				        {
				            obj.TemplateSource = $"{Path.GetDirectoryName(filePath)}/{obj.TemplateSource}";
				            ContentLogger.Log($"Adding dependency '{obj.TemplateSource}'");

				            // We depend on the template.
				            context.AddDependency(obj.TemplateSource);
				        }
				    }
				}

			    return tileset;
			}
		}
	}
}
