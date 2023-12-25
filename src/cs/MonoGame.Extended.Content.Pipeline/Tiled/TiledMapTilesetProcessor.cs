using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
	[ContentProcessor(DisplayName = "Tiled Map Tileset Processor - MonoGame.Extended")]
	public class TiledMapTilesetProcessor : ContentProcessor<TiledMapTilesetContentItem, TiledMapTilesetContentItem>
	{
		public override TiledMapTilesetContentItem Process(TiledMapTilesetContentItem contentItem, ContentProcessorContext context)
		{
			try
			{
			    var tileset = contentItem.Data;

			    ContentLogger.Logger = context.Logger;
				ContentLogger.Log($"Processing tileset '{tileset.Name}'");

				// Build the Texture2D asset and load it as it will be saved as part of this tileset file.
                if (tileset.Image is not null)
                    contentItem.BuildExternalReference<Texture2DContent>(context, tileset.Image);

				foreach (var tile in tileset.Tiles)
				{
				    foreach (var obj in tile.Objects)
				    {
				        TiledMapContentHelper.Process(obj, context);
				    }
                    if (tile.Image is not null)
                        contentItem.BuildExternalReference<Texture2DContent>(context, tile.Image);
				}

			    ContentLogger.Log($"Processed tileset '{tileset.Name}'");

				return contentItem;
			}
			catch (Exception ex)
			{
				context.Logger.LogImportantMessage(ex.Message);
				throw ex;
			}
		}
	}
}
