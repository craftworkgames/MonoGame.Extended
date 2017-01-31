using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.Tiled.Graphics
{
    public sealed class TiledMapLayerAnimatedModel : TiledMapLayerModel
    {
        public TiledMapTilesetAnimatedTile[] AnimatedTilesetTiles { get; }

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
    }
}