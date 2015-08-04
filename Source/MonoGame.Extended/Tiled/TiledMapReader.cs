using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;

namespace MonoGame.Extended.Tiled
{
    public class TiledMapReader : ContentTypeReader<TiledMap>
    {
        protected override TiledMap Read(ContentReader input, TiledMap existingInstance)
        {
            var tileMap = new TiledMap(
                graphicsDevice: input.ContentManager.GetGraphicsDevice(), 
                width: input.ReadInt32(),
                height: input.ReadInt32(),
                tileWidth: input.ReadInt32(),
                tileHeight: input.ReadInt32());
            
            var tileSetCount = input.ReadInt32();

            for (var i = 0; i < tileSetCount; i++)
            {
                var assetName = input.ReadString();
                var texture = input.ContentManager.Load<Texture2D>(assetName);
                tileMap.CreateTileSet(
                    texture: texture,
                    firstId: input.ReadInt32(),
                    tileWidth: input.ReadInt32(),
                    tileHeight: input.ReadInt32(),
                    spacing: input.ReadInt32(),
                    margin: input.ReadInt32());
            }

            var layerCount = input.ReadInt32();

            for (var i = 0; i < layerCount; i++)
            {
                var tileDataCount = input.ReadInt32();
                var tileData = new int[tileDataCount];

                for (var d = 0; d < tileDataCount; d++)
                    tileData[d] = input.ReadInt32();

                tileMap.CreateLayer(
                    name: input.ReadString(),
                    width: input.ReadInt32(),
                    height: input.ReadInt32(),
                    data: tileData);
            }
            
            return tileMap;
        }
    }
}