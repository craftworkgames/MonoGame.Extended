using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Serialization;
using MonoGame.Framework.Utilities.Deflate;
using CompressionMode = System.IO.Compression.CompressionMode;
using GZipStream = System.IO.Compression.GZipStream;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    public static class TiledMapContentHelper
    {
        public static void Process(TiledMapObjectContent obj, ContentProcessorContext context)
        {
            if (!string.IsNullOrWhiteSpace(obj.TemplateSource))
            {
                var externalReference = new ExternalReference<TiledMapObjectLayerContent>(obj.TemplateSource);
                var template = context.BuildAndLoadAsset<TiledMapObjectLayerContent, TiledMapObjectTemplateContent>(externalReference, "");

                // Nothing says a template can't reference another template.
                // Yay recusion!
                Process(template.Object, context);

                if (!obj._globalIdentifier.HasValue && template.Object._globalIdentifier.HasValue)
                    obj.GlobalIdentifier = template.Object.GlobalIdentifier;

                if (!obj._height.HasValue && template.Object._height.HasValue)
                    obj.Height = template.Object.Height;

                if (!obj._identifier.HasValue && template.Object._identifier.HasValue)
                    obj.Identifier = template.Object.Identifier;

                if (!obj._rotation.HasValue && template.Object._rotation.HasValue)
                    obj.Rotation = template.Object.Rotation;

                if (!obj._visible.HasValue && template.Object._visible.HasValue)
                    obj.Visible = template.Object.Visible;

                if (!obj._width.HasValue && template.Object._width.HasValue)
                    obj.Width = template.Object.Width;

                if (!obj._x.HasValue && template.Object._x.HasValue)
                    obj.X = template.Object.X;

                if (!obj._y.HasValue && template.Object._y.HasValue)
                    obj.Y = template.Object.Y;

                if (obj.Ellipse == null && template.Object.Ellipse != null)
                    obj.Ellipse = template.Object.Ellipse;

                if (string.IsNullOrWhiteSpace(obj.Name) && !string.IsNullOrWhiteSpace(template.Object.Name))
                    obj.Name = template.Object.Name;

                if (obj.Polygon == null && template.Object.Polygon != null)
                    obj.Polygon = template.Object.Polygon;

                if (obj.Polyline == null && template.Object.Polyline != null)
                    obj.Polyline = template.Object.Polyline;

                foreach (var tProperty in template.Object.Properties)
                {
                    if (!obj.Properties.Exists(p => p.Name == tProperty.Name))
                        obj.Properties.Add(tProperty);
                }

                if (string.IsNullOrWhiteSpace(obj.Type) && !string.IsNullOrWhiteSpace(template.Object.Type))
                    obj.Type = template.Object.Type;
            }
        }
    }
   

    [ContentProcessor(DisplayName = "Tiled Map Processor - MonoGame.Extended")]
    public class TiledMapProcessor : ContentProcessor<TiledMapContentItem, TiledMapContentItem>
    {
        public override TiledMapContentItem Process(TiledMapContentItem contentItem, ContentProcessorContext context)
        {
            try
			{
				ContentLogger.Logger = context.Logger;
			    var map = contentItem.Data;

				if (map.Orientation == TiledMapOrientationContent.Hexagonal || map.Orientation == TiledMapOrientationContent.Staggered)
					throw new NotSupportedException($"{map.Orientation} Tiled Maps are currently not implemented!");

				foreach (var tileset in map.Tilesets)
				{
					if (string.IsNullOrWhiteSpace(tileset.Source))
					{
                        // Load the Texture2DContent for the tileset as it will be saved into the map content file.
                        //var externalReference = new ExternalReference<Texture2DContent>(tileset.Image.Source);
                        var parameters = new OpaqueDataDictionary
                        {
                            { "ColorKeyColor", tileset.Image.TransparentColor },
                            { "ColorKeyEnabled", true }
                        };
                        //tileset.Image.ContentRef = context.BuildAsset<Texture2DContent, Texture2DContent>(externalReference, "", parameters, "", "");
                        contentItem.BuildExternalReference<Texture2DContent>(context, tileset.Image.Source, parameters);
					}
					else
					{
					    // Link to the tileset for the content loader to load at runtime.
					    //var externalReference = new ExternalReference<TiledMapTilesetContent>(tileset.Source);
					    //tileset.Content = context.BuildAsset<TiledMapTilesetContent, TiledMapTilesetContent>(externalReference, "");
					    contentItem.BuildExternalReference<TiledMapTilesetContent>(context, tileset.Source);
					}
				}

				ProcessLayers(contentItem, map, context, map.Layers);
				
				return contentItem;
			}
			catch (Exception ex)
            {
                context.Logger.LogImportantMessage(ex.Message);
				throw;
            }
        }

		private static void ProcessLayers(TiledMapContentItem contentItem, TiledMapContent map, ContentProcessorContext context, List<TiledMapLayerContent> layers)
		{
			foreach (var layer in layers)
			{
				switch (layer)
				{
				    case TiledMapImageLayerContent imageLayer:
				        ContentLogger.Log($"Processing image layer '{imageLayer.Name}'");
				        //var externalReference = new ExternalReference<Texture2DContent>(imageLayer.Image.Source);
				        var parameters = new OpaqueDataDictionary
				        {
				            { "ColorKeyColor", imageLayer.Image.TransparentColor },
				            { "ColorKeyEnabled", true }
				        };
				        //imageLayer.Image.ContentRef = context.BuildAsset<Texture2DContent, Texture2DContent>(externalReference, "", parameters, "", "");
				        contentItem.BuildExternalReference<Texture2DContent>(context, imageLayer.Image.Source, parameters);
				        ContentLogger.Log($"Processed image layer '{imageLayer.Name}'");
				        break;

				    case TiledMapTileLayerContent tileLayer when tileLayer.Data.Chunks.Count > 0:
				        throw new NotSupportedException($"{map.FilePath} contains data chunks. These are currently not supported.");

				    case TiledMapTileLayerContent tileLayer:
				        var data = tileLayer.Data;
				        var encodingType = data.Encoding ?? "xml";
				        var compressionType = data.Compression ?? "xml";

				        ContentLogger.Log($"Processing tile layer '{tileLayer.Name}': Encoding: '{encodingType}', Compression: '{compressionType}'");
				        var tileData = DecodeTileLayerData(encodingType, tileLayer);
				        var tiles = CreateTiles(map.RenderOrder, map.Width, map.Height, tileData);
				        tileLayer.Tiles = tiles;
				        ContentLogger.Log($"Processed tile layer '{tileLayer}': {tiles.Length} tiles");
				        break;

				    case TiledMapObjectLayerContent objectLayer:
				        ContentLogger.Log($"Processing object layer '{objectLayer.Name}'");

				        foreach (var obj in objectLayer.Objects)
				            TiledMapContentHelper.Process(obj, context);

				        ContentLogger.Log($"Processed object layer '{objectLayer.Name}'");
				        break;

				    case TiledMapGroupLayerContent groupLayer:
				        ProcessLayers(contentItem, map, context, groupLayer.Layers);
				        break;
				}
			}
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

            return compressionMode switch
            {
                "gzip" => new GZipStream(memoryStream, CompressionMode.Decompress),
                "zlib" => new ZlibStream(memoryStream, Framework.Utilities.Deflate.CompressionMode.Decompress),
                _ => memoryStream
            };
        }
    }
}