using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
	[ContentProcessor(DisplayName = "Tiled Map Tileset Processor - MonoGame.Extended")]
	public class TiledMapTilesetProcessor : ContentProcessor<TiledMapTilesetContent, TiledMapTilesetContent>
	{
		public override TiledMapTilesetContent Process(TiledMapTilesetContent tileset, ContentProcessorContext context)
		{
			try
			{
				ContentLogger.Logger = context.Logger;

				ContentLogger.Log($"Processing tileset '{tileset.Name}'");
				// Build the Texture2D asset and load it as it will be saved as part of this tileset file.
			    var externalReference = new ExternalReference<Texture2DContent>(tileset.Image.Source);
			    tileset.Image.ContentRef = context.BuildAsset<Texture2DContent, Texture2DContent>(externalReference, "", new OpaqueDataDictionary
			    {
			        { "ColorKeyColor", tileset.Image.TransparentColor },
			        { "ColorKeyEnabled", true }
			    }, "", "");

				foreach (var tile in tileset.Tiles)
				{
				    foreach (var obj in tile.Objects)
				    {
				        TiledMapObjectContent.Process(obj, context);
				    }
				}

			    ContentLogger.Log($"Processed tileset '{tileset.Name}'");

				return tileset;
			}
			catch (Exception ex)
			{
				context.Logger.LogImportantMessage(ex.Message);
				throw ex;
			}
		}
	}
}
