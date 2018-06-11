using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Utilities;
using CompressionMode = System.IO.Compression.CompressionMode;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentProcessor(DisplayName = "Tiled Map Processor - MonoGame.Extended")]
    public class TiledMapProcessor : ContentProcessor<TiledMapContent, TiledMapContent>
    {
        public override TiledMapContent Process(TiledMapContent map, ContentProcessorContext context)
        {
            try
			{
				ContentLogger.Logger = context.Logger;

				if (map.Orientation == TiledMapOrientationContent.Hexagonal || map.Orientation == TiledMapOrientationContent.Staggered)
					throw new NotSupportedException($"{map.Orientation} Tiled Maps are currently not implemented!");

				foreach (var tileset in map.Tilesets)
				{
					if (String.IsNullOrWhiteSpace(tileset.Source))
						// Load the Texture2DContent for the tileset as it will be saved into the map content file.
						tileset.Image.Content = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(tileset.Image.Source), "");
					else
						// Link to the tileset for the content loader to load at runtime.
						tileset.Content = context.BuildAsset<TiledMapTilesetContent, TiledMapTilesetContent>(new ExternalReference<TiledMapTilesetContent>(tileset.Source), "");
					
				}

				ProcessLayers(map, context, map.Layers);

				map.Layers = FlattenGroups(map.Layers, out var hadGroups);

				if (hadGroups) ContentLogger.Log($"FYI, TiledMap '{map.FilePath}' contains group layers. These are currently not supported \n" +
					$"as is, and they will be discarded with the sub layers moved to root.");
				
				return map;
			}
			catch (Exception ex)
            {
                context.Logger.LogImportantMessage(ex.Message);
                context.Logger.LogImportantMessage("Hello World!");
                return null;
            }
        }

		private static void ProcessLayers(TiledMapContent map, ContentProcessorContext context, List<TiledMapLayerContent> layers)
		{
			foreach (var layer in layers)
			{
				if (layer is TiledMapImageLayerContent imageLayer)
				{
					ContentLogger.Log($"Processing image layer '{imageLayer.Name}'");
					imageLayer.Image.ContentRef = context.BuildAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(imageLayer.Image.Source), "");
					ContentLogger.Log($"Processed image layer '{imageLayer.Name}'");
				}

				if (layer is TiledMapTileLayerContent tileLayer)
				{
					if (tileLayer.Data.Chunks.Count > 0)
						throw new NotSupportedException($"{map.FilePath} contains data chunks. These are currently not supported.");

					var data = tileLayer.Data;
					var encodingType = data.Encoding ?? "xml";
					var compressionType = data.Compression ?? "xml";

					ContentLogger.Log(
						$"Processing tile layer '{tileLayer.Name}': Encoding: '{encodingType}', Compression: '{compressionType}'");

					var tileData = DecodeTileLayerData(encodingType, tileLayer);
					var tiles = CreateTiles(map.RenderOrder, map.Width, map.Height, tileData);
					tileLayer.Tiles = tiles;

					ContentLogger.Log($"Processed tile layer '{tileLayer}': {tiles.Length} tiles");
				}

				if (layer is TiledMapObjectLayerContent objectLayer)
				{
					ContentLogger.Log($"Processing object layer '{objectLayer.Name}'");
					foreach (var obj in objectLayer.Objects)
						TiledMapObjectContent.Process(obj, context);
					ContentLogger.Log($"Processed object layer '{objectLayer.Name}'");
				}

				if (layer is TiledMapGroupLayerContent groupLayer)
					ProcessLayers(map, context, groupLayer.Layers);
			}
		}

		private static List<TiledMapLayerContent> FlattenGroups(List<TiledMapLayerContent> layers, out bool hadGroups)
		{
			hadGroups = false;
			var result = new List<TiledMapLayerContent>();

			foreach (var layer in layers)
			{
				if (layer is TiledMapGroupLayerContent groupLayer)
				{
					hadGroups = true;
					foreach (var sublayer in groupLayer.Layers)
					{
						sublayer.OffsetX += groupLayer.OffsetX;
						sublayer.OffsetY += groupLayer.OffsetY;
						sublayer.Opacity *= groupLayer.Opacity;
						sublayer.Visible &= groupLayer.Visible;
					}
					result.AddRange(FlattenGroups(groupLayer.Layers, out var subHadGroups));
				}
				else
					result.Add(layer);
			}

			return result;
		}

		private static List<TiledMapTileContent> DecodeTileLayerData(string encodingType, TiledMapTileLayerContent tileLayer)
        {
            List<TiledMapTileContent> tiles;

            switch (encodingType)
            {
                case "xml":
                    tiles = tileLayer.Data.Tiles;
                    break;
                case "csv":
                    tiles = DecodeCommaSeperatedValuesData(tileLayer.Data);
                    break;
                case "base64":
                    tiles = DecodeBase64Data(tileLayer.Data, tileLayer.Width, tileLayer.Height);
                    break;
                default:
                    throw new NotSupportedException($"The tile layer encoding '{encodingType}' is not supported.");
            }

            return tiles;
        }

        private static TiledMapTile[] CreateTiles(TiledMapTileDrawOrderContent renderOrder, int mapWidth, int mapHeight, List<TiledMapTileContent> tileData)
        {
            TiledMapTile[] tiles;

            switch (renderOrder)
            {
                case TiledMapTileDrawOrderContent.LeftDown:
                    tiles = CreateTilesInLeftDownOrder(tileData, mapWidth, mapHeight).ToArray();
                    break;
                case TiledMapTileDrawOrderContent.LeftUp:
                    tiles = CreateTilesInLeftUpOrder(tileData, mapWidth, mapHeight).ToArray();
                    break;
                case TiledMapTileDrawOrderContent.RightDown:
                    tiles = CreateTilesInRightDownOrder(tileData, mapWidth, mapHeight).ToArray();
                    break;
                case TiledMapTileDrawOrderContent.RightUp:
                    tiles = CreateTilesInRightUpOrder(tileData, mapWidth, mapHeight).ToArray();
                    break;
                default:
                    throw new NotSupportedException($"{renderOrder} is not supported.");
            }

            return tiles.ToArray();
        }

        private static IEnumerable<TiledMapTile> CreateTilesInLeftDownOrder(List<TiledMapTileContent> tileLayerData, int mapWidth, int mapHeight)
        {
            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = mapWidth - 1; x >= 0; x--)
                {
                    var dataIndex = x + y * mapWidth;
                    var globalIdentifier = tileLayerData[dataIndex].GlobalIdentifier;
                    if (globalIdentifier == 0)
                        continue;
                    var tile = new TiledMapTile(globalIdentifier, (ushort)x, (ushort)y);
                    yield return tile;
                }
            }
        }

        private static IEnumerable<TiledMapTile> CreateTilesInLeftUpOrder(List<TiledMapTileContent> tileLayerData, int mapWidth, int mapHeight)
        {
            for (var y = mapHeight - 1; y >= 0; y--)
            {
                for (var x = mapWidth - 1; x >= 0; x--)
                {
                    var dataIndex = x + y * mapWidth;
                    var globalIdentifier = tileLayerData[dataIndex].GlobalIdentifier;
                    if (globalIdentifier == 0)
                        continue;
                    var tile = new TiledMapTile(globalIdentifier, (ushort)x, (ushort)y);
                    yield return tile;
                }
            }
        }

        private static IEnumerable<TiledMapTile> CreateTilesInRightDownOrder(List<TiledMapTileContent> tileLayerData, int mapWidth, int mapHeight)
        {
            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    var dataIndex = x + y * mapWidth;
                    var globalIdentifier = tileLayerData[dataIndex].GlobalIdentifier;
                    if (globalIdentifier == 0)
                        continue;
                    var tile = new TiledMapTile(globalIdentifier, (ushort)x, (ushort)y);
                    yield return tile;
                }
            }
        }

        private static IEnumerable<TiledMapTile> CreateTilesInRightUpOrder(List<TiledMapTileContent> tileLayerData, int mapWidth, int mapHeight)
        {
            for (var y = mapHeight - 1; y >= 0; y--)
            {
                for (var x = mapWidth - 1; x >= 0; x--)
                {
                    var dataIndex = x + y * mapWidth;
                    var globalIdentifier = tileLayerData[dataIndex].GlobalIdentifier;
                    if (globalIdentifier == 0)
                        continue;
                    var tile = new TiledMapTile(globalIdentifier, (ushort)x, (ushort)y);
                    yield return tile;
                }
            }
        }

        private static List<TiledMapTileContent> DecodeBase64Data(TiledMapTileLayerDataContent data, int width, int height)
        {
            var tileList = new List<TiledMapTileContent>();
            var encodedData = data.Value.Trim();
            var decodedData = Convert.FromBase64String(encodedData);

            using (var stream = OpenStream(decodedData, data.Compression))
            {
                using (var reader = new BinaryReader(stream))
                {
                    data.Tiles = new List<TiledMapTileContent>();

                    for (var y = 0; y < width; y++)
                    {
                        for (var x = 0; x < height; x++)
                        {
                            var gid = reader.ReadUInt32();
                            tileList.Add(new TiledMapTileContent
                            {
                                GlobalIdentifier = gid
                            });
                        }
                    }
                }
            }

            return tileList;
        }

        private static List<TiledMapTileContent> DecodeCommaSeperatedValuesData(TiledMapTileLayerDataContent data)
        {
            return data.Value
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(uint.Parse)
                .Select(x => new TiledMapTileContent { GlobalIdentifier = x })
                .ToList();
        }

        private static Stream OpenStream(byte[] decodedData, string compressionMode)
        {
            var memoryStream = new MemoryStream(decodedData, false);

            switch (compressionMode)
            {
                case "gzip":
                    return new GZipStream(memoryStream, CompressionMode.Decompress);
                case "zlib":
                    return new ZlibStream(memoryStream, Utilities.CompressionMode.Decompress);
                default:
                    return memoryStream;
            }
        }
    }
}