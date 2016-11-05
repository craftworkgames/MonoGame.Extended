using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledMapReader : ContentTypeReader<TiledMap>
    {
        protected override TiledMap Read(ContentReader reader, TiledMap existingInstance)
        {
            var backgroundColor = reader.ReadColor();
            var renderOrder = (TiledRenderOrder)Enum.Parse(typeof(TiledRenderOrder), reader.ReadString(), true);
            var tiledMap = new TiledMap(
                name: reader.AssetName,
                width: reader.ReadInt32(),
                height: reader.ReadInt32(),
                tileWidth: reader.ReadInt32(),
                tileHeight: reader.ReadInt32(),
                orientation: (TiledMapOrientation)reader.ReadInt32())
            {
                BackgroundColor = backgroundColor,
                RenderOrder = renderOrder
            };

            ReadCustomProperties(reader, tiledMap.Properties);

            var tilesetCount = reader.ReadInt32();

            for (var i = 0; i < tilesetCount; i++)
            {
                var textureAssetName = reader.GetRelativeAssetPath(reader.ReadString());
                var texture = reader.ContentManager.Load<Texture2D>(textureAssetName);
                var tileset = tiledMap.CreateTileset(
                    texture: texture,
                    firstId: reader.ReadInt32(),
                    tileWidth: reader.ReadInt32(),
                    tileHeight: reader.ReadInt32(),
                    tileCount: reader.ReadInt32(),
                    spacing: reader.ReadInt32(),
                    margin: reader.ReadInt32());
                var tileSetTileCount = reader.ReadInt32();
                for (var j = 0; j < tileSetTileCount; j++)
                {
                    var tileId = reader.ReadInt32();
                    tileId = tileset.FirstId + tileId - 1;
                    var tileSetTile = tileset.CreateTileSetTile(tileId);
                    var tileSetTileFrameCount = reader.ReadInt32();
                    for (var k = 0; k < tileSetTileFrameCount; k++)
                    {
                        var frameId = reader.ReadInt32();
                        frameId = tileset.FirstId + frameId - 1;
                        tileSetTile.CreateTileSetTileFrame(order: k, tileId: frameId, duration: reader.ReadInt32());
                    }
                    ReadCustomProperties(reader, tileSetTile.Properties);
                    
                    tileSetTile.ObjectGroups.AddRange(ReadObjectGroups(reader));
                }
                ReadCustomProperties(reader, tileset.Properties);
            }

            var layerCount = reader.ReadInt32();

            float depthInc = 1.0f / (layerCount - 1);

            for (var i = 0; i < layerCount; i++)
            {
                float depth = 0.0f - (i * depthInc);
                var layer = ReadLayer(reader, tiledMap, depth);
                ReadCustomProperties(reader, layer.Properties);
            }

            tiledMap.AddObjectGroup(ReadObjectGroups(reader));

            return tiledMap;
        }

        private static List<TiledObjectGroup> ReadObjectGroups(ContentReader reader) 
        {
            var list = new List<TiledObjectGroup>();
            var objectGroupsCount = reader.ReadInt32();

            for (var i = 0; i < objectGroupsCount; i++)
            {
                var objectGroup = ReadObjectGroup(reader);
                ReadCustomProperties(reader, objectGroup.Properties);
                list.Add(objectGroup);
            }
            return list;
        }

        private static TiledObjectGroup ReadObjectGroup(ContentReader reader)
        {
            var groupName = reader.ReadString();
            var visible = reader.ReadBoolean();
            var opacity = reader.ReadSingle();
            var count = reader.ReadInt32();
            var objects = new TiledObject[count];

            for (var i = 0; i < count; i++)
            {
                var objectType = (TiledObjectType)reader.ReadInt32();
                var id = reader.ReadInt32();
                var gid = reader.ReadInt32();
                var x = reader.ReadSingle();
                var y = reader.ReadSingle();
                var width = reader.ReadSingle();
                var height = reader.ReadSingle();
                var rotation = reader.ReadSingle();
                var name = reader.ReadString();
                var type = reader.ReadString();
                var isVisible = reader.ReadBoolean();

                objects[i] = new TiledObject(objectType, id, gid >= 0 ? gid : (int?)null, x, y, width, height)
                {
                    IsVisible = isVisible,
                    Opacity = opacity,
                    Rotation = rotation,
                    Name = name,
                    Type = type
                };

                if (objectType == TiledObjectType.Polyline || objectType == TiledObjectType.Polygon)
                {
                    var pointsCount = reader.ReadInt32();

                    for (var j = 0; j < pointsCount; j++)
                        objects[i].Points.Add(reader.ReadVector2());
                }

                ReadCustomProperties(reader, objects[i].Properties);
            }
            
            return new TiledObjectGroup(groupName, objects) { IsVisible = visible };
        }

        private static void ReadCustomProperties(ContentReader reader, TiledProperties properties)
        {
            var count = reader.ReadInt32();

            for (var i = 0; i < count; i++)
                properties.Add(reader.ReadString(), reader.ReadString());
        }

        private static TiledLayer ReadLayer(ContentReader reader, TiledMap tiledMap, float depth)
        {
            var layerName = reader.ReadString();
            var visible = reader.ReadBoolean();
            var opacity = reader.ReadSingle();
            var offsetx = reader.ReadSingle();
            var offsety = reader.ReadSingle();
            var layerType = reader.ReadString();
            var layer = ReadLayerTypeProperties(reader, tiledMap, layerName, layerType);
            layer.IsVisible = visible;
            layer.Opacity = opacity;
            layer.OffsetX = offsetx;
            layer.OffsetY = offsety;
            layer.Depth = depth;
            return layer;
        }

        private static TiledLayer ReadLayerTypeProperties(ContentReader reader, TiledMap tiledMap, string layerName, string layerType)
        {
            if (layerType == "TileLayer")
                return ReadTileLayer(reader, tiledMap, layerName);

            if (layerType == "ImageLayer")
                return ReadImageLayer(reader, tiledMap, layerName);

            throw new NotSupportedException($"Layer type {layerType} is not supported");
        }

        private static TiledTileLayer ReadTileLayer(ContentReader reader, TiledMap tileMap, string layerName)
        {
            var tileDataCount = reader.ReadInt32();
            var tileData = new int[tileDataCount];

            for (var d = 0; d < tileDataCount; d++)
                tileData[d] = reader.ReadInt32();

            return tileMap.CreateTileLayer(name: layerName, width: reader.ReadInt32(), height: reader.ReadInt32(), data: tileData);
        }

        private static TiledImageLayer ReadImageLayer(ContentReader reader, TiledMap tileMap, string layerName)
        {
            var assetName = reader.GetRelativeAssetPath(reader.ReadString());
            var texture = reader.ContentManager.Load<Texture2D>(assetName);
            var position = reader.ReadVector2();

            return tileMap.CreateImageLayer(layerName, texture, position);
        }
    }
}