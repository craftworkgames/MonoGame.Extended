using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapReader : ContentTypeReader<TiledMap>
    {
        protected override TiledMap Read(ContentReader reader, TiledMap existingInstance)
        {
            //var backgroundColor = reader.ReadColor();
            var renderOrder = (TiledMapRenderOrder) Enum.Parse(typeof (TiledMapRenderOrder), reader.ReadString(), true);
            var tileMap = new TiledMap(
                graphicsDevice: reader.ContentManager.GetGraphicsDevice(),
                width: reader.ReadInt32(),
                height: reader.ReadInt32(),
                tileWidth: reader.ReadInt32(),
                tileHeight: reader.ReadInt32())
            {
                //BackgroundColor = backgroundColor,
                RenderOrder = renderOrder
            };

            ReadCustomProperties(reader, tileMap.Properties);

            var tilesetCount = reader.ReadInt32();

            for (var i = 0; i < tilesetCount; i++)
            {
                var assetName = reader.ReadString();
                var texture = reader.ContentManager.Load<Texture2D>(assetName);
                var tileset = tileMap.CreateTileset(
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
                var tileDataCount = reader.ReadInt32();
                var tileData = new int[tileDataCount];

                for (var d = 0; d < tileDataCount; d++)
                    tileData[d] = reader.ReadInt32();

                var layer = tileMap.CreateLayer(
                    name: reader.ReadString(),
                    width: reader.ReadInt32(),
                    height: reader.ReadInt32(),
                    data: tileData);

                ReadCustomProperties(reader, layer.Properties);
            }
            
            return tileMap;
        }

        private void ReadCustomProperties(ContentReader reader, TiledProperties properties)
        {
            var count = reader.ReadInt32();

            for (var i = 0; i < count; i++)
                properties.Add(reader.ReadString(), reader.ReadString());
        }
    }
}