#region

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace MonoGame.Extended.Tiled.Graphics
{
    public sealed class TiledMapLayerAnimatedModel : TiledMapLayerModel
    {
        public TiledMapTilesetAnimatedTile[] AnimatedTilesetTiles { get; }

        internal TiledMapLayerAnimatedModel(ContentReader input, TiledMap map) 
            : base(input, true)
        {
            var tilesetFirstGlobalIdentifier = input.ReadInt32();
            var tileset = map.GetTilesetByTileGlobalIdentifier(tilesetFirstGlobalIdentifier);

            var animatedTilesetTileCount = input.ReadInt32();
            AnimatedTilesetTiles = new TiledMapTilesetAnimatedTile[animatedTilesetTileCount];

            for (var i = 0; i < animatedTilesetTileCount; i++)
            {
                var tileLocalIdentifier = input.ReadInt32();
                AnimatedTilesetTiles[i] = tileset.GetAnimatedTilesetTileByLocalTileIdentifier(tileLocalIdentifier);
            }
        }
    }
}