using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Tiled;
using MonoGame.Utilities;
using CompressionMode = System.IO.Compression.CompressionMode;

namespace MonoGame.Extended.Content.Pipeline.Tiled
{
    [ContentProcessor(DisplayName = "Tiled Map Processor - MonoGame.Extended")]
    public class TiledMapProcessor : ContentProcessor<TiledMapContent, TiledMapContent>
    {
        public override TiledMapContent Process(TiledMapContent map, ContentProcessorContext context)
        {
            ContentLogger.Logger = context.Logger;

            var previousWorkingDirectory = Environment.CurrentDirectory;
            var newWorkingDirectory = Path.GetDirectoryName(map.FilePath);
            if (string.IsNullOrEmpty(newWorkingDirectory))
                throw new NullReferenceException();
            Environment.CurrentDirectory = newWorkingDirectory;

            foreach (var layer in map.Layers)
            {
                var imageLayer = layer as TiledMapImageLayerContent;
                if (imageLayer != null)
                {
                    ContentLogger.Log($"Processing image layer '{imageLayer.Name}'");

                    var model = CreateImageLayerModel(imageLayer);
                    layer.Models = new[]
                    {
                        model
                    };

                    ContentLogger.Log($"Processed image layer '{imageLayer.Name}'");
                    continue;
                }

                var tileLayer = layer as TiledMapTileLayerContent;
                if (tileLayer == null)
                    continue;

                var data = tileLayer.Data;
                var encodingType = data.Encoding ?? "xml";
                var compressionType = data.Compression ?? "xml";

                ContentLogger.Log(
                    $"Processing tile layer '{tileLayer.Name}': Encoding: '{encodingType}', Compression: '{compressionType}'");

                var tileData = DecodeTileLayerData(encodingType, tileLayer);
                var tiles = CreateTiles(map.RenderOrder, map.Width, map.Height, tileData);
                tileLayer.Tiles = tiles;

                layer.Models = CreateTileLayerModels(map, tileLayer.Name, tiles).ToArray();

                ContentLogger.Log($"Processed tile layer '{tileLayer}': {tiles.Length} tiles");
            }

            Environment.CurrentDirectory = previousWorkingDirectory;

            return map;
        }

        private static List<TiledMapTileContent> DecodeTileLayerData(string encodingType,
            TiledMapTileLayerContent tileLayer)
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

        private static TiledMapTile[] CreateTiles(TiledMapTileDrawOrderContent renderOrder, int mapWidth, int mapHeight,
             List<TiledMapTileContent> tileData)
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

        private static IEnumerable<TiledMapTile> CreateTilesInLeftDownOrder(
            // ReSharper disable once SuggestBaseTypeForParameter
            List<TiledMapTileContent> tileLayerData, int mapWidth, int mapHeight)
        {
            for (var y = 0; y < mapHeight; y++)
            for (var x = mapWidth - 1; x >= 0; x--)
            {
                var dataIndex = x + y * mapWidth;
                var globalIdentifier = tileLayerData[dataIndex].GlobalIdentifier;
                if (globalIdentifier == 0)
                    continue;
                var tile = new TiledMapTile(globalIdentifier, (ushort) x, (ushort) y);
                yield return tile;
            }
        }

        private static IEnumerable<TiledMapTile> CreateTilesInLeftUpOrder(
            // ReSharper disable once SuggestBaseTypeForParameter
            List<TiledMapTileContent> tileLayerData, int mapWidth, int mapHeight)
        {
            for (var y = mapHeight - 1; y >= 0; y--)
            for (var x = mapWidth - 1; x >= 0; x--)
            {
                var dataIndex = x + y * mapWidth;
                var globalIdentifier = tileLayerData[dataIndex].GlobalIdentifier;
                if (globalIdentifier == 0)
                    continue;
                var tile = new TiledMapTile(globalIdentifier, (ushort) x, (ushort) y);
                yield return tile;
            }
        }

        private static IEnumerable<TiledMapTile> CreateTilesInRightDownOrder(
            // ReSharper disable once SuggestBaseTypeForParameter
            List<TiledMapTileContent> tileLayerData, int mapWidth, int mapHeight)
        {
            for (var y = 0; y < mapHeight; y++)
            for (var x = 0; x < mapWidth; x++)
            {
                var dataIndex = x + y * mapWidth;
                var globalIdentifier = tileLayerData[dataIndex].GlobalIdentifier;
                if (globalIdentifier == 0)
                    continue;
                var tile = new TiledMapTile(globalIdentifier, (ushort) x, (ushort) y);
                yield return tile;
            }
        }

        private static IEnumerable<TiledMapTile> CreateTilesInRightUpOrder(
            // ReSharper disable once SuggestBaseTypeForParameter
            List<TiledMapTileContent> tileLayerData, int mapWidth, int mapHeight)
        {
            for (var y = mapHeight - 1; y >= 0; y--)
            for (var x = mapWidth - 1; x >= 0; x--)
            {
                var dataIndex = x + y * mapWidth;
                var globalIdentifier = tileLayerData[dataIndex].GlobalIdentifier;
                if (globalIdentifier == 0)
                    continue;
                var tile = new TiledMapTile(globalIdentifier, (ushort) x, (ushort) y);
                yield return tile;
            }
        }

        private static List<TiledMapTileContent> DecodeBase64Data(TiledMapTileLayerDataContent data, int width,
            int height)
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

            return tileList;
        }

