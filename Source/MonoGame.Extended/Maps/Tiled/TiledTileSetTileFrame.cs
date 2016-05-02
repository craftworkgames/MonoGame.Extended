using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Extended.Maps.Tiled
{
    public class TiledTileSetTileFrame
    {
        public TiledTileSetTileFrame(int order, int tileId, int duration)
        {
            Order = order;
            TileId = tileId;
            Duration = duration;
        }
        public int Order { get; set; }
        public int TileId { get; set; }
        public int Duration { get; set; }
        public override string ToString()
        {
            return $"{Order}:{TileId}:{Duration}";
        }
    }
}
