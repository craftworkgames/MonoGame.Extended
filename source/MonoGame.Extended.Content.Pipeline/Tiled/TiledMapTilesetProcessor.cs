using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
#if MONOGAME_385_OR_NEWER && !KNI && !FNA
    /// <summary>
    /// Processes Tiled tileset files into runtime content.
    /// </summary>
    [ContentProcessor(DisplayName = "Tiled Map Tileset Processor - MonoGame.Extended")]
	public class TiledMapTilesetProcessor : ContentProcessor<TiledMapTilesetContentItem, TiledMapTilesetContentItem>
	{
        /// <summary>
        /// Processes the Tiled tileset content.
        /// </summary>
        /// <param name="contentItem">The imported tileset content item.</param>
        /// <param name="context">The processor context.</param>
        /// <returns>The processed tileset content item with built external references.</returns>
        public override TiledMapTilesetContentItem Process(TiledMapTilesetContentItem contentItem, ContentProcessorContext context)
		{
			try
			{
			    ContentLogger.Logger = context.Logger;
			    var tileset = contentItem.Data;

                //ContentLogger.Log($"Processing tileset '{tileset.Name}'");
                ContentLogger.Log($"Processing Tiled tileset");

                // Build the Texture2D asset and load it as it will be saved as part of this tileset file.
                if (tileset.Image is not null)
                {
                    ContentLogger.Log($"Building texture reference: {tileset.Image.Source}");
                    TiledContentBuildHelpers.BuildTextureReference(contentItem, context, tileset.Image);
                    //contentItem.BuildExternalReference<Texture2DContent>(context, tileset.Image);
                }

                // Build texture references for individual tile images
                foreach (var tile in tileset.Tiles)
				{
                    if(tile.Image != null)
                    {
                        ContentLogger.Log($"Building texture reference for tile {tile.LocalIdentifier}: {tile.Image.Source}");
                        TiledContentBuildHelpers.BuildTextureReference(contentItem, context, tile.Image);
                    }

				    foreach (var obj in tile.Objects)
				    {
				        TiledMapContentHelper.Process(obj, context);
				    }

                    //if (tile.Image is not null)
                    //    contentItem.BuildExternalReference<Texture2DContent>(context, tile.Image);
				}

			    //ContentLogger.Log($"Processed tileset '{tileset.Name}'");
                ContentLogger.Log($"Processed Tiled tileset");

                return contentItem;
			}
			catch (Exception ex)
			{
                //context.Logger.LogImportantMessage(ex.Message);
                context.Logger.Log(ex.Message);
                throw;
			}
		}
	}
#else

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
#endif
}
