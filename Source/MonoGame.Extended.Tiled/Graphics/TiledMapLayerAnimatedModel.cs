using Microsoft.Xna.Framework.Content;

namespace MonoGame.Extended.Tiled.Graphics
{
    public sealed class TiledMapLayerAnimatedModel : TiledMapLayerModel
    {
        internal TiledMapLayerAnimatedModel(ContentReader reader, TiledMap map) 
            : base(reader, true)
        {
            var tilesetFirstGlobalIdentifier = reader.ReadInt32();
            var tileset = map.GetTilesetByTileGlobalIdentifier(tilesetFirstGlobalIdentifier);

            var animatedTilesetTileCount = reader.ReadInt32();
            AnimatedTilesetTiles = new TiledMapTilesetAnimatedTile[animatedTilesetTileCount];

            for (var i = 0; i < animatedTilesetTileCount; i++)
            {
                var tileLocalIdentifier = reader.ReadInt32();
                AnimatedTilesetTiles[i] = tileset.GetAnimatedTilesetTileByLocalTileIdentifier(tileLocalIdentifier);
            }
        }

        public TiledMapTilesetAnimatedTile[] AnimatedTilesetTiles { get; }
    }
}