using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.Shapes;

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
                var trans = reader.ReadColor();
                texture = MakeColorTransparent(texture.GraphicsDevice, texture, trans);

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
                        tileSetTile.CreateTileSetTileFrame(k, frameId, reader.ReadInt32());
                    }

                    ReadCustomProperties(reader, tileSetTile.Properties);
                }

                ReadCustomProperties(reader, tileset.Properties);
            }

            var layerCount = reader.ReadInt32();
            var depthInc = 1.0f / (layerCount - 1);

            for (var i = 0; i < layerCount; i++)
            {
                var depth = 0.0f;

                if (layerCount > 1)
                    depth = 0.0f - i * depthInc;

                var layer = ReadLayer(reader, tiledMap, depth);

                if (!(layer is TiledObjectLayer))
                    ReadCustomProperties(reader, layer.Properties);
            }

            return tiledMap;
        }

        private static TiledObjectLayer ReadObjectLayer(ContentReader reader, TiledMap tiledMap)
        {
            var layerName = reader.ReadString();
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
                var tilesetTile = tiledMap.GetTilesetTileById(gid);

                IShapeF shape = null;
                if ((objectType == TiledObjectType.Polyline) || (objectType == TiledObjectType.Polygon))
                {
                    var pointsCount = reader.ReadInt32();
                    var points = new List<Vector2>();

                    for (var j = 0; j < pointsCount; j++)
                        points.Add(reader.ReadVector2());

                    if (objectType == TiledObjectType.Polygon)
                    {
                        shape = new PolygonF(points);
                    }
                    else
                    {
                        shape = new PolylineF(points);
                    }
                }
                else if (objectType == TiledObjectType.Ellipse)
                {
                    Vector2 center = new Vector2(x + width / 2.0f, y + height / 2.0f);
                    shape = new EllipseF(center, width / 2.0f, height / 2.0f);
                }
                else if (objectType == TiledObjectType.Rectangle || objectType == TiledObjectType.Tile)
                {
                    shape = new RectangleF(x, y, width, height);
                }

                objects[i] = new TiledObject(objectType, id, gid >= 0 ? gid : (int?) null, shape, x, y, tilesetTile)
                {
                    IsVisible = isVisible,
                    Opacity = opacity,
                    Rotation = rotation,
                    Name = name,
                    Type = type
                };

                ReadCustomProperties(reader, objects[i].Properties);
            }

            return new TiledObjectLayer(layerName, objects) { IsVisible = visible };
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

        private static TiledLayer ReadLayerTypeProperties(ContentReader reader, TiledMap tiledMap, string layerName,
            string layerType)
        {
            if (layerType == "TileLayer")
                return ReadTileLayer(reader, tiledMap, layerName);

            if (layerType == "ImageLayer")
                return ReadImageLayer(reader, tiledMap, layerName);

            if (layerType == "ObjectLayer")
            {
                var layer = ReadObjectLayer(reader, tiledMap, layerName);
                ReadCustomProperties(reader, layer.Properties);
                return layer;
            }

            throw new NotSupportedException($"Layer type {layerType} is not supported");
        }

        private static TiledTileLayer ReadTileLayer(ContentReader reader, TiledMap tileMap, string layerName)
        {
            var tileDataCount = reader.ReadInt32();
            var tileData = new int[tileDataCount];

            for (var d = 0; d < tileDataCount; d++)
                tileData[d] = reader.ReadInt32();

            return tileMap.CreateTileLayer(layerName, reader.ReadInt32(), reader.ReadInt32(), tileData);
        }

        private static TiledImageLayer ReadImageLayer(ContentReader reader, TiledMap tileMap, string layerName)
        {
            var assetName = reader.GetRelativeAssetPath(reader.ReadString());
            var texture = reader.ContentManager.Load<Texture2D>(assetName);
            var position = reader.ReadVector2();

            return tileMap.CreateImageLayer(layerName, texture, position);
        }

        private static TiledObjectLayer ReadObjectLayer(ContentReader reader, TiledMap tileMap, string layerName)
        {
            var objectLayer = ReadObjectLayer(reader, tileMap);
            tileMap.AddLayer(objectLayer);
            return objectLayer;
        }

        public Texture2D MakeColorTransparent(GraphicsDevice graphicsDevice, Texture2D input, Color color)
        {
            var data = new Color[input.Width * input.Height];
            input.GetData(data);

            for (var i = 0; i < input.Width * input.Height; i++)
            {
                if (data[i].R == color.R && data[i].G == color.G && data[i].B == color.B)
                {
                    data[i].R = 0;
                    data[i].G = 0;
                    data[i].B = 0;
                    data[i].A = 0;
                }
            }

            var output = new Texture2D(graphicsDevice, input.Width, input.Height);
            output.SetData(data);

            return output;
        }
    }
}