        private static List<TiledMapTileContent> DecodeCommaSeperatedValuesData(TiledMapTileLayerDataContent data)
        {
            return data.Value
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(uint.Parse)
                .Select(x => new TiledMapTileContent {GlobalIdentifier = x})
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

        private static TiledMapLayerModelContent CreateImageLayerModel(TiledMapImageLayerContent imageLayer)
        {
            var model = new TiledMapLayerModelContent(imageLayer.Name, imageLayer.Image);
            // sprite/tile; same thing
            model.AddTileIndices();
            model.AddTileVertices(new Point2(imageLayer.X, imageLayer.Y));

            return model;
        }

        private static IEnumerable<TiledMapLayerModelContent> CreateTileLayerModels(TiledMapContent map,
            string layerName, IEnumerable<TiledMapTile> tiles)
        {
            // the code below builds the geometry (triangles) for every tile
            // for every unique tileset used by a tile in a layer, we are going to end up with a different model (list of vertices and list of indices pair)
            // we also could end up with more models if the map is very large
            // regardless, each model is going to require one draw call to render at runtime

            var modelsByTileset = new Dictionary<TiledMapTilesetContent, List<TiledMapLayerModelContent>>();

            // loop through all the tiles in the proper render order, building the geometry for each tile
            // by processing the tiles in the correct rendering order we ensure the geometry for the tiles will be rendered correctly later using the painter's algorithm
            foreach (var tile in tiles)
            {
                // get the tileset for this tile
                var tileGlobalIdentifier = tile.GlobalIdentifier;
                var tileset = map.Tilesets.FirstOrDefault(x => x.ContainsGlobalIdentifier(tileGlobalIdentifier));
                if (tileset == null)
                    throw new NullReferenceException(
                        $"Could not find tileset for global tile identifier '{tileGlobalIdentifier}'");

                var localTileIdentifier = tileGlobalIdentifier - tileset.FirstGlobalIdentifier;
                Debug.Assert(tileset != null);

                // check if this tile is animated
                var tilesetTile = tileset.Tiles.FirstOrDefault(x => x.LocalIdentifier == localTileIdentifier);
                var isAnimatedTile = tilesetTile?.Frames != null && tilesetTile.Frames.Count > 0;

                // check if we already have built a list of models for this tileset
                TiledMapLayerModelContent model;
                List<TiledMapLayerModelContent> models;

                if (modelsByTileset.TryGetValue(tileset, out models))
                {
                    // if we found the list of models for this tileset, try to use the last model added
                    // (assuming the the ones before the last are all full)
                    model = models.FindLast(x => x is TiledMapLayerAnimatedModelContent == isAnimatedTile);
                    // since it is possible that the correct type of model was not added yet we might have to create the correct model now
                    if (model == null)
                    {
                        model = isAnimatedTile
                            ? new TiledMapLayerAnimatedModelContent(layerName, tileset)
                            : new TiledMapLayerModelContent(layerName, tileset);
                        models.Add(model);
                    }
                }
                else
                {
                    // if we have not found the list of models for this tileset, we need to create the list and start a new model of the correct type
                    models = new List<TiledMapLayerModelContent>();
                    model = isAnimatedTile
                        ? new TiledMapLayerAnimatedModelContent(layerName, tileset)
                        : new TiledMapLayerModelContent(layerName, tileset);
                    models.Add(model);
                    modelsByTileset.Add(tileset, models);
                }

                // check if the current model is full
                if (model.Vertices.Count + TiledMapHelper.VerticesPerTile > TiledMapHelper.MaximumVerticesPerModel)
                {
                    // if the current model is full, we need to start a new one
                    model = isAnimatedTile
                        ? new TiledMapLayerAnimatedModelContent(layerName, tileset)
                        : new TiledMapLayerModelContent(layerName, tileset);
                    models.Add(model);
                }

                // if the tile is animated, record the index of animated tile for the tilset so we can get the correct texture coordinates at runtime
                if (isAnimatedTile)
                {
                    var animatedModel = (TiledMapLayerAnimatedModelContent) model;
                    animatedModel.AddAnimatedTile(tilesetTile);
                }

                // fixes a problem
                if (tileset.Columns == 0)
                    tileset.Columns = 1;

                // build the geometry for the tile
                var position = GetTilePosition(map, tile);
                var sourceRectangle = TiledMapHelper.GetTileSourceRectangle(localTileIdentifier, tileset.TileWidth,
                    tileset.TileHeight, tileset.Columns, tileset.Margin, tileset.Spacing);
                var flipFlags = tile.Flags;
                model.AddTileIndices();
                model.AddTileVertices(position, sourceRectangle, flipFlags);
            }

            // for each tileset used in this layer
            foreach (var keyValuePair in modelsByTileset)
            {
                var models = keyValuePair.Value;

                // and for each model apart of a tileset
                foreach (var model in models)
                    yield return model;
            }
        }

        private static Point2 GetTilePosition(TiledMapContent map, TiledMapTile mapTile)
        {
            switch (map.Orientation)
            {
                case TiledMapOrientationContent.Orthogonal:
                    return TiledMapHelper.GetOrthogonalPosition(mapTile.X, mapTile.Y, map.TileWidth, map.TileHeight);
                case TiledMapOrientationContent.Isometric:
                    return TiledMapHelper.GetIsometricPosition(mapTile.X, mapTile.Y, map.TileWidth, map.TileHeight);
                case TiledMapOrientationContent.Staggered:
                    throw new NotImplementedException("Staggered maps are not yet implemented.");
                default:
                    throw new NotSupportedException($"Tiled Map {map.Orientation} is not supported.");
            }
        }
    }
}