using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;

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
				var firstGlobalIdentifier = reader.ReadInt32();
                var tileset = ReadTileset(reader, map);
                map.AddTileset(tileset, firstGlobalIdentifier);
            }
        }
        
        private static TiledMapTileset ReadTileset(ContentReader reader, TiledMap map)
        {
            var external = reader.ReadBoolean();
			var tileset = external ? reader.ReadExternalReference<TiledMapTileset>() : TiledMapTilesetReader.ReadTileset(reader);

			return tileset;
        }

        private static void ReadLayers(ContentReader reader, TiledMap map)
		{
			foreach (var layer in ReadGroup(reader, map))
				map.AddLayer(layer);
		}
		private static List<TiledMapLayer> ReadGroup(ContentReader reader, TiledMap map)
        {
            var layerCount = reader.ReadInt32();
			var value = new List<TiledMapLayer>(layerCount);

            for (var i = 0; i < layerCount; i++)
                value.Add(ReadLayer(reader, map));

			return value;
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
				case TiledMapLayerType.GroupLayer:
					layer = new TiledMapGroupLayer(name, ReadGroup(reader, map), offset, opacity, isVisible);
					break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

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
                    mapObject = new TiledMapRectangleObject(identifier, name, size, position, rotation, opacity, isVisible, type);
                    break;
                case TiledMapObjectType.Tile:
                    var globalTileIdentifierWithFlags = reader.ReadUInt32();
                    var tile = new TiledMapTile(globalTileIdentifierWithFlags, (ushort)position.X, (ushort)position.Y);
                    var tileset = map.GetTilesetByTileGlobalIdentifier(tile.GlobalIdentifier);
                    var localTileIdentifier = tile.GlobalIdentifier - map.GetTilesetFirstGlobalIdentifier(tileset);
                    var tilesetTile = tileset.Tiles.FirstOrDefault(x => x.LocalTileIdentifier == localTileIdentifier);
                    mapObject = new TiledMapTileObject(identifier, name, tileset, tilesetTile, size, position, rotation, opacity, isVisible, type);
                    break;
                case TiledMapObjectType.Ellipse:
                    mapObject = new TiledMapEllipseObject(identifier, name, size, position, rotation, opacity, isVisible);
                    break;
                case TiledMapObjectType.Polygon:
                    mapObject = new TiledMapPolygonObject(identifier, name, ReadPoints(reader), size, position, rotation, opacity, isVisible, type);
                    break;
                case TiledMapObjectType.Polyline:
                    mapObject = new TiledMapPolylineObject(identifier, name, ReadPoints(reader), size, position, rotation, opacity, isVisible, type);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var property in properties)
                mapObject.Properties.Add(property.Key, property.Value);

            return mapObject;
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
            var texture = reader.ReadExternalReference<Texture2D>();
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

                tiles[x + y * width] = new TiledMapTile(globalTileIdentifierWithFlags, x, y);
            }

            return new TiledMapTileLayer(name, width, height, tileWidth, tileHeight, tiles, offset, opacity, isVisible);
        }
    }
}