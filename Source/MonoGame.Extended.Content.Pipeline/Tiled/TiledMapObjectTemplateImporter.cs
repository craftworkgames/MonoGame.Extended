using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.IO;
using System.Xml.Serialization;
using MonoGame.Extended.Tiled.Serialization;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
	[ContentImporter(".tx", DefaultProcessor = "TiledMapObjectTemplateProcessor", DisplayName = "Tiled Map Object Template Importer - MonoGame.Extended")]
	public class TiledMapObjectTemplateImporter : ContentImporter<TiledMapObjectTemplateContent>
	{
		public override TiledMapObjectTemplateContent Import(string filePath, ContentImporterContext context)
		{
			try
			{
				if (filePath == null)
					throw new ArgumentNullException(nameof(filePath));

				ContentLogger.Logger = context.Logger;
				ContentLogger.Log($"Importing '{filePath}'");

				var template = DeserializeTileMapObjectTemplateContent(filePath, context);

				ContentLogger.Log($"Imported '{filePath}'");

				return template;
			}
			catch (Exception e)
			{
				context.Logger.LogImportantMessage(e.StackTrace);
				return null;
			}
		}

		private static TiledMapObjectTemplateContent DeserializeTileMapObjectTemplateContent(string filePath, ContentImporterContext context)
		{
			using (var reader = new StreamReader(filePath))
			{
				var templateSerializer = new XmlSerializer(typeof(TiledMapObjectTemplateContent));
				var template = (TiledMapObjectTemplateContent)templateSerializer.Deserialize(reader);

				if (!string.IsNullOrWhiteSpace(template.Tileset?.Source))
				{
					template.Tileset.Source = $"{Path.GetDirectoryName(filePath)}/{template.Tileset.Source}";
					ContentLogger.Log($"Adding dependency '{template.Tileset.Source}'");
					// We depend on this tileset.
					context.AddDependency(template.Tileset.Source);
				}

				return template;
			}
		}
	}
}
