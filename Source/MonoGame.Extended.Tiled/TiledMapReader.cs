using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Tiled.Graphics;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapReader : ContentTypeReader<TiledMap>
    {
        protected override TiledMap Read(ContentReader reader, TiledMap existingInstance)
        {
            if (existingInstance != null)
                return existingInstance;

            var map = ReadTiledMap(reader);
            ReadProperties(reader, map.Properties);
            ReadTilesets(reader, map);
            ReadLayers(reader, map);
            return map;
        }

        private static void ReadProperties(ContentReader reader, TiledMapProperties properties)
        {
            var count = reader.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                var key = reader.ReadString();
                var value = reader.ReadString();
                properties[key] = value;
            }
        }

        private static TiledMap ReadTiledMap(ContentReader reader)
        {
            var name = reader.AssetName;
            var width = reader.ReadInt32();
            var height = reader.ReadInt32();
            var tileWidth = reader.ReadInt32();
            var tileHeight = reader.ReadInt32();
            var backgroundColor = reader.ReadColor();
            var renderOrder = (TiledMapTileDrawOrder)reader.ReadByte();
            var orientation = (TiledMapOrientation)reader.ReadByte();

            return new TiledMap(name, width, height, tileWidth, tileHeight, renderOrder, orientation, backgroundColor);
        }

        private static void ReadTilesets(ContentReader reader, TiledMap map)
        {
            var tilesetCount = reader.ReadInt32();

            for (var i = 0; i < tilesetCount; i++)
            {
                var tileset = ReadTileset(reader, map);
                map.AddTileset(tileset);
            }
        }



        private static TiledMapTileset ReadTileset(ContentReader input, TiledMap map)
        {
            var textureAssetName = input.GetRelativeAssetName(input.ReadString());
            var texture = input.ContentManager.Load<Texture2D>(textureAssetName);
            var firstGlobalIdentifier = input.ReadInt32();
            var tileWidth = input.ReadInt32();
            var tileHeight = input.ReadInt32();
            var tileCount = input.ReadInt32();
            var spacing = input.ReadInt32();
            var margin = input.ReadInt32();
            var columns = input.ReadInt32();
            var explicitTileCount = input.ReadInt32();

            var tileset = new TiledMapTileset(texture, firstGlobalIdentifier, tileWidth, tileHeight, tileCount, spacing, margin, columns);

            for (var tileIndex = 0; tileIndex < explicitTileCount; tileIndex++)
            {
                var localTileIdentifier = input.ReadInt32();
                var animationFramesCount = input.ReadInt32();
                var tilesetTile = animationFramesCount <= 0 
                    ? new TiledMapTilesetTile(localTileIdentifier, input) 
                    : new TiledMapTilesetAnimatedTile(tileset, input, localTileIdentifier, animationFramesCount);

                ReadProperties(input, tilesetTile.Properties);
                tileset.Tiles.Add(tilesetTile);
            }

            ReadProperties(input, tileset.Properties);
            return tileset;
        }

        private static void ReadLayers(ContentReader reader, TiledMap map)
        {
            var layerCount = reader.ReadInt32();

            for (var i = 0; i < layerCount; i++)
            {
                var layer = ReadLayer(reader, map);
                map.AddLayer(layer);
            }
        }

        private static TiledMapLayer ReadLayer(ContentReader reader, TiledMap map)
        {
            var layerType = (TiledMapLayerType)reader.ReadByte();
            var name = reader.ReadString();
            var isVisible = reader.ReadBoolean();
            var opacity = reader.ReadSingle();
            var offsetX = reader.ReadSingle();
            var offsetY = reader.ReadSingle();
            var offset = new Vector2(offsetX, offsetY);
            var properties = new TiledMapProperties();

            ReadProperties(reader, properties);

            TiledMapLayer layer;

            switch (layerType)
            {
                case TiledMapLayerType.ImageLayer:
                    layer = ReadImageLayer(reader, name, offset, opacity, isVisible);
                    break;
                case TiledMapLayerType.TileLayer:
                    layer = ReadTileLayer(reader, name, offset, opacity, isVisible, map);
                    break;
                case TiledMapLayerType.ObjectLayer:
                    layer = ReadObjectLayer(reader, name, offset, opacity, isVisible, map);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (layerType != TiledMapLayerType.ObjectLayer)
                ReadModels(reader, layer, map);

            foreach (var property in properties)
                layer.Properties.Add(property.Key, property.Value);

            return layer;
        }

        private static TiledMapLayer ReadObjectLayer(ContentReader reader, string name, Vector2 offset, float opacity, bool isVisible, TiledMap map)
        {
            var color = reader.ReadColor();
            var drawOrder = (TiledMapObjectDrawOrder)reader.ReadByte();
            var objectCount = reader.ReadInt32();
            var objects = new TiledMapObject[objectCount];

            for (var i = 0; i < objectCount; i++)
                objects[i] = ReadTiledMapObject(reader, map);

            return new TiledMapObjectLayer(name, objects, color, drawOrder, offset, opacity, isVisible);
        }

        private static TiledMapObject ReadTiledMapObject(ContentReader reader, TiledMap map)
        {
            var objectType = (TiledMapObjectType)reader.ReadByte();
            var identifier = reader.ReadInt32();
            var name = reader.ReadString();
            var type = reader.ReadString();
            var position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            var width = reader.ReadSingle();
            var height = reader.ReadSingle();
            var size = new Size2(width, height);
            var rotation = reader.ReadSingle();
            var isVisible = reader.ReadBoolean();
            var properties = new TiledMapProperties();
            const float opacity = 1.0f;

            ReadProperties(reader, properties);

            TiledMapObject mapObject;

            switch (objectType)
            {
                case TiledMapObjectType.Rectangle:
                    mapObject = new TiledMapRectangleObject(identifier, name, size, position, rotation, opacity, isVisible);
                    break;
                case TiledMapObjectType.Tile:
                    var globalTileIdentifierWithFlags = reader.ReadUInt32();
                    var tile = new TiledMapTile(globalTileIdentifierWithFlags);
                    var tileset = map.GetTilesetByTileGlobalIdentifier(tile.GlobalIdentifier);
                    var localTileIdentifier = tile.GlobalIdentifier - tileset.FirstGlobalIdentifier;
                    var tilesetTile = tileset.Tiles.FirstOrDefault(x => x.LocalTileIdentifier == localTileIdentifier);
                    mapObject = new TiledMapTileObject(identifier, name, tileset, tilesetTile, size, position, rotation, opacity, isVisible);
                    break;
                case TiledMapObjectType.Ellipse:
                    mapObject = new TiledMapEllipseObject(identifier, name, size, position, rotation, opacity, isVisible);
                    break;
                case TiledMapObjectType.Polygon:
                    mapObject = new TiledMapPolygonObject(identifier, name, ReadPoints(reader), size, position, rotation, opacity, isVisible);
                    break;
                case TiledMapObjectType.Polyline:
                    var points = ReadPoints(reader);
                    mapObject = new TiledMapPolylineObject(identifier, name, ReadPoints(reader), size, position, rotation, opacity, isVisible);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var property in properties)
                mapObject.Properties.Add(property.Key, property.Value);

            throw new ArgumentOutOfRangeException();
        }

        private static Point2[] ReadPoints(ContentReader reader)
        {
            var pointCount = reader.ReadInt32();
            var points = new Point2[pointCount];

            for (var i = 0; i < pointCount; i++)
            {
                var x = reader.ReadSingle();
                var y = reader.ReadSingle();
                points[i] = new Point2(x, y);
            }

            return points;
        }

        private static TiledMapImageLayer ReadImageLayer(ContentReader reader, string name, Vector2 offset, float opacity, bool isVisible)
        {
            var textureAssetName = reader.GetRelativeAssetName(reader.ReadString());
            var texture = reader.ContentManager.Load<Texture2D>(textureAssetName);
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var position = new Vector2(x, y);
            return new TiledMapImageLayer(name, texture, position, offset, opacity, isVisible);
        }

        private static TiledMapTileLayer ReadTileLayer(ContentReader reader, string name, Vector2 offset, float opacity, bool isVisible, TiledMap map)
        {
            var width = reader.ReadInt32();
            var height = reader.ReadInt32();
            var tileWidth = map.TileWidth;
            var tileHeight = map.TileHeight;

            var tileCount = reader.ReadInt32();
            var tiles = new TiledMapTile[width * height];

            for (var i = 0; i < tileCount; i++)
            {
                var globalTileIdentifierWithFlags = reader.ReadUInt32();
                var x = reader.ReadUInt16();
                var y = reader.ReadUInt16();

                tiles[x + y * width] = new TiledMapTile(globalTileIdentifierWithFlags);
            }

            return new TiledMapTileLayer(name, width, height, tileWidth, tileHeight, tiles, offset, opacity, isVisible);
        }

        private static void ReadModels(ContentReader reader, TiledMapLayer layer, TiledMap map)
        {
            var modelCount = reader.ReadInt32();
            var animatedModelCount = reader.ReadInt32();

            var models = layer.Models = new TiledMapLayerModel[modelCount];
            var animatedModels = layer.AnimatedModels = new TiledMapLayerAnimatedModel[animatedModelCount];

            var animatedModelIndex = 0;

            for (var modelIndex = 0; modelIndex < modelCount; modelIndex++)
            {
                var isAnimated = reader.ReadBoolean();
                var model = isAnimated ? new TiledMapLayerAnimatedModel(reader, map) : new TiledMapLayerModel(reader);

                models[modelIndex] = model;
                if (isAnimated)
                    animatedModels[animatedModelIndex++] = (TiledMapLayerAnimatedModel)model;
            }
        }
    }
}