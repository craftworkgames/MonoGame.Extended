using System;
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
            var renderOrder = (TiledRenderOrder) Enum.Parse(typeof (TiledRenderOrder), reader.ReadString(), true);
            var tiledMap = new TiledMap(
                graphicsDevice: reader.ContentManager.GetGraphicsDevice(),
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
                    spacing: reader.ReadInt32(),
                    margin: reader.ReadInt32());
                ReadCustomProperties(reader, tileset.Properties);
            }

            var layerCount = reader.ReadInt32();

            for (var i = 0; i < layerCount; i++)
            {
                var layer = ReadLayer(reader, tiledMap);
                ReadCustomProperties(reader, layer.Properties);
            }

            return tiledMap;
        }

        private static void ReadCustomProperties(ContentReader reader, TiledProperties properties)
        {
            var count = reader.ReadInt32();

            for (var i = 0; i < count; i++)
                properties.Add(reader.ReadString(), reader.ReadString());
        }

        private static TiledLayer ReadLayer(ContentReader reader, TiledMap tiledMap)
        {
            var layerName = reader.ReadString();
            // ReSharper disable UnusedVariable
            var visible = reader.ReadBoolean();
            var opacity = reader.ReadSingle();
            // ReSharper restore UnusedVariable
            var layerType = reader.ReadString();

            if (layerType == "TileLayer")
                return ReadTileLayer(reader, tiledMap, layerName);

            if (layerType == "ImageLayer")
                return ReadImageLayer(reader, tiledMap, layerName);

            throw new NotSupportedException(string.Format("Layer type {0} is not supported", layerType));
        }

        private static TiledTileLayer ReadTileLayer(ContentReader reader, TiledMap tileMap, string layerName)
        {
            var tileDataCount = reader.ReadInt32();
            var tileData = new int[tileDataCount];

            for (var d = 0; d < tileDataCount; d++)
                tileData[d] = reader.ReadInt32();

            return tileMap.CreateTileLayer(
                name: layerName,
                width: reader.ReadInt32(),
                height: reader.ReadInt32(),
                data: tileData);
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